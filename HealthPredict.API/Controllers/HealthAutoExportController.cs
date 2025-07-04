using Microsoft.AspNetCore.Mvc;
using HealthPredict.Models.HealthAutoExport;
using HealthPredict.BLL;
using Microsoft.AspNetCore.Authorization;

namespace HealthPredict.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class HealthAutoExportController : ControllerBase
    {
        private readonly HealthAutoExportService _healthAutoExportService;
        private readonly ILogger<HealthAutoExportController> _logger;

        public HealthAutoExportController(
            HealthAutoExportService healthAutoExportService,
            ILogger<HealthAutoExportController> logger)
        {
            _healthAutoExportService = healthAutoExportService;
            _logger = logger;
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
        /// Endpoint para recibir datos en formato JSON genérico
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

                // Intentar deserializar como HealthAutoExportData
                var jsonString = System.Text.Json.JsonSerializer.Serialize(jsonData);
                var data = System.Text.Json.JsonSerializer.Deserialize<HealthAutoExportData>(jsonString);

                if (data == null)
                {
                    return BadRequest(new HealthAutoExportResponse
                    {
                        Success = false,
                        Message = "No se pudo procesar los datos JSON",
                        ProcessedAt = DateTime.UtcNow
                    });
                }

                var response = await _healthAutoExportService.ProcessHealthDataAsync(data, usuarioId);
                return Ok(response);
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
    }
} 