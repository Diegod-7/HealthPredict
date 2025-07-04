# Health Auto Export - Formato Estándar

## Descripción

Se ha implementado soporte completo para el formato estándar de Health Auto Export, que permite recibir datos de salud en el formato JSON estructurado que utiliza la aplicación oficial.

## Nuevo Endpoint

### POST `/api/HealthAutoExport/health-data`

Este endpoint acepta datos en el formato estándar de Health Auto Export con la siguiente estructura:

```json
{
  "data": {
    "metrics": [
      {
        "name": "stepcount",
        "units": "count",
        "data": [
          {
            "qty": 8532,
            "date": "2024-01-15 14:30:00 +0000"
          }
        ]
      },
      {
        "name": "heartrate",
        "units": "bpm",
        "data": [
          {
            "date": "2024-01-15 14:30:00 +0000",
            "min": 65,
            "avg": 72,
            "max": 85
          }
        ]
      },
      {
        "name": "bloodpressure",
        "units": "mmHg",
        "data": [
          {
            "date": "2024-01-15 14:30:00 +0000",
            "systolic": 120,
            "diastolic": 80
          }
        ]
      }
    ],
    "workouts": [
      {
        "name": "Running",
        "start": "2024-01-15 08:00:00 +0000",
        "end": "2024-01-15 09:00:00 +0000",
        "activeEnergy": {
          "qty": 450,
          "units": "kcal"
        },
        "distance": {
          "qty": 5.2,
          "units": "km"
        },
        "avgHeartRate": {
          "qty": 145,
          "units": "bpm"
        }
      }
    ]
  }
}
```

## Tipos de Datos Soportados

### Métricas de Salud

| Nombre | Descripción | Unidades Comunes |
|--------|-------------|------------------|
| `stepcount` | Conteo de pasos | count, steps |
| `heartrate` | Frecuencia cardíaca | bpm |
| `bloodpressure` | Presión arterial | mmHg |
| `bloodglucose` | Glucosa en sangre | mg/dL, mmol/L |
| `bodyweight` | Peso corporal | kg, lb |
| `bodytemperature` | Temperatura corporal | °C, °F |
| `oxygensaturation` | Saturación de oxígeno | % |
| `sleepanalysis` | Análisis de sueño | minutos |
| `activeenergyburned` | Energía activa quemada | kcal, kJ |
| `distancewalking` | Distancia caminando | km, mi |
| `vo2max` | VO2 máximo | mL/kg/min |
| `restingheartrate` | Frecuencia cardíaca en reposo | bpm |
| `walkingheartrateaverage` | FC promedio caminando | bpm |
| `respiratoryrate` | Frecuencia respiratoria | breaths/min |

### Formatos Específicos

#### Presión Arterial
```json
{
  "date": "2024-01-15 14:30:00 +0000",
  "systolic": 120,
  "diastolic": 80
}
```

#### Frecuencia Cardíaca
```json
{
  "date": "2024-01-15 14:30:00 +0000",
  "min": 65,
  "avg": 72,
  "max": 85
}
```

#### Análisis de Sueño
```json
{
  "date": "2024-01-15",
  "asleep": 420,
  "sleepStart": "2024-01-15 23:00:00 +0000",
  "sleepEnd": "2024-01-16 07:00:00 +0000",
  "sleepSource": "Apple Watch",
  "inBed": 480,
  "inBedStart": "2024-01-15 22:30:00 +0000",
  "inBedEnd": "2024-01-16 07:30:00 +0000",
  "inBedSource": "Apple Watch"
}
```

#### Glucosa en Sangre
```json
{
  "date": "2024-01-15 14:30:00 +0000",
  "qty": 95,
  "mealTime": "Before Meal"
}
```

### Entrenamientos

Los entrenamientos se procesan como datos vitales individuales:

- **Energía Activa Quemada**: Calorías quemadas durante el ejercicio
- **Energía Total Quemada**: Calorías totales del entrenamiento
- **Pasos de Entrenamiento**: Pasos dados durante el ejercicio
- **Distancia de Entrenamiento**: Distancia recorrida
- **Frecuencia Cardíaca Promedio**: FC promedio durante el ejercicio
- **Frecuencia Cardíaca Máxima**: FC máxima alcanzada
- **Velocidad**: Velocidad promedio del entrenamiento

## Autenticación

### Opcional con API Key
```bash
curl -X POST "https://tu-api.com/api/HealthAutoExport/health-data" \
  -H "Content-Type: application/json" \
  -H "X-API-Key: tu-api-key" \
  -d @health-data.json
```

### Sin autenticación (para pruebas)
```bash
curl -X POST "https://tu-api.com/api/HealthAutoExport/health-data" \
  -H "Content-Type: application/json" \
  -d @health-data.json
```

## Respuesta de la API

```json
{
  "success": true,
  "message": "Procesados 15 registros, omitidos 2",
  "processedRecords": 15,
  "skippedRecords": 2,
  "errors": [],
  "processedAt": "2024-01-15T14:30:00.000Z"
}
```

## Características

### ✅ Implementado
- ✅ Formato estándar de Health Auto Export
- ✅ Procesamiento de métricas de salud
- ✅ Procesamiento de entrenamientos
- ✅ Detección de duplicados
- ✅ Mapeo automático de tipos de datos
- ✅ Validación de API Key opcional
- ✅ Logging detallado
- ✅ Manejo de errores robusto

### 🔄 Características Adicionales
- Soporte para formatos específicos (presión arterial, frecuencia cardíaca, sueño)
- Procesamiento de metadatos de entrenamientos
- Conversión automática de unidades
- Almacenamiento de información de dispositivo
- Estadísticas de sincronización

## Endpoints Relacionados

- `GET /api/HealthAutoExport/test` - Verificar conectividad
- `GET /api/HealthAutoExport/config` - Obtener configuración
- `GET /api/HealthAutoExport/stats` - Estadísticas de sincronización
- `POST /api/HealthAutoExport/generate-api-key` - Generar nueva API Key

## Compatibilidad

Este endpoint es totalmente compatible con:
- Health Auto Export iOS App
- Apple Health data export
- Formato JSON estándar de Health Auto Export
- Todos los tipos de datos soportados por la aplicación

## Migración

Los endpoints existentes siguen funcionando:
- `/api/HealthAutoExport/data` - Formato individual
- `/api/HealthAutoExport/batch` - Formato por lotes
- `/api/HealthAutoExport/simple` - Formato simplificado
- `/api/HealthAutoExport/json` - Formato JSON genérico

El nuevo endpoint `/api/HealthAutoExport/health-data` es la forma recomendada para integraciones con Health Auto Export. 