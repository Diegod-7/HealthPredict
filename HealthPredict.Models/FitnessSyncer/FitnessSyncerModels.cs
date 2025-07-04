using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace HealthPredict.Models.FitnessSyncer
{
    // Modelo para la configuración de FitnessSyncer
    public class FitnessSyncerConfig
    {
        public int Id { get; set; }
        public int UsuarioId { get; set; }
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
        public DateTime TokenExpiry { get; set; }
        public bool IsActive { get; set; }
        public DateTime FechaCreacion { get; set; }
        public DateTime? UltimaSincronizacion { get; set; }
        
        // Relaciones
        public virtual Usuario? Usuario { get; set; }
    }

    // Modelos de respuesta de FitnessSyncer API
    public class FitnessSyncerSourcesResponse
    {
        [JsonPropertyName("items")]
        public List<FitnessSyncerSource> Items { get; set; } = new();
    }

    public class FitnessSyncerSource
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }
        
        [JsonPropertyName("name")]
        public string Name { get; set; }
        
        [JsonPropertyName("type")]
        public string Type { get; set; }
        
        [JsonPropertyName("enabled")]
        public bool Enabled { get; set; }
        
        [JsonPropertyName("providerType")]
        public string ProviderType { get; set; }
        
        [JsonPropertyName("lastError")]
        public string? LastError { get; set; }
        
        [JsonPropertyName("date")]
        public long? Date { get; set; }
    }

    public class FitnessSyncerItemsResponse
    {
        [JsonPropertyName("items")]
        public List<FitnessSyncerItem> Items { get; set; } = new();
    }

    public class FitnessSyncerItem
    {
        [JsonPropertyName("itemId")]
        public string ItemId { get; set; }
        
        [JsonPropertyName("date")]
        public long Date { get; set; }
        
        [JsonPropertyName("activity")]
        public FitnessSyncerActivity? Activity { get; set; }
        
        [JsonPropertyName("weight")]
        public FitnessSyncerWeight? Weight { get; set; }
        
        [JsonPropertyName("bloodPressure")]
        public FitnessSyncerBloodPressure? BloodPressure { get; set; }
        
        [JsonPropertyName("glucose")]
        public FitnessSyncerGlucose? Glucose { get; set; }
        
        [JsonPropertyName("sleep")]
        public FitnessSyncerSleep? Sleep { get; set; }
        
        [JsonPropertyName("nutrition")]
        public FitnessSyncerNutrition? Nutrition { get; set; }
    }

    public class FitnessSyncerActivity
    {
        [JsonPropertyName("steps")]
        public int? Steps { get; set; }
        
        [JsonPropertyName("distance")]
        public decimal? Distance { get; set; }
        
        [JsonPropertyName("calories")]
        public decimal? Calories { get; set; }
        
        [JsonPropertyName("duration")]
        public int? Duration { get; set; }
        
        [JsonPropertyName("heartRate")]
        public decimal? HeartRate { get; set; }
        
        [JsonPropertyName("activityType")]
        public string? ActivityType { get; set; }
    }

    public class FitnessSyncerWeight
    {
        [JsonPropertyName("weight")]
        public decimal Weight { get; set; }
        
        [JsonPropertyName("bodyFat")]
        public decimal? BodyFat { get; set; }
        
        [JsonPropertyName("muscleMass")]
        public decimal? MuscleMass { get; set; }
        
        [JsonPropertyName("bmi")]
        public decimal? BMI { get; set; }
    }

    public class FitnessSyncerBloodPressure
    {
        [JsonPropertyName("systolic")]
        public decimal Systolic { get; set; }
        
        [JsonPropertyName("diastolic")]
        public decimal Diastolic { get; set; }
        
        [JsonPropertyName("pulse")]
        public decimal? Pulse { get; set; }
    }

    public class FitnessSyncerGlucose
    {
        [JsonPropertyName("glucose")]
        public decimal Glucose { get; set; }
        
        [JsonPropertyName("mealType")]
        public string? MealType { get; set; }
    }

    public class FitnessSyncerSleep
    {
        [JsonPropertyName("duration")]
        public int Duration { get; set; }
        
        [JsonPropertyName("efficiency")]
        public decimal? Efficiency { get; set; }
        
        [JsonPropertyName("deepSleep")]
        public int? DeepSleep { get; set; }
        
        [JsonPropertyName("lightSleep")]
        public int? LightSleep { get; set; }
        
        [JsonPropertyName("remSleep")]
        public int? RemSleep { get; set; }
    }

    public class FitnessSyncerNutrition
    {
        [JsonPropertyName("calories")]
        public decimal? Calories { get; set; }
        
        [JsonPropertyName("protein")]
        public decimal? Protein { get; set; }
        
        [JsonPropertyName("carbs")]
        public decimal? Carbs { get; set; }
        
        [JsonPropertyName("fat")]
        public decimal? Fat { get; set; }
        
        [JsonPropertyName("fiber")]
        public decimal? Fiber { get; set; }
        
        [JsonPropertyName("sugar")]
        public decimal? Sugar { get; set; }
        
        [JsonPropertyName("sodium")]
        public decimal? Sodium { get; set; }
    }

    // Modelo para la respuesta de autenticación OAuth
    public class FitnessSyncerAuthResponse
    {
        [JsonPropertyName("access_token")]
        public string AccessToken { get; set; }
        
        [JsonPropertyName("refresh_token")]
        public string RefreshToken { get; set; }
        
        [JsonPropertyName("expires_in")]
        public int ExpiresIn { get; set; }
        
        [JsonPropertyName("token_type")]
        public string TokenType { get; set; }
    }

    // Modelo para solicitar refresh del token
    public class FitnessSyncerRefreshRequest
    {
        [JsonPropertyName("grant_type")]
        public string GrantType { get; set; } = "refresh_token";
        
        [JsonPropertyName("refresh_token")]
        public string RefreshToken { get; set; }
        
        [JsonPropertyName("client_id")]
        public string ClientId { get; set; }
        
        [JsonPropertyName("client_secret")]
        public string ClientSecret { get; set; }
    }

    // Modelo para el resultado de sincronización
    public class SyncResult
    {
        public bool Success { get; set; }
        public int ProcessedItems { get; set; }
        public int NewItems { get; set; }
        public int UpdatedItems { get; set; }
        public int SkippedItems { get; set; }
        public List<string> Errors { get; set; } = new();
        public DateTime SyncTime { get; set; }
        public string? LastSyncedItemId { get; set; }
    }

    // Modelo para configuración de sincronización
    public class SyncConfiguration
    {
        public bool SyncActivity { get; set; } = true;
        public bool SyncWeight { get; set; } = true;
        public bool SyncBloodPressure { get; set; } = true;
        public bool SyncGlucose { get; set; } = true;
        public bool SyncSleep { get; set; } = true;
        public bool SyncNutrition { get; set; } = true;
        public int MaxItemsPerSync { get; set; } = 100;
        public int DaysToSync { get; set; } = 30;
        public bool AutoSync { get; set; } = false;
        public int AutoSyncIntervalHours { get; set; } = 24;
    }

    // Modelo para estadísticas de sincronización
    public class SyncStats
    {
        public int TotalSyncs { get; set; }
        public DateTime? LastSuccessfulSync { get; set; }
        public DateTime? LastFailedSync { get; set; }
        public int TotalItemsSynced { get; set; }
        public int FailedSyncs { get; set; }
        public List<string> ActiveSources { get; set; } = new();
        public Dictionary<string, int> ItemsByType { get; set; } = new();
    }
} 