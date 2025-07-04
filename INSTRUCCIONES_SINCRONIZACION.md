# 🔄 Sistema de Sincronización HealthPredict

## Descripción General

Este sistema permite sincronizar automáticamente los datos de pasos desde un archivo específico en Google Drive (`HealthAutoExport-2025-07-04.json`) hacia la API de HealthPredict.

## 🚀 Características

- ✅ **Archivo fijo**: Siempre sincroniza `HealthAutoExport-2025-07-04.json`
- ✅ **Ubicación fija**: Busca en `Mi unidad/HealthAutoExport/Health`
- ✅ **Botón en la app**: Interfaz integrada en el dashboard
- ✅ **Sincronización automática**: Desde Google Drive a la API
- ✅ **Feedback en tiempo real**: Notificaciones de éxito/error
- ✅ **Información detallada**: Muestra datos del archivo procesado

## 📱 Uso desde la Aplicación

### 1. Acceder al Dashboard
- Inicia sesión en HealthPredict
- Ve al dashboard principal

### 2. Encontrar el Botón de Sincronización
- Busca la sección "🔄 Sincronización de Datos"
- Verás un botón azul "Sincronizar desde Google Drive"

### 3. Ejecutar la Sincronización
- Haz clic en "Sincronizar desde Google Drive"
- El botón mostrará "Sincronizando..." con un spinner
- Espera a que complete el proceso

### 4. Ver Resultados
- ✅ **Éxito**: Notificación verde con confirmación
- ❌ **Error**: Notificación roja con detalles del error
- 📊 **Información**: Detalles del archivo procesado

## 🐍 Uso desde Script Python

### Opción 1: Script Interactivo
```bash
python google-drive-sync.py
```

**Opciones disponibles:**
1. Sincronizar una vez
2. Monitorear continuamente
3. Salir

### Opción 2: Script Simple (para la app)
```bash
python sync_pasos_simple.py
```

Este script:
- No requiere interacción
- Retorna JSON con el resultado
- Está optimizado para llamadas desde la API

## 🔧 Configuración

### Archivo Configurado
- **Nombre**: `HealthAutoExport-2025-07-04.json`
- **Ubicación**: `Mi unidad/HealthAutoExport/Health`
- **Tipo**: Datos de pasos en formato JSON

### API Endpoint
- **URL**: `https://healthpredict-l1hu.onrender.com/api/HealthAutoExport/pasos`
- **Método**: POST
- **Timeout**: 60 segundos

## 🛠️ Estructura del Proyecto

```
HealthPredict/
├── sync_pasos_simple.py          # Script simple para la app
├── google-drive-sync.py          # Script interactivo completo
├── credentials.json              # Credenciales de Google Drive
├── token.json                    # Token de autenticación
├── requirements.txt              # Dependencias Python
└── HealthPredict.Client/
    └── src/app/
        ├── services/
        │   └── google-drive-sync.service.ts
        └── components/
            └── google-drive-sync/
                ├── google-drive-sync.component.ts
                ├── google-drive-sync.component.html
                └── google-drive-sync.component.scss
```

## 📊 Endpoints de la API

### 1. Sincronizar desde Google Drive
```
POST /api/HealthAutoExport/sync-google-drive
```

**Respuesta exitosa:**
```json
{
  "success": true,
  "message": "Datos sincronizados exitosamente",
  "file_info": {
    "name": "HealthAutoExport-2025-07-04.json",
    "modified": "2025-07-04T10:30:00Z",
    "size": "848"
  }
}
```

### 2. Obtener última sincronización
```
GET /api/HealthAutoExport/ultima-sincronizacion
```

**Respuesta:**
```json
{
  "ultimaSincronizacion": "2025-07-04T10:30:00Z",
  "estado": "completada",
  "archivo": "HealthAutoExport-2025-07-04.json"
}
```

### 3. Recibir datos de pasos
```
POST /api/HealthAutoExport/pasos
```

**Cuerpo de la solicitud:**
```json
[
  {
    "date": "2025-07-04",
    "steps": 8500,
    "distance": 6.2,
    "calories": 320
  }
]
```

## 🚨 Manejo de Errores

### Errores Comunes

1. **Archivo no encontrado**
   - Verifica que el archivo existe en Google Drive
   - Confirma la ruta: `Mi unidad/HealthAutoExport/Health`

2. **Error de autenticación**
   - Revisa `credentials.json`
   - Elimina `token.json` y vuelve a autenticarte

3. **Error de API (503)**
   - El servicio puede estar temporalmente no disponible
   - Reintentar después de unos minutos

4. **Timeout**
   - Conexión lenta o servicio sobrecargado
   - Se reintenta automáticamente

### Códigos de Error

| Código | Descripción | Solución |
|--------|-------------|----------|
| `credentials_not_found` | Falta credentials.json | Configurar OAuth |
| `file_not_found` | Archivo no existe | Verificar ubicación |
| `download_error` | Error descargando | Verificar permisos |
| `json_error` | JSON inválido | Verificar formato |
| `api_error_503` | Servicio no disponible | Reintentar más tarde |
| `timeout` | Tiempo agotado | Verificar conexión |

## 🔐 Seguridad

- **OAuth 2.0**: Autenticación segura con Google
- **Permisos mínimos**: Solo lectura de Google Drive
- **HTTPS**: Todas las comunicaciones encriptadas
- **Validación**: Verificación de formato de datos

## 📈 Monitoreo

### Logs del Sistema
- Todos los eventos se registran en los logs
- Incluye timestamps y detalles de errores
- Útil para debugging y monitoreo

### Métricas
- Tiempo de sincronización
- Cantidad de datos procesados
- Tasa de éxito/error
- Frecuencia de uso

## 🔄 Flujo de Sincronización

1. **Usuario hace clic** en el botón de sincronización
2. **Frontend** envía solicitud a `/api/HealthAutoExport/sync-google-drive`
3. **Backend** ejecuta `sync_pasos_simple.py`
4. **Script Python**:
   - Se autentica con Google Drive
   - Busca `HealthAutoExport-2025-07-04.json`
   - Descarga el archivo
   - Envía datos a `/api/HealthAutoExport/pasos`
5. **API** procesa y guarda los datos
6. **Frontend** muestra resultado al usuario

## 🎯 Próximas Mejoras

- [ ] Sincronización programada automática
- [ ] Historial de sincronizaciones
- [ ] Soporte para múltiples archivos
- [ ] Notificaciones push
- [ ] Dashboard de métricas
- [ ] Configuración de intervalos personalizados

## 🆘 Soporte

Si tienes problemas:

1. **Revisa los logs** en la consola del navegador
2. **Verifica la configuración** de Google Drive
3. **Confirma la conectividad** de la API
4. **Consulta esta documentación** para errores comunes

---

**Última actualización**: 2025-07-04  
**Versión**: 1.0.0 