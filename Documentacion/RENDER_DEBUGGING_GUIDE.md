# 🔍 Guía de Diagnóstico para Render - HealthPredict

## 📋 **Problema Actual**
- ❌ Error al inicializar los datos de la base de datos
- ❌ No se puede iniciar sesión
- ❌ Posible problema de conexión con PostgreSQL

---

## 🚀 **PASO 1: Ver Logs Detallados en Render**

### **Acceder a los Logs:**
1. Ve a **https://dashboard.render.com**
2. Selecciona tu servicio **"HealthPredict API"**
3. Haz clic en la pestaña **"Logs"** en el menú lateral
4. Los logs se actualizan en tiempo real

### **Mensajes Clave a Buscar:**

#### ✅ **Conexión Exitosa:**
```
🔍 PostgreSQL Connection String encontrado: true
✅ PostgreSQL Connection String configurado correctamente
🔗 Connection String: Host=ep-royal-dream...
🔄 Iniciando inicialización de la base de datos...
🔗 Probando conexión a la base de datos...
✅ Conexión a la base de datos exitosa
✅ Migraciones aplicadas correctamente
✅ Base de datos inicializada con datos de prueba
```

#### ❌ **Errores Comunes:**
```
❌ ERROR: No se encontró el string de conexión de PostgreSQL
❌ ERROR CRÍTICO al inicializar la base de datos:
   Tipo de excepción: [NpgsqlException/TimeoutException/etc]
   Mensaje: [mensaje específico del error]
```

---

## 🛠️ **PASO 2: Ejecutar Diagnóstico Automatizado**

### **Opción A: Script PowerShell (Recomendado)**
```powershell
.\check-render-logs.ps1
```

### **Opción B: Verificación Manual**
Ve a estos endpoints en tu navegador:

1. **Diagnóstico de BD:** https://healthpredict-api.onrender.com/api/Usuarios/diagnostico-bd
2. **Swagger UI:** https://healthpredict-api.onrender.com/swagger
3. **Lista de Usuarios:** https://healthpredict-api.onrender.com/api/Usuarios

---

## 🔧 **PASO 3: Soluciones por Tipo de Error**

### **Error 1: No se encuentra Connection String**
```
❌ ERROR: No se encontró el string de conexión de PostgreSQL
```

**Solución:**
1. Ve a **Render Dashboard** → **Tu Servicio** → **Environment**
2. Verifica que existe la variable: `DATABASE_URL`
3. Debe contener: `postgresql://user:password@host:port/database`

### **Error 2: Timeout de Conexión**
```
Tipo de excepción: TimeoutException
```

**Solución:**
1. Verifica que **Neon DB** esté activo
2. Ve a **Neon Console** → **Tu Database** → **Dashboard**
3. Si está "sleeping", haz una consulta para activarlo

### **Error 3: Error de Autenticación**
```
Tipo de excepción: NpgsqlException
Mensaje: password authentication failed
```

**Solución:**
1. Regenera las credenciales en **Neon**
2. Actualiza `DATABASE_URL` en **Render**
3. Haz **Manual Deploy** para reiniciar

### **Error 4: Base de Datos No Existe**
```
Mensaje: database "neondb" does not exist
```

**Solución:**
1. Verifica el nombre de la base de datos en **Neon**
2. Actualiza el connection string
3. Asegúrate de usar el formato correcto

---

## 🔄 **PASO 4: Forzar Reinicialización**

### **Si la conexión funciona pero no hay usuarios:**

#### **Método 1: Endpoint de Inicialización**
```bash
curl -X POST https://healthpredict-api.onrender.com/api/Usuarios/forzar-inicializacion
```

#### **Método 2: Desde Swagger**
1. Ve a https://healthpredict-api.onrender.com/swagger
2. Busca `POST /api/Usuarios/forzar-inicializacion`
3. Haz clic en **"Try it out"** → **"Execute"**

---

## 🧪 **PASO 5: Probar Login**

### **Credenciales de Prueba:**
Una vez inicializados los datos, usa estas credenciales:

#### **Jefe:**
- **Email:** `carlos.rodriguez@healthpredict.com`
- **Password:** `admin123`

#### **Trabajadores:**
- **Diego:** `diego.diaz@healthpredict.com` / `diego123`
- **Matías:** `matias.maripangue@healthpredict.com` / `matias123`
- **Iahn:** `iahn.vera@healthpredict.com` / `iahn123`

### **Probar Login desde Swagger:**
1. Ve a `POST /api/Usuarios/authenticate`
2. Usa este JSON:
```json
{
  "email": "diego.diaz@healthpredict.com",
  "password": "diego123"
}
```

---

## 📊 **PASO 6: Interpretar Resultados del Diagnóstico**

### **Estado Óptimo:**
```json
{
  "diagnostico": {
    "connectionString": "✅ Configurado",
    "environment": "Production"
  },
  "conexion": {
    "puedeConectar": true,
    "mensaje": "✅ Conexión exitosa"
  },
  "baseDatos": {
    "totalUsuarios": 4,
    "totalTablas": 3
  },
  "estado": "✅ Base de datos operativa"
}
```

### **Problemas Detectados:**
- `"connectionString": "❌ No encontrado"` → Configurar DATABASE_URL
- `"puedeConectar": false` → Problema de red/credenciales
- `"totalUsuarios": 0` → Ejecutar inicialización
- `"error"` presente → Ver detalles del error

---

## 🚨 **PASO 7: Escalación de Problemas**

### **Si nada funciona:**

1. **Reinicio Completo:**
   - Render Dashboard → Settings → Manual Deploy
   - Esperar 2-3 minutos

2. **Verificar Neon DB:**
   - Neon Console → Database → Connection Details
   - Probar conexión desde local

3. **Recrear Variables:**
   - Eliminar `DATABASE_URL` en Render
   - Crear nueva con formato correcto
   - Manual Deploy

4. **Logs Detallados:**
   - Activar logging nivel Debug
   - Revisar stack traces completos

---

## 📞 **Contacto y Soporte**

### **Información para Soporte:**
Cuando reportes el problema, incluye:

1. **URL del servicio:** https://healthpredict-api.onrender.com
2. **Resultado del diagnóstico:** `/api/Usuarios/diagnostico-bd`
3. **Últimas 20 líneas de logs** de Render
4. **Variables de entorno** (sin credenciales)
5. **Timestamp** del último deploy

### **Comandos de Emergencia:**
```bash
# Verificar estado general
curl https://healthpredict-api.onrender.com/api/Usuarios/diagnostico-bd

# Forzar inicialización
curl -X POST https://healthpredict-api.onrender.com/api/Usuarios/forzar-inicializacion

# Probar login
curl -X POST https://healthpredict-api.onrender.com/api/Usuarios/authenticate \
  -H "Content-Type: application/json" \
  -d '{"email":"diego.diaz@healthpredict.com","password":"diego123"}'
```

---

## ✅ **Checklist de Verificación**

- [ ] Logs de Render revisados
- [ ] DATABASE_URL configurado
- [ ] Neon DB activo
- [ ] Diagnóstico ejecutado
- [ ] Inicialización forzada (si necesario)
- [ ] Login probado con credenciales correctas
- [ ] Swagger UI funcional

---

**¡Con esta guía deberías poder identificar y resolver el problema de inicialización de la base de datos!** 🚀 