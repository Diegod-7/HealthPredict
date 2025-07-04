using System.ComponentModel.DataAnnotations;

namespace HealthPredict.Models.HealthAutoExport
{
    // Modelo principal para recibir datos de Health Auto Export
    public class HealthAutoExportData
    {
        public string DataType { get; set; } = string.Empty;
        public string Source { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public double Value { get; set; }
        public string Unit { get; set; } = string.Empty;
        public Dictionary<string, object> Metadata { get; set; } = new();
        public string DeviceModel { get; set; } = string.Empty;
        public string DeviceManufacturer { get; set; } = string.Empty;
        public string AppVersion { get; set; } = string.Empty;
    }

    // Modelo para batch de datos (múltiples registros)
    public class HealthAutoExportBatch
    {
        public List<HealthAutoExportData> Data { get; set; } = new();
        public DateTime ExportTimestamp { get; set; }
        public string ExportVersion { get; set; } = string.Empty;
        public string DeviceInfo { get; set; } = string.Empty;
    }

    // Modelo para datos de pasos
    public class StepCountData : HealthAutoExportData
    {
        public int Steps { get; set; }
        public double Distance { get; set; }
        public string DistanceUnit { get; set; } = "m";
    }

    // Modelo para datos de frecuencia cardíaca
    public class HeartRateData : HealthAutoExportData
    {
        public int HeartRate { get; set; }
        public string Context { get; set; } = string.Empty; // "resting", "active", "workout"
        public string MotionContext { get; set; } = string.Empty;
    }

    // Modelo para datos de presión arterial
    public class BloodPressureData : HealthAutoExportData
    {
        public int SystolicPressure { get; set; }
        public int DiastolicPressure { get; set; }
        public string MeasurementMethod { get; set; } = string.Empty;
        public string BodyPosition { get; set; } = string.Empty;
    }

    // Modelo para datos de glucosa
    public class BloodGlucoseData : HealthAutoExportData
    {
        public double GlucoseLevel { get; set; }
        public string MealContext { get; set; } = string.Empty; // "fasting", "postprandial", etc.
        public string SpecimenSource { get; set; } = string.Empty;
    }

    // Modelo para datos de peso
    public class BodyWeightData : HealthAutoExportData
    {
        public double Weight { get; set; }
        public string WeightUnit { get; set; } = "kg";
        public double? BMI { get; set; }
        public double? BodyFatPercentage { get; set; }
    }

    // Modelo para datos de sueño
    public class SleepData : HealthAutoExportData
    {
        public string SleepStage { get; set; } = string.Empty; // "awake", "light", "deep", "rem"
        public int DurationMinutes { get; set; }
        public DateTime BedTime { get; set; }
        public DateTime WakeTime { get; set; }
        public double SleepEfficiency { get; set; }
    }

    // Modelo para datos de entrenamientos
    public class WorkoutData : HealthAutoExportData
    {
        public string WorkoutType { get; set; } = string.Empty;
        public int DurationMinutes { get; set; }
        public double CaloriesBurned { get; set; }
        public double Distance { get; set; }
        public string DistanceUnit { get; set; } = "m";
        public int? AverageHeartRate { get; set; }
        public int? MaxHeartRate { get; set; }
        public List<WorkoutEvent> Events { get; set; } = new();
        public WorkoutRoute? Route { get; set; }
    }

    // Modelo para eventos de entrenamiento
    public class WorkoutEvent
    {
        public DateTime Timestamp { get; set; }
        public string EventType { get; set; } = string.Empty;
        public double? Value { get; set; }
        public string Unit { get; set; } = string.Empty;
    }

    // Modelo para rutas de entrenamiento
    public class WorkoutRoute
    {
        public List<LocationPoint> Points { get; set; } = new();
        public double TotalDistance { get; set; }
        public double ElevationGain { get; set; }
    }

    // Modelo para puntos de ubicación
    public class LocationPoint
    {
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public double? Altitude { get; set; }
        public DateTime Timestamp { get; set; }
        public double? Speed { get; set; }
        public double? Course { get; set; }
    }

    // Modelo para datos de temperatura corporal
    public class BodyTemperatureData : HealthAutoExportData
    {
        public double Temperature { get; set; }
        public string TemperatureUnit { get; set; } = "°C";
        public string MeasurementLocation { get; set; } = string.Empty; // "oral", "axillary", etc.
    }

    // Modelo para datos de saturación de oxígeno
    public class OxygenSaturationData : HealthAutoExportData
    {
        public double OxygenSaturation { get; set; }
        public string MeasurementMethod { get; set; } = string.Empty;
    }

    // Modelo para respuesta de la API
    public class HealthAutoExportResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public int ProcessedRecords { get; set; }
        public int SkippedRecords { get; set; }
        public List<string> Errors { get; set; } = new();
        public DateTime ProcessedAt { get; set; }
    }

    // Modelo para configuración de Health Auto Export
    public class HealthAutoExportConfig
    {
        public int Id { get; set; }
        public int UsuarioId { get; set; }
        public string ApiKey { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? LastSyncAt { get; set; }
        public string AllowedDataTypes { get; set; } = string.Empty; // JSON array
        public string DeviceInfo { get; set; } = string.Empty;
        public int SyncIntervalMinutes { get; set; } = 60;
    }

    // Modelo para estadísticas de sincronización
    public class SyncStats
    {
        public int TotalRecords { get; set; }
        public int ProcessedRecords { get; set; }
        public int SkippedRecords { get; set; }
        public int ErrorRecords { get; set; }
        public DateTime LastSync { get; set; }
        public Dictionary<string, int> DataTypeBreakdown { get; set; } = new();
    }
} 