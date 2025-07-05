using Microsoft.AspNetCore.Mvc;
using HealthPredict.Models.HealthAutoExport;
using HealthPredict.BLL;
using HealthPredict.DAL;
using HealthPredict.Models;
using Microsoft.AspNetCore.Authorization;
using System.Text.Json;
using HealthPredict.API.Services;
using System.Diagnostics;
using System.IO;

namespace HealthPredict.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class HealthAutoExportController : ControllerBase
    {
        private readonly HealthAutoExportService _healthAutoExportService;
        private readonly ILogger<HealthAutoExportController> _logger;
        private readonly HealthPredictContext _context;
        private readonly GoogleDriveService _googleDriveService;

        public HealthAutoExportController(
            HealthAutoExportService healthAutoExportService,
            ILogger<HealthAutoExportController> logger,
            HealthPredictContext context,
            GoogleDriveService googleDriveService)
        {
            _healthAutoExportService = healthAutoExportService;
            _logger = logger;
            _context = context;
            _googleDriveService = googleDriveService;
        }

        /// <summary>
        /// Endpoint principal para recibir datos de Health Auto Export
        /// </summary>
        /// <param name="data">Datos de salud de Health Auto Export</param>
        /// <param name="apiKey">API Key para autenticación</param>
        /// <returns>Respuesta del procesamiento</returns>
        [HttpPost("data")]
        public async Task<ActionResult<HealthAutoExportResponse>> ReceiveHealthData(
            [FromBody] HealthAutoExportData data,
            [FromHeader(Name = "X-API-Key")] string? apiKey = null)
        {
            try
            {
                // Usar usuario 7 por defecto como solicitado
                const int usuarioId = 7;

                _logger.LogInformation($"Recibiendo datos de Health Auto Export: {data.DataType}");

                // Validar API Key si se proporciona
                if (!string.IsNullOrEmpty(apiKey))
                {
                    var isValidKey = await _healthAutoExportService.ValidateApiKeyAsync(apiKey, usuarioId);
                    if (!isValidKey)
                    {
                        return Unauthorized(new HealthAutoExportResponse
                        {
                            Success = false,
                            Message = "API Key inválida",
                            ProcessedAt = DateTime.UtcNow
                        });
                    }
                }

                var response = await _healthAutoExportService.ProcessHealthDataAsync(data, usuarioId);
                
                if (response.Success)
                {
                    return Ok(response);
                }
                else
                {
                    return BadRequest(response);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error procesando datos de Health Auto Export");
                return StatusCode(500, new HealthAutoExportResponse
                {
                    Success = false,
                    Message = $"Error interno del servidor: {ex.Message}",
                    ProcessedAt = DateTime.UtcNow
                });
            }
        }

        /// <summary>
        /// Endpoint para recibir lotes de datos de Health Auto Export
        /// </summary>
        /// <param name="batch">Lote de datos de salud</param>
        /// <param name="apiKey">API Key para autenticación</param>
        /// <returns>Respuesta del procesamiento del lote</returns>
        [HttpPost("batch")]
        public async Task<ActionResult<HealthAutoExportResponse>> ReceiveHealthDataBatch(
            [FromBody] HealthAutoExportBatch batch,
            [FromHeader(Name = "X-API-Key")] string? apiKey = null)
        {
            try
            {
                // Usar usuario 7 por defecto como solicitado
                const int usuarioId = 7;

                _logger.LogInformation($"Recibiendo lote de {batch.Data.Count} registros de Health Auto Export");

                // Validar API Key si se proporciona
                if (!string.IsNullOrEmpty(apiKey))
                {
                    var isValidKey = await _healthAutoExportService.ValidateApiKeyAsync(apiKey, usuarioId);
                    if (!isValidKey)
                    {
                        return Unauthorized(new HealthAutoExportResponse
                        {
                            Success = false,
                            Message = "API Key inválida",
                            ProcessedAt = DateTime.UtcNow
                        });
                    }
                }

                var response = await _healthAutoExportService.ProcessHealthDataBatchAsync(batch, usuarioId);
                
                if (response.Success)
                {
                    return Ok(response);
                }
                else
                {
                    return BadRequest(response);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error procesando lote de datos de Health Auto Export");
                return StatusCode(500, new HealthAutoExportResponse
                {
                    Success = false,
                    Message = $"Error interno del servidor: {ex.Message}",
                    ProcessedAt = DateTime.UtcNow
                });
            }
        }

        /// <summary>
        /// Endpoint simplificado para recibir datos individuales (sin autenticación)
        /// </summary>
        /// <param name="data">Datos de salud</param>
        /// <returns>Respuesta del procesamiento</returns>
        [HttpPost("simple")]
        public async Task<ActionResult<HealthAutoExportResponse>> ReceiveHealthDataSimple(
            [FromBody] HealthAutoExportData data)
        {
            try
            {
                // Usar usuario 7 por defecto como solicitado
                const int usuarioId = 7;

                _logger.LogInformation($"Recibiendo datos simples de Health Auto Export: {data.DataType}");

                var response = await _healthAutoExportService.ProcessHealthDataAsync(data, usuarioId);
                
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error procesando datos simples de Health Auto Export");
                return StatusCode(500, new HealthAutoExportResponse
                {
                    Success = false,
                    Message = $"Error interno del servidor: {ex.Message}",
                    ProcessedAt = DateTime.UtcNow
                });
            }
        }

        /// <summary>
        /// Generar una nueva API Key para el usuario
        /// </summary>
        /// <returns>Nueva API Key</returns>
        [HttpPost("generate-api-key")]
        public async Task<ActionResult<object>> GenerateApiKey()
        {
            try
            {
                // Usar usuario 7 por defecto como solicitado
                const int usuarioId = 7;

                var apiKey = _healthAutoExportService.GenerateApiKey();
                var config = await _healthAutoExportService.CreateConfigAsync(usuarioId, apiKey);

                return Ok(new
                {
                    success = true,
                    apiKey = apiKey,
                    message = "API Key generada exitosamente",
                    config = new
                    {
                        config.Id,
                        config.UsuarioId,
                        config.IsActive,
                        config.CreatedAt,
                        config.SyncIntervalMinutes
                    }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generando API Key");
                return StatusCode(500, new
                {
                    success = false,
                    message = $"Error generando API Key: {ex.Message}"
                });
            }
        }

        /// <summary>
        /// Obtener estadísticas de sincronización
        /// </summary>
        /// <returns>Estadísticas de sincronización</returns>
        [HttpGet("stats")]
        public async Task<ActionResult<SyncStats>> GetSyncStats()
        {
            try
            {
                // Usar usuario 7 por defecto como solicitado
                const int usuarioId = 7;

                var stats = await _healthAutoExportService.GetSyncStatsAsync(usuarioId);
                return Ok(stats);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error obteniendo estadísticas");
                return StatusCode(500, new
                {
                    success = false,
                    message = $"Error obteniendo estadísticas: {ex.Message}"
                });
            }
        }

        /// <summary>
        /// Obtener configuración actual
        /// </summary>
        /// <returns>Configuración de Health Auto Export</returns>
        [HttpGet("config")]
        public async Task<ActionResult<HealthAutoExportConfig>> GetConfig()
        {
            try
            {
                // Usar usuario 7 por defecto como solicitado
                const int usuarioId = 7;

                var config = await _healthAutoExportService.GetConfigAsync(usuarioId);
                
                if (config == null)
                {
                    return NotFound(new
                    {
                        success = false,
                        message = "No se encontró configuración para el usuario"
                    });
                }

                // Ocultar la API Key por seguridad
                config.ApiKey = "***HIDDEN***";
                
                return Ok(config);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error obteniendo configuración");
                return StatusCode(500, new
                {
                    success = false,
                    message = $"Error obteniendo configuración: {ex.Message}"
                });
            }
        }

        /// <summary>
        /// Endpoint de prueba para verificar conectividad
        /// </summary>
        /// <returns>Respuesta de prueba</returns>
        [HttpGet("test")]
        public ActionResult<object> TestConnection()
        {
            return Ok(new
            {
                success = true,
                message = "Conexión exitosa con Health Auto Export API",
                timestamp = DateTime.UtcNow,
                version = "1.0.0",
                supportedDataTypes = new[]
                {
                    "stepcount", "heartrate", "bloodpressuresystolic", "bloodpressurediastolic",
                    "bloodglucose", "bodyweight", "bodytemperature", "oxygensaturation",
                    "sleepanalysis", "activeenergyburned", "distancewalking", "vo2max",
                    "restingheartrate", "walkingheartrateaverage", "respiratoryrate"
                }
            });
        }

        /// <summary>
        /// Endpoint principal para recibir datos de Health Auto Export en formato estándar
        /// </summary>
        /// <param name="payload">Payload con datos de salud de Health Auto Export</param>
        /// <param name="apiKey">API Key para autenticación</param>
        /// <returns>Respuesta del procesamiento</returns>
        [HttpPost("health-data")]
        public async Task<ActionResult<HealthAutoExportResponse>> ReceiveHealthAutoExportData(
            [FromBody] HealthAutoExportPayload payload,
            [FromHeader(Name = "X-API-Key")] string? apiKey = null)
        {
            try
            {
                const int usuarioId = 7;

                // Validar que el payload no sea null
                if (payload == null)
                {
                    return BadRequest(new HealthAutoExportResponse
                    {
                        Success = false,
                        Message = "El payload es requerido",
                        ProcessedAt = DateTime.UtcNow
                    });
                }

                // Validar que tenga datos
                if (payload.Data == null)
                {
                    return BadRequest(new HealthAutoExportResponse
                    {
                        Success = false,
                        Message = "El campo 'data' es requerido en el payload",
                        ProcessedAt = DateTime.UtcNow
                    });
                }

                _logger.LogInformation($"Recibiendo datos de Health Auto Export: {payload.Data.Metrics?.Count ?? 0} métricas, {payload.Data.Workouts?.Count ?? 0} entrenamientos");

                // Validar API Key si se proporciona
                if (!string.IsNullOrEmpty(apiKey))
                {
                    var isValidKey = await _healthAutoExportService.ValidateApiKeyAsync(apiKey, usuarioId);
                    if (!isValidKey)
                    {
                        return Unauthorized(new HealthAutoExportResponse
                        {
                            Success = false,
                            Message = "API Key inválida",
                            ProcessedAt = DateTime.UtcNow
                        });
                    }
                }

                var response = await _healthAutoExportService.ProcessHealthAutoExportDataAsync(payload, usuarioId);
                
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error procesando datos de Health Auto Export");
                return StatusCode(500, new HealthAutoExportResponse
                {
                    Success = false,
                    Message = $"Error interno del servidor: {ex.Message}",
                    ProcessedAt = DateTime.UtcNow
                });
            }
        }

        /// <summary>
        /// Endpoint para recibir datos en formato JSON genérico (compatible con Swagger)
        /// </summary>
        /// <param name="jsonData">Datos en formato JSON</param>
        /// <returns>Respuesta del procesamiento</returns>
        [HttpPost("json")]
        public async Task<ActionResult<HealthAutoExportResponse>> ReceiveJsonData(
            [FromBody] object jsonData)
        {
            try
            {
                // Usar usuario 7 por defecto como solicitado
                const int usuarioId = 7;

                _logger.LogInformation("Recibiendo datos JSON de Health Auto Export");

                var jsonString = System.Text.Json.JsonSerializer.Serialize(jsonData);
                _logger.LogInformation($"JSON recibido: {jsonString}");

                // Intentar deserializar como HealthAutoExportPayload primero
                try
                {
                    var payload = System.Text.Json.JsonSerializer.Deserialize<HealthAutoExportPayload>(jsonString);
                    if (payload?.Data != null)
                    {
                        var response = await _healthAutoExportService.ProcessHealthAutoExportDataAsync(payload, usuarioId);
                        return Ok(response);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogInformation($"No se pudo deserializar como HealthAutoExportPayload: {ex.Message}");
                }

                // Si no funciona, intentar como HealthAutoExportData
                try
                {
                    var data = System.Text.Json.JsonSerializer.Deserialize<HealthAutoExportData>(jsonString);
                    if (data != null)
                    {
                        var response = await _healthAutoExportService.ProcessHealthDataAsync(data, usuarioId);
                        return Ok(response);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogInformation($"No se pudo deserializar como HealthAutoExportData: {ex.Message}");
                }

                return BadRequest(new HealthAutoExportResponse
                {
                    Success = false,
                    Message = "No se pudo procesar los datos JSON. Formato no reconocido.",
                    ProcessedAt = DateTime.UtcNow
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error procesando datos JSON");
                return StatusCode(500, new HealthAutoExportResponse
                {
                    Success = false,
                    Message = $"Error procesando datos JSON: {ex.Message}",
                    ProcessedAt = DateTime.UtcNow
                });
            }
        }

        /// <summary>
        /// Endpoint simplificado para Swagger - recibe JSON directo en formato Health Auto Export
        /// Formato esperado: { "data": { "metrics": [...], "workouts": [...] } }
        /// </summary>
        /// <param name="healthData">Datos de Health Auto Export en formato JSON estándar</param>
        /// <returns>Respuesta del procesamiento</returns>
        [HttpPost("swagger")]
        public async Task<ActionResult<HealthAutoExportResponse>> ReceiveSwaggerData(
            [FromBody] HealthAutoExportPayload healthData)
        {
            try
            {
                const int usuarioId = 7;

                _logger.LogInformation("Recibiendo datos desde Swagger");

                // Validar que los datos no sean null
                if (healthData?.Data == null)
                {
                    return BadRequest(new HealthAutoExportResponse
                    {
                        Success = false,
                        Message = "Se requiere un objeto con estructura { \"data\": { \"metrics\": [], \"workouts\": [] } }",
                        ProcessedAt = DateTime.UtcNow
                    });
                }

                _logger.LogInformation($"Procesando {healthData.Data.Metrics?.Count ?? 0} métricas y {healthData.Data.Workouts?.Count ?? 0} entrenamientos");

                var response = await _healthAutoExportService.ProcessHealthAutoExportDataAsync(healthData, usuarioId);
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error procesando datos desde Swagger");
                return StatusCode(500, new HealthAutoExportResponse
                {
                    Success = false,
                    Message = $"Error interno del servidor: {ex.Message}",
                    ProcessedAt = DateTime.UtcNow
                });
            }
        }

        /// <summary>
        /// Endpoint para formato Health Auto Export real - acepta cualquier estructura JSON
        /// </summary>
        /// <param name="rawData">Datos en formato JSON tal como los exporta Health Auto Export</param>
        /// <returns>Respuesta del procesamiento</returns>
        [HttpPost("raw")]
        public async Task<ActionResult<HealthAutoExportResponse>> ReceiveRawHealthData(
            [FromBody] object rawData)
        {
            try
            {
                const int usuarioId = 7;

                _logger.LogInformation("Recibiendo datos raw de Health Auto Export");

                var jsonString = System.Text.Json.JsonSerializer.Serialize(rawData);
                _logger.LogInformation($"JSON recibido (primeros 500 chars): {jsonString.Substring(0, Math.Min(500, jsonString.Length))}...");

                // Intentar convertir al formato esperado
                var convertedPayload = ConvertRawDataToPayload(rawData);
                
                if (convertedPayload?.Data == null)
                {
                    return BadRequest(new HealthAutoExportResponse
                    {
                        Success = false,
                        Message = "No se pudo convertir el JSON al formato esperado. Verifique la estructura de datos.",
                        ProcessedAt = DateTime.UtcNow
                    });
                }

                var response = await _healthAutoExportService.ProcessHealthAutoExportDataAsync(convertedPayload, usuarioId);
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error procesando datos raw");
                return StatusCode(500, new HealthAutoExportResponse
                {
                    Success = false,
                    Message = $"Error interno del servidor: {ex.Message}",
                    ProcessedAt = DateTime.UtcNow
                });
            }
        }

        private HealthAutoExportPayload? ConvertRawDataToPayload(object rawData)
        {
            try
            {
                var jsonString = System.Text.Json.JsonSerializer.Serialize(rawData);
                var jsonDoc = System.Text.Json.JsonDocument.Parse(jsonString);
                var root = jsonDoc.RootElement;

                // Si ya tiene el formato correcto
                if (root.TryGetProperty("data", out var dataElement))
                {
                    return System.Text.Json.JsonSerializer.Deserialize<HealthAutoExportPayload>(jsonString);
                }

                // Si tiene métricas y entrenamientos directamente en la raíz
                var payload = new HealthAutoExportPayload
                {
                    Data = new HealthAutoExportPayloadData()
                };

                if (root.TryGetProperty("metrics", out var metricsElement))
                {
                    payload.Data.Metrics = System.Text.Json.JsonSerializer.Deserialize<List<HealthMetric>>(metricsElement.GetRawText()) ?? new List<HealthMetric>();
                }

                if (root.TryGetProperty("workouts", out var workoutsElement))
                {
                    payload.Data.Workouts = System.Text.Json.JsonSerializer.Deserialize<List<WorkoutMetric>>(workoutsElement.GetRawText()) ?? new List<WorkoutMetric>();
                }

                return payload;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error convirtiendo datos raw a payload");
                return null;
            }
        }

        /// <summary>
        /// Endpoint SIMPLE - acepta cualquier JSON y guarda en datos vitales
        /// </summary>
        /// <param name="jsonData">Cualquier JSON de Health Auto Export</param>
        /// <returns>Respuesta simple</returns>
        [HttpPost("guardar")]
        public async Task<ActionResult> GuardarDatosSimple([FromBody] object jsonData)
        {
            try
            {
                const int usuarioId = 7;
                var jsonString = System.Text.Json.JsonSerializer.Serialize(jsonData);
                
                _logger.LogInformation($"Recibiendo JSON simple para usuario {usuarioId}");
                
                // Parsear el JSON para extraer métricas
                var jsonDoc = System.Text.Json.JsonDocument.Parse(jsonString);
                var root = jsonDoc.RootElement;
                
                int registrosGuardados = 0;
                
                // Buscar métricas en data.metrics
                if (root.TryGetProperty("data", out var dataElement) && 
                    dataElement.TryGetProperty("metrics", out var metricsElement))
                {
                    foreach (var metric in metricsElement.EnumerateArray())
                    {
                        var nombreMetrica = metric.TryGetProperty("name", out var nameElement) ? nameElement.GetString() : "Unknown";
                        var unidades = metric.TryGetProperty("units", out var unitsElement) ? unitsElement.GetString() : "";
                        
                        if (metric.TryGetProperty("data", out var dataPoints))
                        {
                            foreach (var point in dataPoints.EnumerateArray())
                            {
                                var datoVital = new DatoVital
                                {
                                    UsuarioId = usuarioId,
                                    TipoDato = nombreMetrica ?? "Unknown",
                                    Valor = (decimal)ExtractValue(point),
                                    Unidad = unidades ?? "",
                                    FechaMedicion = ExtractDate(point),
                                    FechaRegistro = DateTime.UtcNow,
                                    Fuente = "Health Auto Export"
                                };
                                
                                _context.DatosVitales.Add(datoVital);
                                registrosGuardados++;
                            }
                        }
                    }
                }
                
                await _context.SaveChangesAsync();
                
                return Ok(new 
                {
                    success = true,
                    message = $"Datos guardados exitosamente",
                    registrosGuardados = registrosGuardados,
                    usuarioId = usuarioId
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error guardando datos simples");
                return StatusCode(500, new 
                {
                    success = false,
                    message = $"Error: {ex.Message}"
                });
            }
        }
        
        private double ExtractValue(JsonElement point)
        {
            if (point.TryGetProperty("qty", out var qty) && qty.TryGetDouble(out var qtyValue))
                return qtyValue;
            if (point.TryGetProperty("avg", out var avg) && avg.TryGetDouble(out var avgValue))
                return avgValue;
            if (point.TryGetProperty("max", out var max) && max.TryGetDouble(out var maxValue))
                return maxValue;
            if (point.TryGetProperty("min", out var min) && min.TryGetDouble(out var minValue))
                return minValue;
            if (point.TryGetProperty("systolic", out var sys) && sys.TryGetDouble(out var sysValue))
                return sysValue;
            
            return 0.0;
        }
        
        private DateTime ExtractDate(JsonElement point)
        {
            if (point.TryGetProperty("date", out var date) && date.TryGetDateTime(out var dateValue))
                return dateValue;
            
            return DateTime.UtcNow;
        }

        /// <summary>
        /// Endpoint para ejecutar la sincronización desde Google Drive
        /// </summary>
        /// <returns>Resultado de la sincronización</returns>
        [HttpPost("sync-google-drive")]
        public async Task<ActionResult<object>> SyncFromGoogleDrive()
        {
            try
            {
                _logger.LogInformation("Iniciando sincronización desde Google Drive");

                // Usar el servicio real de Google Drive
                var result = await _googleDriveService.SyncFromGoogleDrive();
                
                if (result.Success)
                {
                    _logger.LogInformation("Sincronización completada exitosamente");
                    return Ok(new
                    {
                        success = result.Success,
                        message = result.Message,
                        file_info = result.FileInfo != null ? new
                        {
                            name = result.FileInfo.Name,
                            modified = result.FileInfo.Modified,
                            size = result.FileInfo.Size
                        } : null,
                        processed_records = result.ProcessedRecords
                    });
                }
                else
                {
                    _logger.LogWarning($"Error en sincronización: {result.Message}");
                    return BadRequest(new
                    {
                        success = result.Success,
                        error = result.Error,
                        message = result.Message
                    });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error ejecutando sincronización desde Google Drive");
                return StatusCode(500, new
                {
                    success = false,
                    error = "sync_error",
                    message = $"Error interno: {ex.Message}"
                });
            }
        }

        /// <summary>
        /// Endpoint para guardar datos de pasos (step_count)
        /// </summary>
        /// <param name="jsonData">JSON con datos de pasos en formato Health Auto Export</param>
        /// <returns>Respuesta simple</returns>
        [HttpPost("pasos")]
        public async Task<ActionResult> GuardarPasos([FromBody] object jsonData)
        {
            try
            {
                const int usuarioId = 7;
                var jsonString = System.Text.Json.JsonSerializer.Serialize(jsonData);

                _logger.LogInformation($"Recibiendo datos de pasos para usuario {usuarioId}");

                var jsonDoc = System.Text.Json.JsonDocument.Parse(jsonString);
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
                                    UsuarioId = usuarioId,
                                    TipoDato = "Pasos",
                                    Valor = (decimal)pasos,
                                    Unidad = "pasos",
                                    FechaMedicion = fechaMedicion,
                                    FechaRegistro = DateTime.UtcNow,
                                    Fuente = $"Health Auto Export - {fuente}",
                                    Dispositivo = fuente
                                };

                                _context.DatosVitales.Add(datoVital);
                                pasosGuardados++;
                            }
                        }
                    }
                }

                await _context.SaveChangesAsync();

                return Ok(new
                {
                    success = true,
                    message = $"Datos de pasos guardados exitosamente",
                    pasosGuardados = pasosGuardados,
                    usuarioId = usuarioId,
                    tipoMetrica = "Pasos"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error guardando datos de pasos");
                return StatusCode(500, new
                {
                    success = false,
                    message = $"Error: {ex.Message}"
                });
            }
        }

        /// <summary>
        /// Obtener información de la última sincronización
        /// </summary>
        /// <returns>Información de la última sincronización</returns>
        [HttpGet("ultima-sincronizacion")]
        public async Task<ActionResult<object>> GetUltimaSincronizacion()
        {
            try
            {
                // Por simplicidad, retornamos información básica
                // En una implementación real, esto se guardaría en la base de datos
                return Ok(new
                {
                    ultimaSincronizacion = DateTime.UtcNow.AddHours(-1), // Ejemplo
                    estado = "completada",
                    archivo = "HealthAutoExport-2025-07-04.json"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error obteniendo información de última sincronización");
                return StatusCode(500, new
                {
                    success = false,
                    message = $"Error interno: {ex.Message}"
                });
            }
        }

        /// <summary>
        /// Sincronizar pasos ejecutando el script de Python
        /// </summary>
        [HttpPost("sync-pasos")]
        public async Task<ActionResult<object>> SyncPasos()
        {
            try
            {
                _logger.LogInformation("Iniciando sincronización de pasos con script Python");

                // Detectar el comando Python correcto según el sistema operativo
                string pythonCommand = GetPythonCommand();
                string scriptPath = GetScriptPath();

                _logger.LogInformation($"Usando comando Python: {pythonCommand}");
                _logger.LogInformation($"Ruta del script: {scriptPath}");

                // Ejecutar el script de Python
                var processStartInfo = new System.Diagnostics.ProcessStartInfo
                {
                    FileName = pythonCommand,
                    Arguments = $"\"{scriptPath}\"",
                    WorkingDirectory = GetWorkingDirectory(),
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };

                using var process = new System.Diagnostics.Process();
                process.StartInfo = processStartInfo;
                
                var outputBuilder = new System.Text.StringBuilder();
                var errorBuilder = new System.Text.StringBuilder();
                
                process.OutputDataReceived += (sender, e) =>
                {
                    if (e.Data != null)
                    {
                        outputBuilder.AppendLine(e.Data);
                        _logger.LogInformation($"Script output: {e.Data}");
                    }
                };
                
                process.ErrorDataReceived += (sender, e) =>
                {
                    if (e.Data != null)
                    {
                        errorBuilder.AppendLine(e.Data);
                        _logger.LogWarning($"Script error: {e.Data}");
                    }
                };

                process.Start();
                process.BeginOutputReadLine();
                process.BeginErrorReadLine();
                
                // Esperar hasta 60 segundos
                if (!process.WaitForExit(60000))
                {
                    process.Kill();
                    return StatusCode(408, new
                    {
                        success = false,
                        error = "timeout",
                        message = "La sincronización tardó demasiado tiempo"
                    });
                }

                // Asegurar que todas las salidas se han capturado
                process.WaitForExit();
                
                var output = outputBuilder.ToString().Trim();
                var error = errorBuilder.ToString().Trim();

                _logger.LogInformation($"Código de salida del script: {process.ExitCode}");
                _logger.LogInformation($"Salida del script: {output}");
                if (!string.IsNullOrEmpty(error))
                {
                    _logger.LogWarning($"Error del script: {error}");
                }

                if (process.ExitCode == 0)
                {
                    if (!string.IsNullOrEmpty(output))
                    {
                        try
                        {
                            // Intentar parsear la respuesta JSON del script
                            var result = JsonSerializer.Deserialize<object>(output);
                            _logger.LogInformation("Sincronización de pasos completada exitosamente");
                            return Ok(result);
                        }
                        catch (JsonException)
                        {
                            _logger.LogWarning("La respuesta del script no es JSON válido: {Output}", output);
                            return Ok(new
                            {
                                success = true,
                                message = "Sincronización completada",
                                output = output
                            });
                        }
                    }
                    else
                    {
                        // El script terminó exitosamente pero sin salida
                        _logger.LogInformation("Script terminó exitosamente pero sin salida");
                        return Ok(new
                        {
                            success = true,
                            message = "Sincronización completada exitosamente",
                            note = "Script ejecutado sin errores"
                        });
                    }
                }
                else
                {
                    _logger.LogError("Error en el script de Python. Exit Code: {ExitCode}, Error: {Error}", 
                        process.ExitCode, error);
                    
                    return BadRequest(new
                    {
                        success = false,
                        error = "script_error",
                        message = !string.IsNullOrEmpty(error) ? error : "Error ejecutando el script de sincronización",
                        exitCode = process.ExitCode,
                        output = output,
                        pythonCommand = pythonCommand,
                        scriptPath = scriptPath,
                        workingDirectory = GetWorkingDirectory()
                    });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error ejecutando sincronización de pasos");
                return StatusCode(500, new
                {
                    success = false,
                    error = "execution_error",
                    message = $"Error interno: {ex.Message}",
                    pythonCommand = GetPythonCommand(),
                    workingDirectory = GetWorkingDirectory()
                });
            }
        }

        /// <summary>
        /// Obtener el comando Python correcto según el sistema operativo
        /// </summary>
        private string GetPythonCommand()
        {
            // En Linux/Docker, usar python3
            if (Environment.OSVersion.Platform == PlatformID.Unix)
            {
                return "python3";
            }
            
            // En Windows, usar python
            return "python";
        }

        /// <summary>
        /// Obtener la ruta del script
        /// </summary>
        private string GetScriptPath()
        {
            var workingDir = GetWorkingDirectory();
            var scriptName = "sync_pasos_simple.py";
            
            // Buscar el script en diferentes ubicaciones
            var possiblePaths = new[]
            {
                Path.Combine(workingDir, scriptName),
                Path.Combine(workingDir, "..", scriptName),
                Path.Combine(workingDir, "..", "..", scriptName),
                Path.Combine("/app", scriptName), // Ruta típica en Docker
                scriptName // Ruta relativa
            };

            foreach (var path in possiblePaths)
            {
                if (System.IO.File.Exists(path))
                {
                    _logger.LogInformation($"Script encontrado en: {path}");
                    return path;
                }
            }

            _logger.LogWarning($"Script no encontrado en ninguna ubicación. Usando: {scriptName}");
            return scriptName;
        }

        /// <summary>
        /// Obtener el directorio de trabajo
        /// </summary>
        private string GetWorkingDirectory()
        {
            var currentDir = Directory.GetCurrentDirectory();
            _logger.LogInformation($"Directorio de trabajo actual: {currentDir}");
            return currentDir;
        }

        /// <summary>
        /// Sincronizar desde Google Drive usando el servicio C#
        /// </summary>
        [HttpPost("sync-google-drive-csharp")]
        public async Task<ActionResult<object>> SyncFromGoogleDriveCsharp()
        {
            try
            {
                _logger.LogInformation("Iniciando sincronización desde Google Drive usando el servicio C#");

                // Usar el servicio real de Google Drive
                var result = await _googleDriveService.SyncFromGoogleDrive();
                
                if (result.Success)
                {
                    _logger.LogInformation("Sincronización completada exitosamente");
                    return Ok(new
                    {
                        success = result.Success,
                        message = result.Message,
                        file_info = result.FileInfo != null ? new
                        {
                            name = result.FileInfo.Name,
                            modified = result.FileInfo.Modified,
                            size = result.FileInfo.Size
                        } : null,
                        processed_records = result.ProcessedRecords
                    });
                }
                else
                {
                    _logger.LogWarning($"Error en sincronización: {result.Message}");
                    return BadRequest(new
                    {
                        success = result.Success,
                        error = result.Error,
                        message = result.Message
                    });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error ejecutando sincronización desde Google Drive usando el servicio C#");
                return StatusCode(500, new
                {
                    success = false,
                    error = "sync_error",
                    message = $"Error interno: {ex.Message}"
                });
            }
        }
    }
} 