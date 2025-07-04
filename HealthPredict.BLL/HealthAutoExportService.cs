using HealthPredict.Models;
using HealthPredict.Models.HealthAutoExport;
using HealthPredict.DAL;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace HealthPredict.BLL
{
    public class HealthAutoExportService
    {
        private readonly HealthPredictContext _context;
        private readonly ILogger<HealthAutoExportService> _logger;

        public HealthAutoExportService(HealthPredictContext context, ILogger<HealthAutoExportService> logger)
        {
            _context = context;
            _logger = logger;
        }

        // Procesar datos individuales de Health Auto Export
        public async Task<HealthAutoExportResponse> ProcessHealthDataAsync(HealthAutoExportData data, int usuarioId = 7)
        {
            var response = new HealthAutoExportResponse
            {
                ProcessedAt = DateTime.UtcNow
            };

            try
            {
                _logger.LogInformation($"Procesando datos de tipo {data.DataType} para usuario {usuarioId}");

                var datoVital = await ConvertToHealthPredictFormatAsync(data, usuarioId);
                
                if (datoVital != null)
                {
                    // Verificar si ya existe un registro similar
                    var existingRecord = await _context.DatosVitales
                        .FirstOrDefaultAsync(d => 
                            d.UsuarioId == usuarioId &&
                            d.TipoDato == datoVital.TipoDato &&
                            d.FechaMedicion == datoVital.FechaMedicion &&
                            Math.Abs(d.Valor - datoVital.Valor) < 0.01m);

                    if (existingRecord == null)
                    {
                        _context.DatosVitales.Add(datoVital);
                        await _context.SaveChangesAsync();
                        
                        response.ProcessedRecords = 1;
                        response.Success = true;
                        response.Message = $"Datos de {data.DataType} procesados exitosamente";
                        
                        _logger.LogInformation($"Datos guardados: {data.DataType} = {data.Value} {data.Unit}");
                    }
                    else
                    {
                        response.SkippedRecords = 1;
                        response.Success = true;
                        response.Message = $"Datos de {data.DataType} ya existían, omitidos";
                        
                        _logger.LogInformation($"Datos duplicados omitidos: {data.DataType}");
                    }
                }
                else
                {
                    response.SkippedRecords = 1;
                    response.Success = true;
                    response.Message = $"Tipo de dato {data.DataType} no soportado";
                    
                    _logger.LogWarning($"Tipo de dato no soportado: {data.DataType}");
                }
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = $"Error procesando datos: {ex.Message}";
                response.Errors.Add(ex.Message);
                
                _logger.LogError(ex, $"Error procesando datos de {data.DataType}");
            }

            return response;
        }

        // Procesar lote de datos de Health Auto Export
        public async Task<HealthAutoExportResponse> ProcessHealthDataBatchAsync(HealthAutoExportBatch batch, int usuarioId = 7)
        {
            var response = new HealthAutoExportResponse
            {
                ProcessedAt = DateTime.UtcNow
            };

            try
            {
                _logger.LogInformation($"Procesando lote de {batch.Data.Count} registros para usuario {usuarioId}");

                foreach (var data in batch.Data)
                {
                    var individualResponse = await ProcessHealthDataAsync(data, usuarioId);
                    
                    response.ProcessedRecords += individualResponse.ProcessedRecords;
                    response.SkippedRecords += individualResponse.SkippedRecords;
                    response.Errors.AddRange(individualResponse.Errors);
                }

                response.Success = response.Errors.Count == 0;
                response.Message = $"Procesados {response.ProcessedRecords} registros, omitidos {response.SkippedRecords}";

                // Actualizar estadísticas de sincronización
                await UpdateSyncStatsAsync(usuarioId, response);
                
                _logger.LogInformation($"Lote procesado: {response.ProcessedRecords} exitosos, {response.SkippedRecords} omitidos");
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = $"Error procesando lote: {ex.Message}";
                response.Errors.Add(ex.Message);
                
                _logger.LogError(ex, "Error procesando lote de datos");
            }

            return response;
        }

        // Convertir datos de Health Auto Export al formato de HealthPredict
        private async Task<DatoVital?> ConvertToHealthPredictFormatAsync(HealthAutoExportData data, int usuarioId)
        {
            var datoVital = new DatoVital
            {
                UsuarioId = usuarioId,
                FechaMedicion = data.StartDate,
                Valor = (decimal)data.Value,
                Unidad = data.Unit,
                Fuente = $"Health Auto Export - {data.Source}",
                Dispositivo = $"{data.DeviceManufacturer} {data.DeviceModel}".Trim(),
                FechaRegistro = DateTime.UtcNow,
                Notas = JsonSerializer.Serialize(data.Metadata)
            };

            // Mapear tipos de datos de Health Auto Export a HealthPredict
            switch (data.DataType.ToLower())
            {
                case "stepcount":
                case "steps":
                    datoVital.TipoDato = "Pasos";
                    datoVital.Valor = (decimal)data.Value;
                    break;

                case "heartrate":
                case "heart_rate":
                    datoVital.TipoDato = "Frecuencia Cardíaca";
                    datoVital.Valor = (decimal)data.Value;
                    datoVital.Unidad = "bpm";
                    break;

                case "bloodpressuresystolic":
                case "systolic_blood_pressure":
                    datoVital.TipoDato = "Presión Arterial Sistólica";
                    datoVital.Valor = (decimal)data.Value;
                    datoVital.Unidad = "mmHg";
                    break;

                case "bloodpressurediastolic":
                case "diastolic_blood_pressure":
                    datoVital.TipoDato = "Presión Arterial Diastólica";
                    datoVital.Valor = (decimal)data.Value;
                    datoVital.Unidad = "mmHg";
                    break;

                case "bloodglucose":
                case "blood_glucose":
                    datoVital.TipoDato = "Glucosa";
                    datoVital.Valor = (decimal)data.Value;
                    datoVital.Unidad = data.Unit.Contains("mg") ? "mg/dL" : "mmol/L";
                    break;

                case "bodyweight":
                case "weight":
                    datoVital.TipoDato = "Peso";
                    datoVital.Valor = (decimal)data.Value;
                    datoVital.Unidad = data.Unit.Contains("lb") ? "lb" : "kg";
                    break;

                case "bodytemperature":
                case "temperature":
                    datoVital.TipoDato = "Temperatura Corporal";
                    datoVital.Valor = (decimal)data.Value;
                    datoVital.Unidad = data.Unit.Contains("F") ? "°F" : "°C";
                    break;

                case "oxygensaturation":
                case "oxygen_saturation":
                    datoVital.TipoDato = "Saturación de Oxígeno";
                    datoVital.Valor = (decimal)data.Value;
                    datoVital.Unidad = "%";
                    break;

                case "sleepanalysis":
                case "sleep":
                    datoVital.TipoDato = "Sueño";
                    datoVital.Valor = (decimal)data.Value; // Minutos de sueño
                    datoVital.Unidad = "min";
                    break;

                case "activeenergyburned":
                case "calories":
                    datoVital.TipoDato = "Calorías Activas";
                    datoVital.Valor = (decimal)data.Value;
                    datoVital.Unidad = "kcal";
                    break;

                case "distancewalking":
                case "distance":
                    datoVital.TipoDato = "Distancia Caminada";
                    datoVital.Valor = (decimal)data.Value;
                    datoVital.Unidad = data.Unit.Contains("mi") ? "mi" : "km";
                    break;

                case "vo2max":
                    datoVital.TipoDato = "VO2 Max";
                    datoVital.Valor = (decimal)data.Value;
                    datoVital.Unidad = "mL/kg/min";
                    break;

                case "restingheartrate":
                    datoVital.TipoDato = "Frecuencia Cardíaca en Reposo";
                    datoVital.Valor = (decimal)data.Value;
                    datoVital.Unidad = "bpm";
                    break;

                case "walkingheartrateaverage":
                    datoVital.TipoDato = "Frecuencia Cardíaca Promedio Caminando";
                    datoVital.Valor = (decimal)data.Value;
                    datoVital.Unidad = "bpm";
                    break;

                case "respiratoryrate":
                    datoVital.TipoDato = "Frecuencia Respiratoria";
                    datoVital.Valor = (decimal)data.Value;
                    datoVital.Unidad = "rpm";
                    break;

                default:
                    _logger.LogWarning($"Tipo de dato no reconocido: {data.DataType}");
                    return null;
            }

            return datoVital;
        }

        // Actualizar estadísticas de sincronización
        private async Task UpdateSyncStatsAsync(int usuarioId, HealthAutoExportResponse response)
        {
            try
            {
                var config = await _context.HealthAutoExportConfigs
                    .FirstOrDefaultAsync(c => c.UsuarioId == usuarioId);

                if (config != null)
                {
                    config.LastSyncAt = DateTime.UtcNow;
                    await _context.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error actualizando estadísticas de sincronización");
            }
        }

        // Obtener configuración de Health Auto Export para un usuario
        public async Task<HealthAutoExportConfig?> GetConfigAsync(int usuarioId)
        {
            return await _context.HealthAutoExportConfigs
                .FirstOrDefaultAsync(c => c.UsuarioId == usuarioId);
        }

        // Crear configuración de Health Auto Export para un usuario
        public async Task<HealthAutoExportConfig> CreateConfigAsync(int usuarioId, string apiKey)
        {
            var config = new HealthAutoExportConfig
            {
                UsuarioId = usuarioId,
                ApiKey = apiKey,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                AllowedDataTypes = JsonSerializer.Serialize(new[]
                {
                    "stepcount", "heartrate", "bloodpressuresystolic", "bloodpressurediastolic",
                    "bloodglucose", "bodyweight", "bodytemperature", "oxygensaturation",
                    "sleepanalysis", "activeenergyburned", "distancewalking", "vo2max",
                    "restingheartrate", "walkingheartrateaverage", "respiratoryrate"
                }),
                SyncIntervalMinutes = 60
            };

            _context.HealthAutoExportConfigs.Add(config);
            await _context.SaveChangesAsync();

            return config;
        }

        // Obtener estadísticas de sincronización
        public async Task<SyncStats> GetSyncStatsAsync(int usuarioId)
        {
            var config = await GetConfigAsync(usuarioId);
            var totalRecords = await _context.DatosVitales
                .CountAsync(d => d.UsuarioId == usuarioId && d.Fuente.Contains("Health Auto Export"));

            var dataTypeBreakdown = await _context.DatosVitales
                .Where(d => d.UsuarioId == usuarioId && d.Fuente.Contains("Health Auto Export"))
                .GroupBy(d => d.TipoDato)
                .Select(g => new { TipoDato = g.Key, Count = g.Count() })
                .ToDictionaryAsync(x => x.TipoDato, x => x.Count);

            return new SyncStats
            {
                TotalRecords = totalRecords,
                ProcessedRecords = totalRecords,
                SkippedRecords = 0,
                ErrorRecords = 0,
                LastSync = config?.LastSyncAt ?? DateTime.MinValue,
                DataTypeBreakdown = dataTypeBreakdown
            };
        }

        // Validar API Key
        public async Task<bool> ValidateApiKeyAsync(string apiKey, int usuarioId)
        {
            var config = await _context.HealthAutoExportConfigs
                .FirstOrDefaultAsync(c => c.UsuarioId == usuarioId && c.ApiKey == apiKey && c.IsActive);

            return config != null;
        }

        // Generar nueva API Key
        public string GenerateApiKey()
        {
            return Guid.NewGuid().ToString("N")[..32].ToUpper();
        }
    }
} 