using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v3;
using Google.Apis.Services;
using System.Text.Json;
using HealthPredict.Models;
using HealthPredict.DAL;

namespace HealthPredict.API.Services
{
    public class GoogleDriveService
    {
        private readonly ILogger<GoogleDriveService> _logger;
        private readonly IConfiguration _configuration;
        private readonly HealthPredictContext _context;
        private DriveService? _driveService;

        // Configuración fija como solicitaste
        private const string ARCHIVO_FIJO = "HealthAutoExport-2025-07-04.json";
        private const string CARPETA_FIJA = "Mi unidad/HealthAutoExport/Health";
        private const int USUARIO_ID = 7; // Usuario fijo

        public GoogleDriveService(
            ILogger<GoogleDriveService> logger,
            IConfiguration configuration,
            HealthPredictContext context)
        {
            _logger = logger;
            _configuration = configuration;
            _context = context;
        }

        /// <summary>
        /// Inicializa el servicio de Google Drive
        /// </summary>
        private async Task<bool> InitializeDriveService()
        {
            try
            {
                // Buscar las credenciales en diferentes ubicaciones
                var credentialsPath = FindCredentialsFile();
                
                if (string.IsNullOrEmpty(credentialsPath))
                {
                    _logger.LogError("No se encontró el archivo credentials.json");
                    return false;
                }

                GoogleCredential credential;
                
                // Cargar credenciales desde archivo
                using (var stream = new FileStream(credentialsPath, FileMode.Open, FileAccess.Read))
                {
                    credential = GoogleCredential.FromStream(stream)
                        .CreateScoped(DriveService.Scope.DriveReadonly);
                }

                // Crear servicio de Drive
                _driveService = new DriveService(new BaseClientService.Initializer()
                {
                    HttpClientInitializer = credential,
                    ApplicationName = "HealthPredict Google Drive Sync"
                });

                _logger.LogInformation("Servicio de Google Drive inicializado correctamente");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error inicializando servicio de Google Drive");
                return false;
            }
        }

        /// <summary>
        /// Busca el archivo de credenciales en diferentes ubicaciones
        /// </summary>
        private string? FindCredentialsFile()
        {
            var possiblePaths = new[]
            {
                "credentials.json",
                Path.Combine(Directory.GetCurrentDirectory(), "credentials.json"),
                Path.Combine(Directory.GetCurrentDirectory(), "..", "credentials.json"),
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "credentials.json")
            };

            foreach (var path in possiblePaths)
            {
                if (File.Exists(path))
                {
                    _logger.LogInformation($"Archivo de credenciales encontrado en: {path}");
                    return path;
                }
            }

            return null;
        }

