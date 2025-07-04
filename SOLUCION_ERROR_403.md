# 🔧 Solución Error 403: access_denied

## 🎯 **Problema**
Error 403 al intentar autenticar con Google Drive API.

## 🔍 **Causa**
La aplicación no está configurada correctamente para acceso externo o tu email no está en la lista de usuarios de prueba.

## ✅ **Solución Completa**

### **Paso 1: Configurar la Pantalla de Consentimiento OAuth**

1. **Ve a [Google Cloud Console](https://console.cloud.google.com/)**
2. **Selecciona tu proyecto**
3. **Ve a: APIs y servicios → Pantalla de consentimiento de OAuth**

#### **Configuración Requerida:**
```
✅ Tipo de usuario: Externo
✅ Nombre de la aplicación: HealthPredict Drive Sync
✅ Correo electrónico de asistencia: [tu-email]
✅ Logotipo: (opcional)
✅ Dominio autorizado: (puede estar vacío)
✅ Correo electrónico del desarrollador: [tu-email]
```

### **Paso 2: Agregar Usuarios de Prueba (CRÍTICO)**

**⚠️ Este es el paso más importante:**

1. **En la pantalla de consentimiento OAuth**
2. **Busca la sección "Usuarios de prueba"**
3. **Haz clic en "AGREGAR USUARIOS"**
4. **Agrega tu email** (el mismo que usas para Google Drive)
5. **Haz clic en "GUARDAR"**

### **Paso 3: Configurar Ámbitos**

1. **En la pantalla de consentimiento OAuth**
2. **Ve a "Ámbitos"**
3. **Haz clic en "AGREGAR O QUITAR ÁMBITOS"**
4. **Busca "Google Drive API"**
5. **Selecciona:** `https://www.googleapis.com/auth/drive.readonly`
6. **Guarda los cambios**

### **Paso 4: Verificar Credenciales OAuth**

1. **Ve a: APIs y servicios → Credenciales**
2. **Encuentra tu "ID de cliente OAuth 2.0"**
3. **Haz clic en el icono de descarga** 📥
4. **Descarga el archivo JSON**
5. **Renómbralo a `credentials.json`**
6. **Reemplaza el archivo anterior**

### **Paso 5: Limpiar Tokens Existentes**

```bash
# Elimina el token anterior si existe
rm token.json

# O en Windows
del token.json
```

### **Paso 6: Ejecutar Nuevamente**

```bash
python google-drive-sync.py
```

## 🔄 **Proceso de Autenticación Correcto**

Cuando ejecutes el script, deberías ver:

1. **Se abre tu navegador automáticamente**
2. **Aparece la pantalla de Google OAuth**
3. **Selecciona tu cuenta de Google**
4. **Acepta los permisos solicitados**
5. **Ves el mensaje: "The authentication flow has completed."**

## 🚨 **Errores Comunes y Soluciones**

### **Error: "This app isn't verified"**
```
Solución:
1. Haz clic en "Advanced" (Avanzado)
2. Haz clic en "Go to HealthPredict Drive Sync (unsafe)"
3. Acepta los permisos
```

### **Error: "access_denied" persiste**
```
Solución:
1. Verifica que tu email esté en "Usuarios de prueba"
2. Asegúrate de usar el mismo email para autenticarte
3. Elimina token.json y vuelve a intentar
```

### **Error: "redirect_uri_mismatch"**
```
Solución:
1. Ve a Credenciales → Tu OAuth Client
2. Agrega estos URIs de redirección:
   - http://localhost:8080/
   - http://localhost:8090/
   - http://localhost:55992/
```

## 📋 **Checklist de Verificación**

Antes de ejecutar el script, verifica:

- [ ] ✅ Proyecto creado en Google Cloud Console
- [ ] ✅ API de Google Drive habilitada
- [ ] ✅ Pantalla de consentimiento OAuth configurada
- [ ] ✅ Tu email agregado como usuario de prueba
- [ ] ✅ Credenciales OAuth 2.0 creadas
- [ ] ✅ Archivo `credentials.json` descargado
- [ ] ✅ Archivo `token.json` eliminado (si existe)
- [ ] ✅ Dependencias de Python instaladas

## 🎯 **Configuración Final Recomendada**

### **Pantalla de Consentimiento OAuth:**
```
Tipo de usuario: Externo
Estado: En pruebas
Usuarios de prueba: [tu-email@gmail.com]
Ámbitos: https://www.googleapis.com/auth/drive.readonly
```

### **Credenciales OAuth:**
```
Tipo: Aplicación de escritorio
Nombre: HealthPredict Desktop Client
URIs de redirección: http://localhost:*
```

## 🔐 **Notas de Seguridad**

- La aplicación solo tiene permisos de **lectura** en Google Drive
- Los tokens se almacenan **localmente** en tu computadora
- **No se comparten** credenciales con terceros
- Puedes **revocar** el acceso en cualquier momento desde tu cuenta de Google

## 🆘 **Si Aún Tienes Problemas**

1. **Elimina el proyecto** en Google Cloud Console
2. **Crea un nuevo proyecto** desde cero
3. **Sigue todos los pasos** nuevamente
4. **Asegúrate** de agregar tu email como usuario de prueba

## 📞 **Contacto**

Si después de seguir todos estos pasos aún tienes problemas, proporciona:
- Captura de pantalla del error
- Configuración de la pantalla de consentimiento
- Archivo `credentials.json` (sin datos sensibles)

¡Con estos pasos deberías poder resolver el error 403 y conectarte exitosamente a Google Drive! 