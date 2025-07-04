using HealthPredict.DAL;
using HealthPredict.Models;
using HealthPredict.Models.FitnessSyncer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace HealthPredict.BLL
{
    public class FitnessSyncerService
    {
        private readonly HealthPredictContext _context;
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private readonly ILogger<FitnessSyncerService> _logger;
        private readonly DatoVitalService _datoVitalService;

        private readonly string _baseUrl = "https://api.fitnesssyncer.com/api";
        private readonly string _authUrl = "https://www.fitnesssyncer.com/api/oauth";

        public FitnessSyncerService(
            HealthPredictContext context,
            HttpClient httpClient,
            IConfiguration configuration,
            ILogger<FitnessSyncerService> logger,
            DatoVitalService datoVitalService)
        {
            _context = context;
            _httpClient = httpClient;
            _configuration = configuration;
            _logger = logger;
            _datoVitalService = datoVitalService;
        }

        #region Configuración y Autenticación

        /// <summary>
        /// Obtiene la URL de autorización OAuth para FitnessSyncer
        /// </summary>
        public string GetAuthorizationUrl(int usuarioId, string redirectUri)
        {
            var clientId = _configuration["FitnessSyncer:ClientId"];
            var scope = "sources";
            var state = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{usuarioId}|{DateTime.UtcNow.Ticks}"));

            var authUrl = $"{_authUrl}/authorize?" +
                         $"client_id={clientId}&" +
                         $"response_type=code&" +
                         $"scope={scope}&" +
                         $"redirect_uri={Uri.EscapeDataString(redirectUri)}&" +
                         $"state={state}";

            return authUrl;
        }

        /// <summary>
        /// Intercambia el código de autorización por tokens de acceso
        /// </summary>
        public async Task<bool> ExchangeCodeForTokensAsync(int usuarioId, string code, string redirectUri)
        {
            try
            {
                var clientId = _configuration["FitnessSyncer:ClientId"];
                var clientSecret = _configuration["FitnessSyncer:ClientSecret"];

                var requestData = new Dictionary<string, string>
                {
                    {"grant_type", "authorization_code"},
                    {"code", code},
                    {"client_id", clientId},
                    {"client_secret", clientSecret},
                    {"redirect_uri", redirectUri}
                };

                var content = new FormUrlEncodedContent(requestData);
                var response = await _httpClient.PostAsync($"{_authUrl}/access_token", content);

                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    var authResponse = JsonSerializer.Deserialize<FitnessSyncerAuthResponse>(responseContent);

                    if (authResponse != null)
                    {
                        await SaveOrUpdateConfigAsync(usuarioId, authResponse);
                        return true;
                    }
                }

                _logger.LogError($"Error al intercambiar código por tokens: {response.StatusCode}");
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al intercambiar código por tokens para usuario {usuarioId}");
                return false;
            }
        }

        /// <summary>
        /// Refresca el token de acceso usando el refresh token
        /// </summary>
        public async Task<bool> RefreshTokenAsync(int usuarioId)
        {
            try
            {
                var config = await GetConfigAsync(usuarioId);
                if (config == null || string.IsNullOrEmpty(config.RefreshToken))
                {
                    return false;
                }

                var clientId = _configuration["FitnessSyncer:ClientId"];
                var clientSecret = _configuration["FitnessSyncer:ClientSecret"];

                var refreshRequest = new FitnessSyncerRefreshRequest
                {
                    RefreshToken = config.RefreshToken,
                    ClientId = clientId,
                    ClientSecret = clientSecret
                };

                var json = JsonSerializer.Serialize(refreshRequest);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync($"{_authUrl}/access_token", content);

                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    var authResponse = JsonSerializer.Deserialize<FitnessSyncerAuthResponse>(responseContent);

                    if (authResponse != null)
                    {
                        await SaveOrUpdateConfigAsync(usuarioId, authResponse);
                        return true;
                    }
                }

                _logger.LogError($"Error al refrescar token: {response.StatusCode}");
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al refrescar token para usuario {usuarioId}");
                return false;
            }
        }

        private async Task SaveOrUpdateConfigAsync(int usuarioId, FitnessSyncerAuthResponse authResponse)
        {
            var config = await _context.FitnessSyncerConfigs
                .FirstOrDefaultAsync(c => c.UsuarioId == usuarioId);

            if (config == null)
            {
                config = new FitnessSyncerConfig
                {
                    UsuarioId = usuarioId,
                    FechaCreacion = DateTime.UtcNow,
                    IsActive = true
                };
                _context.FitnessSyncerConfigs.Add(config);
            }

            config.AccessToken = authResponse.AccessToken;
            config.RefreshToken = authResponse.RefreshToken;
            config.TokenExpiry = DateTime.UtcNow.AddSeconds(authResponse.ExpiresIn);
            config.IsActive = true;

            await _context.SaveChangesAsync();
        }

        private async Task<FitnessSyncerConfig?> GetConfigAsync(int usuarioId)
        {
            return await _context.FitnessSyncerConfigs
                .FirstOrDefaultAsync(c => c.UsuarioId == usuarioId && c.IsActive);
        }

        #endregion

        #region Sincronización de Datos

        /// <summary>
        /// Sincroniza todos los datos de fitness para un usuario
        /// </summary>
        public async Task<SyncResult> SyncUserDataAsync(int usuarioId, SyncConfiguration? config = null)
        {
            var result = new SyncResult
            {
                SyncTime = DateTime.UtcNow
            };

            try
            {
                var userConfig = await GetConfigAsync(usuarioId);
                if (userConfig == null || !userConfig.IsActive)
                {
                    result.Errors.Add("Usuario no tiene configuración activa de FitnessSyncer");
                    return result;
                }

                // Verificar si el token necesita ser refrescado
                if (userConfig.TokenExpiry <= DateTime.UtcNow.AddMinutes(5))
                {
                    var refreshed = await RefreshTokenAsync(usuarioId);
                    if (!refreshed)
                    {
                        result.Errors.Add("No se pudo refrescar el token de acceso");
                        return result;
                    }
                    userConfig = await GetConfigAsync(usuarioId); // Obtener config actualizada
                }

                // Configurar headers de autorización
                _httpClient.DefaultRequestHeaders.Clear();
                _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {userConfig.AccessToken}");

                // Obtener fuentes de datos
                var sources = await GetUserSourcesAsync();
                if (sources == null || !sources.Any())
                {
                    result.Errors.Add("No se encontraron fuentes de datos activas");
                    return result;
                }

                config ??= new SyncConfiguration(); // Usar configuración por defecto si no se proporciona

                // Sincronizar datos de cada fuente
                foreach (var source in sources.Where(s => s.Enabled))
                {
                    try
                    {
                        var sourceResult = await SyncSourceDataAsync(usuarioId, source, config);
                        result.ProcessedItems += sourceResult.ProcessedItems;
                        result.NewItems += sourceResult.NewItems;
                        result.UpdatedItems += sourceResult.UpdatedItems;
                        result.SkippedItems += sourceResult.SkippedItems;
                        result.Errors.AddRange(sourceResult.Errors);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, $"Error sincronizando fuente {source.Id} para usuario {usuarioId}");
                        result.Errors.Add($"Error en fuente {source.Name}: {ex.Message}");
                    }
                }

                // Actualizar última sincronización
                userConfig.UltimaSincronizacion = DateTime.UtcNow;
                await _context.SaveChangesAsync();

                result.Success = result.Errors.Count == 0 || result.NewItems > 0;

                _logger.LogInformation($"Sincronización completada para usuario {usuarioId}. " +
                                     $"Nuevos: {result.NewItems}, Actualizados: {result.UpdatedItems}, " +
                                     $"Omitidos: {result.SkippedItems}, Errores: {result.Errors.Count}");

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error general en sincronización para usuario {usuarioId}");
                result.Errors.Add($"Error general: {ex.Message}");
                return result;
            }
        }

        private async Task<List<FitnessSyncerSource>?> GetUserSourcesAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync($"{_baseUrl}/providers/sources/");
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var sourcesResponse = JsonSerializer.Deserialize<FitnessSyncerSourcesResponse>(content);
                    return sourcesResponse?.Items;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error obteniendo fuentes de usuario");
            }
            return null;
        }

        private async Task<SyncResult> SyncSourceDataAsync(int usuarioId, FitnessSyncerSource source, SyncConfiguration config)
        {
            var result = new SyncResult();

            try
            {
                // Calcular fecha de inicio para la sincronización
                var startDate = DateTime.UtcNow.AddDays(-config.DaysToSync);
                var lastSync = await _context.FitnessSyncerConfigs
                    .Where(c => c.UsuarioId == usuarioId)
                    .Select(c => c.UltimaSincronizacion)
                    .FirstOrDefaultAsync();

                if (lastSync.HasValue && lastSync.Value > startDate)
                {
                    startDate = lastSync.Value;
                }

                // Obtener datos de la fuente
                var url = $"{_baseUrl}/providers/sources/{source.Id}/items/?limit={config.MaxItemsPerSync}";
                var response = await _httpClient.GetAsync(url);

                if (!response.IsSuccessStatusCode)
                {
                    result.Errors.Add($"Error obteniendo datos de {source.Name}: {response.StatusCode}");
                    return result;
                }

                var content = await response.Content.ReadAsStringAsync();
                var itemsResponse = JsonSerializer.Deserialize<FitnessSyncerItemsResponse>(content);

                if (itemsResponse?.Items == null || !itemsResponse.Items.Any())
                {
                    return result;
                }

                // Procesar cada item
                foreach (var item in itemsResponse.Items)
                {
                    try
                    {
                        var itemDate = DateTimeOffset.FromUnixTimeMilliseconds(item.Date).DateTime;
                        if (itemDate < startDate)
                        {
                            result.SkippedItems++;
                            continue;
                        }

                        var datosVitales = ConvertToHealthPredictData(usuarioId, item, source);
                        if (datosVitales.Any())
                        {
                            var newItems = await _datoVitalService.CreateDatosVitalesEnLoteAsync(datosVitales);
                            result.NewItems += newItems.Count;
                            result.ProcessedItems++;
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, $"Error procesando item {item.ItemId} de fuente {source.Id}");
                        result.Errors.Add($"Error procesando item: {ex.Message}");
                    }
                }

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error sincronizando fuente {source.Id}");
                result.Errors.Add($"Error en fuente {source.Name}: {ex.Message}");
                return result;
            }
        }

        #endregion

        #region Conversión de Datos

        private List<DatoVital> ConvertToHealthPredictData(int usuarioId, FitnessSyncerItem item, FitnessSyncerSource source)
        {
            var datosVitales = new List<DatoVital>();
            var fecha = DateTimeOffset.FromUnixTimeMilliseconds(item.Date).DateTime;

            try
            {
                // Convertir datos de actividad
                if (item.Activity != null)
                {
                    if (item.Activity.Steps.HasValue)
                    {
                        datosVitales.Add(CreateDatoVital(usuarioId, fecha, "pasos", item.Activity.Steps.Value, "pasos", source.Name));
                    }

                    if (item.Activity.Distance.HasValue)
                    {
                        datosVitales.Add(CreateDatoVital(usuarioId, fecha, "distancia", item.Activity.Distance.Value, "metros", source.Name));
                    }

                    if (item.Activity.Calories.HasValue)
                    {
                        datosVitales.Add(CreateDatoVital(usuarioId, fecha, "calorias", item.Activity.Calories.Value, "kcal", source.Name));
                    }

                    if (item.Activity.HeartRate.HasValue)
                    {
                        datosVitales.Add(CreateDatoVital(usuarioId, fecha, "frecuencia_cardiaca", item.Activity.HeartRate.Value, "bpm", source.Name));
                    }
                }

                // Convertir datos de peso
                if (item.Weight != null)
                {
                    datosVitales.Add(CreateDatoVital(usuarioId, fecha, "peso", item.Weight.Weight, "kg", source.Name));

                    if (item.Weight.BodyFat.HasValue)
                    {
                        datosVitales.Add(CreateDatoVital(usuarioId, fecha, "grasa_corporal", item.Weight.BodyFat.Value, "%", source.Name));
                    }

                    if (item.Weight.BMI.HasValue)
                    {
                        datosVitales.Add(CreateDatoVital(usuarioId, fecha, "imc", item.Weight.BMI.Value, "kg/m²", source.Name));
                    }
                }

                // Convertir datos de presión arterial
                if (item.BloodPressure != null)
                {
                    datosVitales.Add(CreateDatoVital(usuarioId, fecha, "presion_sistolica", item.BloodPressure.Systolic, "mmHg", source.Name));
                    datosVitales.Add(CreateDatoVital(usuarioId, fecha, "presion_diastolica", item.BloodPressure.Diastolic, "mmHg", source.Name));

                    if (item.BloodPressure.Pulse.HasValue)
                    {
                        datosVitales.Add(CreateDatoVital(usuarioId, fecha, "pulso", item.BloodPressure.Pulse.Value, "bpm", source.Name));
                    }
                }

                // Convertir datos de glucosa
                if (item.Glucose != null)
                {
                    var notas = !string.IsNullOrEmpty(item.Glucose.MealType) ? $"Tipo de comida: {item.Glucose.MealType}" : null;
                    datosVitales.Add(CreateDatoVital(usuarioId, fecha, "glucosa", item.Glucose.Glucose, "mg/dL", source.Name, notas));
                }

                // Convertir datos de sueño
                if (item.Sleep != null)
                {
                    datosVitales.Add(CreateDatoVital(usuarioId, fecha, "duracion_sueno", item.Sleep.Duration, "minutos", source.Name));

                    if (item.Sleep.Efficiency.HasValue)
                    {
                        datosVitales.Add(CreateDatoVital(usuarioId, fecha, "eficiencia_sueno", item.Sleep.Efficiency.Value, "%", source.Name));
                    }
                }

                // Convertir datos de nutrición
                if (item.Nutrition != null)
                {
                    if (item.Nutrition.Calories.HasValue)
                    {
                        datosVitales.Add(CreateDatoVital(usuarioId, fecha, "calorias_consumidas", item.Nutrition.Calories.Value, "kcal", source.Name));
                    }

                    if (item.Nutrition.Protein.HasValue)
                    {
                        datosVitales.Add(CreateDatoVital(usuarioId, fecha, "proteinas", item.Nutrition.Protein.Value, "g", source.Name));
                    }

                    if (item.Nutrition.Carbs.HasValue)
                    {
                        datosVitales.Add(CreateDatoVital(usuarioId, fecha, "carbohidratos", item.Nutrition.Carbs.Value, "g", source.Name));
                    }

                    if (item.Nutrition.Fat.HasValue)
                    {
                        datosVitales.Add(CreateDatoVital(usuarioId, fecha, "grasas", item.Nutrition.Fat.Value, "g", source.Name));
                    }
                }

                return datosVitales;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error convirtiendo datos del item {item.ItemId}");
                return new List<DatoVital>();
            }
        }

        private DatoVital CreateDatoVital(int usuarioId, DateTime fecha, string tipo, decimal valor, string unidad, string dispositivo, string? notas = null)
        {
            return new DatoVital
            {
                UsuarioId = usuarioId,
                FechaRegistro = fecha,
                TipoDato = tipo,
                Valor = valor,
                Unidad = unidad,
                DispositivoOrigen = $"FitnessSyncer - {dispositivo}",
                Notas = notas
            };
        }

        #endregion

        #region Métodos de Consulta

        /// <summary>
        /// Obtiene las estadísticas de sincronización para un usuario
        /// </summary>
        public async Task<SyncStats> GetSyncStatsAsync(int usuarioId)
        {
            var config = await GetConfigAsync(usuarioId);
            if (config == null)
            {
                return new SyncStats();
            }

            // Obtener estadísticas de la base de datos
            var datosVitales = await _context.DatosVitales
                .Where(d => d.UsuarioId == usuarioId && d.DispositivoOrigen.StartsWith("FitnessSyncer"))
                .ToListAsync();

            var stats = new SyncStats
            {
                LastSuccessfulSync = config.UltimaSincronizacion,
                TotalItemsSynced = datosVitales.Count,
                ItemsByType = datosVitales
                    .GroupBy(d => d.TipoDato)
                    .ToDictionary(g => g.Key, g => g.Count())
            };

            // Obtener fuentes activas
            try
            {
                _httpClient.DefaultRequestHeaders.Clear();
                _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {config.AccessToken}");
                
                var sources = await GetUserSourcesAsync();
                if (sources != null)
                {
                    stats.ActiveSources = sources
                        .Where(s => s.Enabled)
                        .Select(s => s.Name)
                        .ToList();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error obteniendo fuentes activas para usuario {usuarioId}");
            }

            return stats;
        }

        /// <summary>
        /// Verifica si un usuario tiene configuración activa
        /// </summary>
        public async Task<bool> IsUserConnectedAsync(int usuarioId)
        {
            var config = await GetConfigAsync(usuarioId);
            return config != null && config.IsActive;
        }

        /// <summary>
        /// Desconecta un usuario de FitnessSyncer
        /// </summary>
        public async Task<bool> DisconnectUserAsync(int usuarioId)
        {
            try
            {
                var config = await GetConfigAsync(usuarioId);
                if (config != null)
                {
                    config.IsActive = false;
                    await _context.SaveChangesAsync();
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error desconectando usuario {usuarioId} de FitnessSyncer");
                return false;
            }
        }

        #endregion
    }
} 