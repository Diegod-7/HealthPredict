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
            return "HAE" + Guid.NewGuid().ToString("N")[..20].ToUpper();
        }

        // ===== NUEVOS MÉTODOS PARA FORMATO ESTÁNDAR DE HEALTH AUTO EXPORT =====

        public async Task<HealthAutoExportResponse> ProcessHealthAutoExportDataAsync(HealthAutoExportPayload payload, int usuarioId = 7)
        {
            var response = new HealthAutoExportResponse
            {
                ProcessedAt = DateTime.UtcNow
            };

            try
            {
                _logger.LogInformation($"Procesando payload con {payload.Data.Metrics.Count} métricas y {payload.Data.Workouts.Count} entrenamientos");

                // Procesar métricas de salud
                foreach (var metric in payload.Data.Metrics)
                {
                    var metricResponse = await ProcessHealthMetricAsync(metric, usuarioId);
                    response.ProcessedRecords += metricResponse.ProcessedRecords;
                    response.SkippedRecords += metricResponse.SkippedRecords;
                    response.Errors.AddRange(metricResponse.Errors);
                }

                // Procesar entrenamientos
                foreach (var workout in payload.Data.Workouts)
                {
                    var workoutResponse = await ProcessWorkoutAsync(workout, usuarioId);
                    response.ProcessedRecords += workoutResponse.ProcessedRecords;
                    response.SkippedRecords += workoutResponse.SkippedRecords;
                    response.Errors.AddRange(workoutResponse.Errors);
                }

                response.Success = response.Errors.Count == 0;
                response.Message = $"Procesados {response.ProcessedRecords} registros, omitidos {response.SkippedRecords}";

                await UpdateSyncStatsAsync(usuarioId, response);
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = $"Error procesando payload: {ex.Message}";
                response.Errors.Add(ex.Message);
                _logger.LogError(ex, "Error procesando payload de Health Auto Export");
            }

            return response;
        }

        private async Task<HealthAutoExportResponse> ProcessHealthMetricAsync(HealthMetric metric, int usuarioId)
        {
            var response = new HealthAutoExportResponse { ProcessedAt = DateTime.UtcNow };

            try
            {
                foreach (var dataPoint in metric.Data)
                {
                    var datosVitales = ConvertMetricToHealthPredictFormat(metric, dataPoint, usuarioId);
                    
                    foreach (var datoVital in datosVitales)
                    {
                        var existingRecord = await _context.DatosVitales
                            .FirstOrDefaultAsync(d => 
                                d.UsuarioId == usuarioId &&
                                d.TipoDato == datoVital.TipoDato &&
                                d.FechaMedicion == datoVital.FechaMedicion &&
                                Math.Abs(d.Valor - datoVital.Valor) < 0.01m);

                        if (existingRecord == null)
                        {
                            _context.DatosVitales.Add(datoVital);
                            response.ProcessedRecords++;
                        }
                        else
                        {
                            response.SkippedRecords++;
                        }
                    }
                }

                await _context.SaveChangesAsync();
                response.Success = true;
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = ex.Message;
                response.Errors.Add(ex.Message);
                _logger.LogError(ex, $"Error procesando métrica {metric.Name}");
            }

            return response;
        }

        private List<DatoVital> ConvertMetricToHealthPredictFormat(HealthMetric metric, MetricDataPoint dataPoint, int usuarioId)
        {
            var datosVitales = new List<DatoVital>();

            switch (metric.Name.ToLower())
            {
                case "stepcount":
                case "steps":
                    if (dataPoint.Qty.HasValue)
                    {
                        datosVitales.Add(new DatoVital
                        {
                            UsuarioId = usuarioId,
                            TipoDato = "Pasos",
                            Valor = (decimal)dataPoint.Qty.Value,
                            Unidad = "pasos",
                            FechaMedicion = dataPoint.Date,
                            FechaRegistro = DateTime.UtcNow,
                            Fuente = "Health Auto Export",
                            Dispositivo = "iPhone/Apple Watch"
                        });
                    }
                    break;

                case "heartrate":
                    if (dataPoint.Avg.HasValue)
                    {
                        datosVitales.Add(new DatoVital
                        {
                            UsuarioId = usuarioId,
                            TipoDato = "Frecuencia Cardíaca",
                            Valor = (decimal)dataPoint.Avg.Value,
                            Unidad = "bpm",
                            FechaMedicion = dataPoint.Date,
                            FechaRegistro = DateTime.UtcNow,
                            Fuente = "Health Auto Export",
                            Dispositivo = "iPhone/Apple Watch",
                            Notas = $"Min: {dataPoint.Min}, Max: {dataPoint.Max}"
                        });
                    }
                    break;

                case "bloodpressure":
                    if (dataPoint.Systolic.HasValue)
                    {
                        datosVitales.Add(new DatoVital
                        {
                            UsuarioId = usuarioId,
                            TipoDato = "Presión Arterial Sistólica",
                            Valor = (decimal)dataPoint.Systolic.Value,
                            Unidad = "mmHg",
                            FechaMedicion = dataPoint.Date,
                            FechaRegistro = DateTime.UtcNow,
                            Fuente = "Health Auto Export",
                            Dispositivo = "iPhone/Apple Watch"
                        });
                    }
                    if (dataPoint.Diastolic.HasValue)
                    {
                        datosVitales.Add(new DatoVital
                        {
                            UsuarioId = usuarioId,
                            TipoDato = "Presión Arterial Diastólica",
                            Valor = (decimal)dataPoint.Diastolic.Value,
                            Unidad = "mmHg",
                            FechaMedicion = dataPoint.Date,
                            FechaRegistro = DateTime.UtcNow,
                            Fuente = "Health Auto Export",
                            Dispositivo = "iPhone/Apple Watch"
                        });
                    }
                    break;

                case "sleepanalysis":
                    if (dataPoint.Asleep.HasValue)
                    {
                        datosVitales.Add(new DatoVital
                        {
                            UsuarioId = usuarioId,
                            TipoDato = "Análisis de Sueño",
                            Valor = (decimal)dataPoint.Asleep.Value,
                            Unidad = "minutos",
                            FechaMedicion = dataPoint.Date,
                            FechaRegistro = DateTime.UtcNow,
                            Fuente = "Health Auto Export",
                            Dispositivo = "iPhone/Apple Watch",
                            Notas = $"Inicio: {dataPoint.SleepStart}, Fin: {dataPoint.SleepEnd}, Fuente: {dataPoint.SleepSource}"
                        });
                    }
                    break;

                case "bloodglucose":
                    if (dataPoint.Qty.HasValue)
                    {
                        datosVitales.Add(new DatoVital
                        {
                            UsuarioId = usuarioId,
                            TipoDato = "Glucosa",
                            Valor = (decimal)dataPoint.Qty.Value,
                            Unidad = metric.Units,
                            FechaMedicion = dataPoint.Date,
                            FechaRegistro = DateTime.UtcNow,
                            Fuente = "Health Auto Export",
                            Dispositivo = "iPhone/Apple Watch",
                            Notas = $"Momento de comida: {dataPoint.MealTime}"
                        });
                    }
                    break;

                case "bodyweight":
                case "weight":
                    if (dataPoint.Qty.HasValue)
                    {
                        datosVitales.Add(new DatoVital
                        {
                            UsuarioId = usuarioId,
                            TipoDato = "Peso",
                            Valor = (decimal)dataPoint.Qty.Value,
                            Unidad = metric.Units,
                            FechaMedicion = dataPoint.Date,
                            FechaRegistro = DateTime.UtcNow,
                            Fuente = "Health Auto Export",
                            Dispositivo = "iPhone/Apple Watch"
                        });
                    }
                    break;

                case "bodytemperature":
                case "temperature":
                    if (dataPoint.Qty.HasValue)
                    {
                        datosVitales.Add(new DatoVital
                        {
                            UsuarioId = usuarioId,
                            TipoDato = "Temperatura Corporal",
                            Valor = (decimal)dataPoint.Qty.Value,
                            Unidad = metric.Units,
                            FechaMedicion = dataPoint.Date,
                            FechaRegistro = DateTime.UtcNow,
                            Fuente = "Health Auto Export",
                            Dispositivo = "iPhone/Apple Watch"
                        });
                    }
                    break;

                case "oxygensaturation":
                case "oxygen_saturation":
                    if (dataPoint.Qty.HasValue)
                    {
                        datosVitales.Add(new DatoVital
                        {
                            UsuarioId = usuarioId,
                            TipoDato = "Saturación de Oxígeno",
                            Valor = (decimal)dataPoint.Qty.Value,
                            Unidad = "%",
                            FechaMedicion = dataPoint.Date,
                            FechaRegistro = DateTime.UtcNow,
                            Fuente = "Health Auto Export",
                            Dispositivo = "iPhone/Apple Watch"
                        });
                    }
                    break;

                case "activeenergyburned":
                case "active_energy":
                    if (dataPoint.Qty.HasValue)
                    {
                        datosVitales.Add(new DatoVital
                        {
                            UsuarioId = usuarioId,
                            TipoDato = "Energía Activa Quemada",
                            Valor = (decimal)dataPoint.Qty.Value,
                            Unidad = metric.Units,
                            FechaMedicion = dataPoint.Date,
                            FechaRegistro = DateTime.UtcNow,
                            Fuente = "Health Auto Export",
                            Dispositivo = "iPhone/Apple Watch"
                        });
                    }
                    break;

                case "distancewalking":
                case "distance_walking":
                    if (dataPoint.Qty.HasValue)
                    {
                        datosVitales.Add(new DatoVital
                        {
                            UsuarioId = usuarioId,
                            TipoDato = "Distancia Caminando",
                            Valor = (decimal)dataPoint.Qty.Value,
                            Unidad = metric.Units,
                            FechaMedicion = dataPoint.Date,
                            FechaRegistro = DateTime.UtcNow,
                            Fuente = "Health Auto Export",
                            Dispositivo = "iPhone/Apple Watch"
                        });
                    }
                    break;

                case "vo2max":
                    if (dataPoint.Qty.HasValue)
                    {
                        datosVitales.Add(new DatoVital
                        {
                            UsuarioId = usuarioId,
                            TipoDato = "VO2 Max",
                            Valor = (decimal)dataPoint.Qty.Value,
                            Unidad = metric.Units,
                            FechaMedicion = dataPoint.Date,
                            FechaRegistro = DateTime.UtcNow,
                            Fuente = "Health Auto Export",
                            Dispositivo = "iPhone/Apple Watch"
                        });
                    }
                    break;

                case "restingheartrate":
                case "resting_heart_rate":
                    if (dataPoint.Qty.HasValue)
                    {
                        datosVitales.Add(new DatoVital
                        {
                            UsuarioId = usuarioId,
                            TipoDato = "Frecuencia Cardíaca en Reposo",
                            Valor = (decimal)dataPoint.Qty.Value,
                            Unidad = "bpm",
                            FechaMedicion = dataPoint.Date,
                            FechaRegistro = DateTime.UtcNow,
                            Fuente = "Health Auto Export",
                            Dispositivo = "iPhone/Apple Watch"
                        });
                    }
                    break;

                case "walkingheartrateaverage":
                case "walking_heart_rate_average":
                    if (dataPoint.Qty.HasValue)
                    {
                        datosVitales.Add(new DatoVital
                        {
                            UsuarioId = usuarioId,
                            TipoDato = "Frecuencia Cardíaca Promedio Caminando",
                            Valor = (decimal)dataPoint.Qty.Value,
                            Unidad = "bpm",
                            FechaMedicion = dataPoint.Date,
                            FechaRegistro = DateTime.UtcNow,
                            Fuente = "Health Auto Export",
                            Dispositivo = "iPhone/Apple Watch"
                        });
                    }
                    break;

                case "respiratoryrate":
                case "respiratory_rate":
                    if (dataPoint.Qty.HasValue)
                    {
                        datosVitales.Add(new DatoVital
                        {
                            UsuarioId = usuarioId,
                            TipoDato = "Frecuencia Respiratoria",
                            Valor = (decimal)dataPoint.Qty.Value,
                            Unidad = metric.Units,
                            FechaMedicion = dataPoint.Date,
                            FechaRegistro = DateTime.UtcNow,
                            Fuente = "Health Auto Export",
                            Dispositivo = "iPhone/Apple Watch"
                        });
                    }
                    break;

                // Caso por defecto para tipos de datos no reconocidos
                default:
                    if (dataPoint.Qty.HasValue)
                    {
                        datosVitales.Add(new DatoVital
                        {
                            UsuarioId = usuarioId,
                            TipoDato = metric.Name,
                            Valor = (decimal)dataPoint.Qty.Value,
                            Unidad = metric.Units,
                            FechaMedicion = dataPoint.Date,
                            FechaRegistro = DateTime.UtcNow,
                            Fuente = "Health Auto Export",
                            Dispositivo = "iPhone/Apple Watch"
                        });
                    }
                    break;
            }

            return datosVitales;
        }

        private async Task<HealthAutoExportResponse> ProcessWorkoutAsync(WorkoutMetric workout, int usuarioId)
        {
            var response = new HealthAutoExportResponse { ProcessedAt = DateTime.UtcNow };

            try
            {
                // Procesar datos del entrenamiento como datos vitales
                var datosVitales = new List<DatoVital>();

                // Energía activa
                if (workout.ActiveEnergy.Qty > 0)
                {
                    datosVitales.Add(new DatoVital
                    {
                        UsuarioId = usuarioId,
                        TipoDato = "Energía Activa Quemada",
                        Valor = (decimal)workout.ActiveEnergy.Qty,
                        Unidad = workout.ActiveEnergy.Units,
                        FechaMedicion = workout.Start,
                        FechaRegistro = DateTime.UtcNow,
                        Fuente = "Health Auto Export - Entrenamiento",
                        Dispositivo = "iPhone/Apple Watch",
                        Notas = $"Entrenamiento: {workout.Name}, Duración: {(workout.End - workout.Start).TotalMinutes:F0} min"
                    });
                }

                // Energía total
                if (workout.TotalEnergy.Qty > 0)
                {
                    datosVitales.Add(new DatoVital
                    {
                        UsuarioId = usuarioId,
                        TipoDato = "Energía Total Quemada",
                        Valor = (decimal)workout.TotalEnergy.Qty,
                        Unidad = workout.TotalEnergy.Units,
                        FechaMedicion = workout.Start,
                        FechaRegistro = DateTime.UtcNow,
                        Fuente = "Health Auto Export - Entrenamiento",
                        Dispositivo = "iPhone/Apple Watch",
                        Notas = $"Entrenamiento: {workout.Name}"
                    });
                }

                // Pasos del entrenamiento
                if (workout.StepCount.Qty > 0)
                {
                    datosVitales.Add(new DatoVital
                    {
                        UsuarioId = usuarioId,
                        TipoDato = "Pasos de Entrenamiento",
                        Valor = (decimal)workout.StepCount.Qty,
                        Unidad = workout.StepCount.Units,
                        FechaMedicion = workout.Start,
                        FechaRegistro = DateTime.UtcNow,
                        Fuente = "Health Auto Export - Entrenamiento",
                        Dispositivo = "iPhone/Apple Watch",
                        Notas = $"Entrenamiento: {workout.Name}"
                    });
                }

                // Distancia
                if (workout.Distance.Qty > 0)
                {
                    datosVitales.Add(new DatoVital
                    {
                        UsuarioId = usuarioId,
                        TipoDato = "Distancia de Entrenamiento",
                        Valor = (decimal)workout.Distance.Qty,
                        Unidad = workout.Distance.Units,
                        FechaMedicion = workout.Start,
                        FechaRegistro = DateTime.UtcNow,
                        Fuente = "Health Auto Export - Entrenamiento",
                        Dispositivo = "iPhone/Apple Watch",
                        Notas = $"Entrenamiento: {workout.Name}"
                    });
                }

                // Frecuencia cardíaca promedio
                if (workout.AvgHeartRate.Qty > 0)
                {
                    datosVitales.Add(new DatoVital
                    {
                        UsuarioId = usuarioId,
                        TipoDato = "Frecuencia Cardíaca Promedio Entrenamiento",
                        Valor = (decimal)workout.AvgHeartRate.Qty,
                        Unidad = workout.AvgHeartRate.Units,
                        FechaMedicion = workout.Start,
                        FechaRegistro = DateTime.UtcNow,
                        Fuente = "Health Auto Export - Entrenamiento",
                        Dispositivo = "iPhone/Apple Watch",
                        Notas = $"Entrenamiento: {workout.Name}"
                    });
                }

                // Frecuencia cardíaca máxima
                if (workout.MaxHeartRate.Qty > 0)
                {
                    datosVitales.Add(new DatoVital
                    {
                        UsuarioId = usuarioId,
                        TipoDato = "Frecuencia Cardíaca Máxima Entrenamiento",
                        Valor = (decimal)workout.MaxHeartRate.Qty,
                        Unidad = workout.MaxHeartRate.Units,
                        FechaMedicion = workout.Start,
                        FechaRegistro = DateTime.UtcNow,
                        Fuente = "Health Auto Export - Entrenamiento",
                        Dispositivo = "iPhone/Apple Watch",
                        Notas = $"Entrenamiento: {workout.Name}"
                    });
                }

                // Velocidad
                if (workout.Speed.Qty > 0)
                {
                    datosVitales.Add(new DatoVital
                    {
                        UsuarioId = usuarioId,
                        TipoDato = "Velocidad Entrenamiento",
                        Valor = (decimal)workout.Speed.Qty,
                        Unidad = workout.Speed.Units,
                        FechaMedicion = workout.Start,
                        FechaRegistro = DateTime.UtcNow,
                        Fuente = "Health Auto Export - Entrenamiento",
                        Dispositivo = "iPhone/Apple Watch",
                        Notas = $"Entrenamiento: {workout.Name}"
                    });
                }

                // Guardar todos los datos vitales
                foreach (var datoVital in datosVitales)
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
                        response.ProcessedRecords++;
                    }
                    else
                    {
                        response.SkippedRecords++;
                    }
                }

                await _context.SaveChangesAsync();
                response.Success = true;
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = ex.Message;
                response.Errors.Add(ex.Message);
                _logger.LogError(ex, $"Error procesando entrenamiento {workout.Name}");
            }

            return response;
        }
    }
} 