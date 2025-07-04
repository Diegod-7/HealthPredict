using HealthPredict.BLL;
using HealthPredict.Models.FitnessSyncer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace HealthPredict.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class FitnessSyncerController : ControllerBase
    {
        private readonly FitnessSyncerService _fitnessSyncerService;
        private readonly ILogger<FitnessSyncerController> _logger;

        public FitnessSyncerController(
            FitnessSyncerService fitnessSyncerService,
            ILogger<FitnessSyncerController> logger)
        {
            _fitnessSyncerService = fitnessSyncerService;
            _logger = logger;
        }

        /// <summary>
        /// Obtiene la URL de autorización para conectar con FitnessSyncer
        /// </summary>
        [HttpGet("auth/url")]
        public IActionResult GetAuthorizationUrl()
        {
            try
            {
                var usuarioId = GetCurrentUserId();
                var redirectUri = $"{Request.Scheme}://{Request.Host}/api/fitnesssyncer/auth/callback";
                
                var authUrl = _fitnessSyncerService.GetAuthorizationUrl(usuarioId, redirectUri);
                
                return Ok(new { 
                    authUrl = authUrl,
                    redirectUri = redirectUri
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error obteniendo URL de autorización");
                return BadRequest(new { error = "Error obteniendo URL de autorización" });
            }
        }

        /// <summary>
        /// Callback de autorización OAuth de FitnessSyncer
        /// </summary>
        [HttpGet("auth/callback")]
        [AllowAnonymous]
        public async Task<IActionResult> AuthCallback([FromQuery] string code, [FromQuery] string state)
        {
            try
            {
                if (string.IsNullOrEmpty(code) || string.IsNullOrEmpty(state))
                {
                    return BadRequest(new { error = "Código o estado faltante" });
                }

                // Decodificar el estado para obtener el usuario ID
                var stateDecoded = System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(state));
                var parts = stateDecoded.Split('|');
                if (parts.Length != 2 || !int.TryParse(parts[0], out int usuarioId))
                {
                    return BadRequest(new { error = "Estado inválido" });
                }

                var redirectUri = $"{Request.Scheme}://{Request.Host}/api/fitnesssyncer/auth/callback";
                var success = await _fitnessSyncerService.ExchangeCodeForTokensAsync(usuarioId, code, redirectUri);

                if (success)
                {
                    // Redirigir al frontend con éxito
                    return Redirect($"/dashboard?fitnesssyncer=connected");
                }
                else
                {
                    return Redirect($"/dashboard?fitnesssyncer=error");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error en callback de autorización");
                return Redirect($"/dashboard?fitnesssyncer=error");
            }
        }

        /// <summary>
        /// Verifica si el usuario está conectado a FitnessSyncer
        /// </summary>
        [HttpGet("status")]
        public async Task<IActionResult> GetConnectionStatus()
        {
            try
            {
                var usuarioId = GetCurrentUserId();
                var isConnected = await _fitnessSyncerService.IsUserConnectedAsync(usuarioId);
                
                return Ok(new { isConnected = isConnected });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error verificando estado de conexión");
                return BadRequest(new { error = "Error verificando estado de conexión" });
            }
        }

        /// <summary>
        /// Sincroniza los datos de fitness del usuario
        /// </summary>
        [HttpPost("sync")]
        public async Task<IActionResult> SyncData([FromBody] SyncConfiguration? config = null)
        {
            try
            {
                var usuarioId = GetCurrentUserId();
                
                var result = await _fitnessSyncerService.SyncUserDataAsync(usuarioId, config);
                
                if (result.Success)
                {
                    return Ok(new { 
                        message = "Sincronización completada exitosamente",
                        result = result
                    });
                }
                else
                {
                    return BadRequest(new { 
                        message = "Sincronización completada con errores",
                        result = result
                    });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error durante la sincronización");
                return BadRequest(new { error = "Error durante la sincronización" });
            }
        }

        /// <summary>
        /// Obtiene las estadísticas de sincronización del usuario
        /// </summary>
        [HttpGet("stats")]
        public async Task<IActionResult> GetSyncStats()
        {
            try
            {
                var usuarioId = GetCurrentUserId();
                var stats = await _fitnessSyncerService.GetSyncStatsAsync(usuarioId);
                
                return Ok(stats);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error obteniendo estadísticas de sincronización");
                return BadRequest(new { error = "Error obteniendo estadísticas de sincronización" });
            }
        }

        /// <summary>
        /// Desconecta al usuario de FitnessSyncer
        /// </summary>
        [HttpPost("disconnect")]
        public async Task<IActionResult> Disconnect()
        {
            try
            {
                var usuarioId = GetCurrentUserId();
                var success = await _fitnessSyncerService.DisconnectUserAsync(usuarioId);
                
                if (success)
                {
                    return Ok(new { message = "Desconectado de FitnessSyncer exitosamente" });
                }
                else
                {
                    return BadRequest(new { error = "Error desconectando de FitnessSyncer" });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error desconectando de FitnessSyncer");
                return BadRequest(new { error = "Error desconectando de FitnessSyncer" });
            }
        }

        /// <summary>
        /// Endpoint para testing - simula una sincronización con datos de prueba
        /// </summary>
        [HttpPost("test")]
        public async Task<IActionResult> TestSync()
        {
            try
            {
                var usuarioId = GetCurrentUserId();
                
                // Crear configuración de prueba
                var testConfig = new SyncConfiguration
                {
                    MaxItemsPerSync = 10,
                    DaysToSync = 7,
                    SyncActivity = true,
                    SyncWeight = true,
                    SyncBloodPressure = true
                };

                var result = await _fitnessSyncerService.SyncUserDataAsync(usuarioId, testConfig);
                
                return Ok(new { 
                    message = "Sincronización de prueba completada",
                    result = result
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error en sincronización de prueba");
                return BadRequest(new { error = "Error en sincronización de prueba" });
            }
        }

        /// <summary>
        /// Endpoint para configuración avanzada de sincronización
        /// </summary>
        [HttpPost("configure")]
        public async Task<IActionResult> ConfigureSync([FromBody] SyncConfiguration config)
        {
            try
            {
                var usuarioId = GetCurrentUserId();
                
                // Validar configuración
                if (config.MaxItemsPerSync <= 0 || config.MaxItemsPerSync > 1000)
                {
                    return BadRequest(new { error = "MaxItemsPerSync debe estar entre 1 y 1000" });
                }

                if (config.DaysToSync <= 0 || config.DaysToSync > 365)
                {
                    return BadRequest(new { error = "DaysToSync debe estar entre 1 y 365" });
                }

                // Realizar sincronización con la configuración personalizada
                var result = await _fitnessSyncerService.SyncUserDataAsync(usuarioId, config);
                
                return Ok(new { 
                    message = "Sincronización configurada y ejecutada",
                    config = config,
                    result = result
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error configurando sincronización");
                return BadRequest(new { error = "Error configurando sincronización" });
            }
        }

        /// <summary>
        /// Obtiene el ID del usuario actual desde el token JWT
        /// </summary>
        private int GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
            {
                throw new UnauthorizedAccessException("Usuario no autenticado");
            }
            return userId;
        }
    }
} 