        /// <summary>
        /// Sincroniza los datos desde Google Drive
        /// </summary>
        public async Task<SyncResult> SyncFromGoogleDrive()
        {
            try
            {
                _logger.LogInformation($"Iniciando sincronización del archivo: {ARCHIVO_FIJO}");

                // Inicializar servicio de Drive
                if (!await InitializeDriveService())
                {
                    return new SyncResult
                    {
                        Success = false,
                        Error = "drive_service_error",
                        Message = "No se pudo inicializar el servicio de Google Drive"
                    };
                }

                // Buscar archivo
                var fileInfo = await FindFileInFolder(ARCHIVO_FIJO, CARPETA_FIJA);
                if (fileInfo == null)
                {
                    return new SyncResult
                    {
                        Success = false,
                        Error = "file_not_found",
                        Message = $"No se encontró el archivo {ARCHIVO_FIJO} en {CARPETA_FIJA}"
                    };
                }

                // Descargar contenido
                var content = await DownloadFileContent(fileInfo.Id);
                if (string.IsNullOrEmpty(content))
                {
                    return new SyncResult
                    {
                        Success = false,
                        Error = "download_error",
                        Message = "No se pudo descargar el contenido del archivo"
                    };
                }

                // Procesar datos
                var processResult = await ProcessHealthData(content);
                if (!processResult.Success)
                {
                    return processResult;
                }

                return new SyncResult
                {
                    Success = true,
                    Message = "Datos sincronizados exitosamente desde Google Drive",
                    FileInfo = new FileInfo
                    {
                        Name = fileInfo.Name,
                        Modified = fileInfo.ModifiedTime?.ToString("yyyy-MM-ddTHH:mm:ssZ") ?? "",
                        Size = fileInfo.Size?.ToString() ?? "0"
                    },
                    ProcessedRecords = processResult.ProcessedRecords
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error durante la sincronización desde Google Drive");
                return new SyncResult
                {
                    Success = false,
                    Error = "sync_error",
                    Message = $"Error durante la sincronización: {ex.Message}"
                };
            }
        }

        /// <summary>
        /// Busca un archivo en una carpeta específica
        /// </summary>
        private async Task<Google.Apis.Drive.v3.Data.File?> FindFileInFolder(string fileName, string folderPath)
        {
            try
            {
                if (_driveService == null)
                    return null;

                // Dividir la ruta en carpetas
                var folders = folderPath.Split('/').Where(f => !string.IsNullOrWhiteSpace(f)).ToList();
                
                // Empezar desde la raíz
                string currentFolderId = "root";

                // Navegar por cada carpeta
                foreach (var folderName in folders)
                {
                    var folderQuery = $"name='{folderName}' and parents in '{currentFolderId}' and mimeType='application/vnd.google-apps.folder' and trashed=false";
                    var folderRequest = _driveService.Files.List();
                    folderRequest.Q = folderQuery;
                    folderRequest.Fields = "files(id, name)";

                    var folderResults = await folderRequest.ExecuteAsync();
                    var foundFolders = folderResults.Files;

                    if (foundFolders == null || !foundFolders.Any())
                    {
                        _logger.LogWarning($"No se encontró la carpeta: {folderName}");
                        return null;
                    }

                    currentFolderId = foundFolders.First().Id;
                }

                // Buscar el archivo en la carpeta final
                var fileQuery = $"name='{fileName}' and parents in '{currentFolderId}' and trashed=false";
                var fileRequest = _driveService.Files.List();
                fileRequest.Q = fileQuery;
                fileRequest.Fields = "files(id, name, modifiedTime, size)";

                var fileResults = await fileRequest.ExecuteAsync();
                var files = fileResults.Files;

                if (files == null || !files.Any())
                {
                    _logger.LogWarning($"No se encontró el archivo: {fileName}");
                    return null;
                }

                _logger.LogInformation($"Archivo encontrado: {fileName} (ID: {files.First().Id})");
                return files.First();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error buscando archivo {fileName} en {folderPath}");
                return null;
            }
        }

        /// <summary>
        /// Descarga el contenido de un archivo
        /// </summary>
        private async Task<string?> DownloadFileContent(string fileId)
        {
            try
            {
                if (_driveService == null)
                    return null;

                var request = _driveService.Files.Get(fileId);
                using var stream = new MemoryStream();
                
                await request.DownloadAsync(stream);
                stream.Position = 0;

                using var reader = new StreamReader(stream);
                var content = await reader.ReadToEndAsync();

                _logger.LogInformation($"Archivo descargado, tamaño: {content.Length} caracteres");
                return content;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error descargando archivo con ID: {fileId}");
                return null;
            }
        }

        /// <summary>
        /// Procesa los datos de salud descargados
        /// </summary>
        private async Task<SyncResult> ProcessHealthData(string jsonContent)
        {
            try
            {
                _logger.LogInformation("Procesando datos de salud");

                var jsonDoc = JsonDocument.Parse(jsonContent);
                var root = jsonDoc.RootElement;

                int pasosGuardados = 0;

                // Buscar métricas en data.metrics
                if (root.TryGetProperty("data", out var dataElement) && 
                    dataElement.TryGetProperty("metrics", out var metricsElement))
                {
                    foreach (var metric in metricsElement.EnumerateArray())
                    {
                        // Solo procesar step_count
                        var nombreMetrica = metric.TryGetProperty("name", out var nameElement) ? nameElement.GetString() : "";
                        
                        if (nombreMetrica == "step_count" && metric.TryGetProperty("data", out var dataPoints))
                        {
                            foreach (var point in dataPoints.EnumerateArray())
                            {
                                // Extraer qty, date y source
                                var pasos = point.TryGetProperty("qty", out var qtyElement) ? qtyElement.GetDouble() : 0;
                                var fechaStr = point.TryGetProperty("date", out var dateElement) ? dateElement.GetString() : "";
                                var fuente = point.TryGetProperty("source", out var sourceElement) ? sourceElement.GetString() : "iPhone";
                                
                                // Convertir fecha
                                DateTime fechaMedicion = DateTime.UtcNow;
                                if (!string.IsNullOrEmpty(fechaStr))
                                {
                                    if (DateTime.TryParse(fechaStr, out var parsedDate))
                                    {
                                        fechaMedicion = parsedDate;
                                    }
                                }
                                
                                var datoVital = new DatoVital
                                {
                                    UsuarioId = USUARIO_ID,
                                    TipoDato = "Pasos",
                                    Valor = (decimal)pasos,
                                    Unidad = "pasos",
                                    FechaMedicion = fechaMedicion,
                                    FechaRegistro = DateTime.UtcNow,
                                    Fuente = $"Google Drive - {fuente}",
                                    Dispositivo = fuente
                                };
                                
                                _context.DatosVitales.Add(datoVital);
                                pasosGuardados++;
                            }
                        }
                    }
                }

                await _context.SaveChangesAsync();

                _logger.LogInformation($"Datos procesados exitosamente: {pasosGuardados} registros de pasos");

                return new SyncResult
                {
                    Success = true,
                    Message = $"Datos procesados exitosamente: {pasosGuardados} registros de pasos",
                    ProcessedRecords = pasosGuardados
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error procesando datos de salud");
                return new SyncResult
                {
                    Success = false,
                    Error = "processing_error",
                    Message = $"Error procesando datos: {ex.Message}"
                };
            }
        }
    }

    /// <summary>
    /// Resultado de la sincronización
    /// </summary>
    public class SyncResult
    {
        public bool Success { get; set; }
        public string Message { get; set; } = "";
        public string? Error { get; set; }
        public FileInfo? FileInfo { get; set; }
        public int ProcessedRecords { get; set; }
    }

    /// <summary>
    /// Información del archivo
    /// </summary>
    public class FileInfo
    {
        public string Name { get; set; } = "";
        public string Modified { get; set; } = "";
        public string Size { get; set; } = "";
    }
} 