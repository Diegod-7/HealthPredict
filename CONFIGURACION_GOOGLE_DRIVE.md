# 🚀 Configuración de Sincronización con Google Drive

Este documento te guiará paso a paso para configurar la sincronización automática de datos de pasos desde Google Drive a tu API de HealthPredict.

## 📋 Requisitos Previos

### 1. Python 3.7 o superior
```bash
python --version
```

### 2. Instalar dependencias
```bash
pip install google-api-python-client google-auth-httplib2 google-auth-oauthlib requests
```

## 🔧 Configuración de Google Drive API

### Paso 1: Crear un Proyecto en Google Cloud Console

1. Ve a [Google Cloud Console](https://console.cloud.google.com/)
2. Crea un nuevo proyecto o selecciona uno existente
3. Asegúrate de que el proyecto esté seleccionado

### Paso 2: Habilitar la API de Google Drive

1. En el menú lateral, ve a **APIs y servicios** → **Biblioteca**
2. Busca "Google Drive API"
3. Haz clic en "Google Drive API" y luego en **HABILITAR**

### Paso 3: Crear Credenciales OAuth 2.0

1. Ve a **APIs y servicios** → **Credenciales**
2. Haz clic en **+ CREAR CREDENCIALES** → **ID de cliente de OAuth 2.0**
3. Si es la primera vez, configura la pantalla de consentimiento:
   - Tipo de usuario: **Externo**
   - Información de la aplicación:
     - Nombre: `HealthPredict Drive Sync`
     - Correo de soporte: tu email
     - Dominio autorizado: puedes dejarlo vacío
   - Ámbitos: No agregues ninguno por ahora
   - Usuarios de prueba: Agrega tu email

4. Crear el ID de cliente:
   - Tipo de aplicación: **Aplicación de escritorio**
   - Nombre: `HealthPredict Desktop Client`

5. **Descargar el archivo JSON** de credenciales
6. Renombrar el archivo a `credentials.json`
7. Colocar el archivo en la misma carpeta que `google-drive-sync.py`

## 📁 Estructura de Archivos

```
tu-proyecto/
├── google-drive-sync.py          # Script principal
├── credentials.json              # Credenciales de Google (descargado)
├── token.json                    # Se genera automáticamente
├── requirements.txt              # Dependencias
└── CONFIGURACION_GOOGLE_DRIVE.md # Esta documentación
```

## 🏃‍♂️ Uso del Script

### Ejecución Básica
```bash
python google-drive-sync.py
```

### Opciones Disponibles

1. **Sincronizar una vez**: Descarga y envía el archivo a la API una sola vez
2. **Monitorear continuamente**: Revisa el archivo cada X minutos y sincroniza si hay cambios
3. **Salir**: Termina el programa

### Ejemplo de Uso

```bash
$ python google-drive-sync.py

🚀 Google Drive Health Sync - Iniciando...
==================================================
✅ Servicio de Google Drive configurado correctamente

📋 Opciones disponibles:
1. Sincronizar archivo una vez
2. Monitorear archivo continuamente
3. Salir

👉 Selecciona una opción (1-3): 2
📁 Nombre del archivo en Google Drive: pasos_salud.json
⏰ Intervalo en minutos (default: 5): 10

👁️ Monitoreando archivo: pasos_salud.json
⏰ Intervalo: 10 minutos
✅ Archivo encontrado: pasos_salud.json
📅 Última modificación: 2024-01-15T10:30:00.000Z
📏 Tamaño: 2048 bytes
```

## 📊 Formatos de JSON Soportados

El script detecta automáticamente el formato de tus datos y los envía al endpoint correcto:

### 1. Formato Simple
```json
{
  "DataType": "HKQuantityTypeIdentifierStepCount",
  "Source": "Health Auto Export",
  "StartDate": "2024-01-15T10:00:00.000Z",
  "EndDate": "2024-01-15T10:00:00.000Z",
  "Value": 8547,
  "Unit": "count",
  "Metadata": {},
  "DeviceModel": "iPhone 14 Pro",
  "DeviceManufacturer": "Apple",
  "AppVersion": "1.0.0"
}
```

### 2. Formato Batch (Lote)
```json
{
  "Data": [
    {
      "DataType": "HKQuantityTypeIdentifierStepCount",
      "Source": "Health Auto Export",
      "StartDate": "2024-01-15T10:00:00.000Z",
      "EndDate": "2024-01-15T10:00:00.000Z",
      "Value": 8547,
      "Unit": "count"
    },
    {
      "DataType": "HKQuantityTypeIdentifierStepCount",
      "Source": "Health Auto Export",
      "StartDate": "2024-01-14T10:00:00.000Z",
      "EndDate": "2024-01-14T10:00:00.000Z",
      "Value": 7234,
      "Unit": "count"
    }
  ],
  "ExportTimestamp": "2024-01-15T10:00:00.000Z",
  "ExportVersion": "1.0.0",
  "DeviceInfo": "iPhone 14 Pro"
}
```

### 3. Formato HealthAutoExport
```json
{
  "Data": {
    "Metrics": [
      {
        "Name": "step_count",
        "Units": "count",
        "Data": [
          {
            "Qty": 8547,
            "Date": "2024-01-15T10:00:00.000Z"
          }
        ]
      }
    ],
    "Workouts": []
  }
}
```

### 4. Formato Lista Simple
```json
[
  {
    "date": "2024-01-15",
    "steps": 8547,
    "distance": 6.2,
    "calories": 245
  }
]
```

## 🔄 Automatización Avanzada

### Ejecutar como Servicio (Windows)

1. Crear un archivo batch `sync_service.bat`:
```batch
@echo off
cd /d "C:\ruta\a\tu\proyecto"
python google-drive-sync.py
pause
```

2. Crear una tarea programada en Windows:
   - Abrir "Programador de tareas"
   - Crear tarea básica
   - Configurar para ejecutar el script cada X minutos

### Ejecutar como Servicio (Linux/Mac)

1. Crear un script shell `sync_service.sh`:
```bash
#!/bin/bash
cd /ruta/a/tu/proyecto
python3 google-drive-sync.py
```

2. Configurar como servicio systemd o usar cron:
```bash
# Editar crontab
crontab -e

# Ejecutar cada 5 minutos
*/5 * * * * /ruta/a/tu/proyecto/sync_service.sh
```

## 🛠️ Personalización

### Cambiar URL de la API
Edita la variable `API_URL` en el script:
```python
API_URL = 'https://tu-api-personalizada.com/api/HealthAutoExport'
```

### Agregar API Key
Si tu API requiere autenticación, modifica la función `send_to_api`:
```python
headers = {
    'Content-Type': 'application/json',
    'X-API-Key': 'tu-api-key-aqui',
    'User-Agent': 'GoogleDriveHealthSync/1.0'
}
```

## 🔍 Solución de Problemas

### Error: "No se encontró el archivo credentials.json"
- Asegúrate de haber descargado las credenciales de Google Cloud Console
- Verifica que el archivo esté en la misma carpeta que el script
- Confirma que el archivo se llame exactamente `credentials.json`

### Error: "Archivo no encontrado en Google Drive"
- Verifica que el archivo existe en tu Google Drive
- Asegúrate de escribir el nombre exacto (incluyendo extensión)
- El archivo debe estar en la raíz de tu Drive o especifica la ruta completa

### Error de conexión a la API
- Verifica que la URL de la API sea correcta
- Confirma que tu API esté funcionando visitando la URL en el navegador
- Revisa tu conexión a internet

### Error de permisos
- Asegúrate de haber autorizado la aplicación en Google Drive
- Verifica que tu cuenta tenga acceso al archivo
- Revisa que los permisos de la API de Google Drive estén configurados correctamente

## 📈 Monitoreo y Logs

El script proporciona logs detallados:
- ✅ Operaciones exitosas
- ❌ Errores y problemas
- 📊 Información sobre los datos procesados
- ⏰ Timestamps de todas las operaciones

## 🔐 Seguridad

- Las credenciales se almacenan localmente en `credentials.json` y `token.json`
- El script solo tiene permisos de lectura en Google Drive
- Los tokens se renuevan automáticamente
- No se almacenan credenciales en el código fuente

## 🆘 Soporte

Si tienes problemas:
1. Revisa los logs del script
2. Verifica la configuración de Google Cloud Console
3. Confirma que tu archivo JSON tenga el formato correcto
4. Prueba la API manualmente con el archivo HTML de prueba

¡Listo! Con esta configuración tendrás sincronización automática de tus datos de pasos desde Google Drive a tu API de HealthPredict. 