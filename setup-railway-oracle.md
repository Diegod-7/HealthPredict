# 🚂 Configurar Oracle XE en Railway - Paso a Paso

## 🎯 **Tutorial Completo (5 minutos)**

### **Paso 1: Crear Cuenta Railway**
1. **Ir a:** [railway.app](https://railway.app)
2. **Click:** "Start a New Project"
3. **Login con GitHub** (recomendado)
4. **Verificar email** si es necesario

### **Paso 2: Crear Proyecto Oracle**
1. **Dashboard** → **"New Project"**
2. **"Empty Project"**
3. **Nombre:** `healthpredict-oracle`

### **Paso 3: Agregar Servicio Oracle XE**
1. **"+ New"** → **"Database"** → **"Add PostgreSQL"** 
   *(No, espera... vamos por Docker)*
2. **"+ New"** → **"Empty Service"**
3. **Settings** → **Source** → **"Docker Image"**
4. **Docker Image:** `gvenzl/oracle-xe:21-slim`

### **Paso 4: Configurar Variables de Entorno**
En **Variables** agregar:
```bash
ORACLE_PASSWORD=HealthPredict2024
ORACLE_DATABASE=HEALTHPREDICTDB
APP_USER=healthuser
APP_USER_PASSWORD=healthpass123
ORACLE_CHARACTERSET=AL32UTF8
```

### **Paso 5: Configurar Networking**
1. **Settings** → **Networking**
2. **Generate Domain** (para obtener URL pública)
3. **Port:** `1521`

### **Paso 6: Deploy y Esperar**
1. **Deploy** automáticamente iniciará
2. **Esperar 2-3 minutos** para que Oracle inicie
3. **Ver logs** para confirmar que está corriendo

### **Paso 7: Obtener Connection String**
Después del deploy, tendrás:
```
Host: healthpredict-oracle-production-xxxx.up.railway.app
Port: 443 (HTTPS) o el puerto asignado
Database: HEALTHPREDICTDB
User: healthuser
Password: healthpass123
```

**Connection String Final:**
```
Data Source=healthpredict-oracle-production-xxxx.up.railway.app:PORT/HEALTHPREDICTDB;User Id=healthuser;Password=healthpass123;
```

## 🔧 **Actualizar tu Aplicación**

### **1. Actualizar appsettings.json**
```json
{
  "ConnectionStrings": {
    "OracleConnection": "Data Source=tu-url-railway:puerto/HEALTHPREDICTDB;User Id=healthuser;Password=healthpass123;"
  }
}
```

### **2. Crear/Ejecutar Migraciones**
```bash
# Si no tienes migraciones
dotnet ef migrations add InitialCreate

# Aplicar a Railway
dotnet ef database update
```

### **3. Probar Conexión**
```bash
# Compilar y probar
dotnet run

# Verificar en browser
https://localhost:5001/swagger
```

## ✅ **Verificar que Funciona**
1. **Railway Dashboard** → Ver que el servicio está "Running" 
2. **Logs** → Debe mostrar "Oracle Database is ready"
3. **Tu API** → `/api/DatosVitales` debe devolver `[]` sin error

## 💰 **Costos Estimados**
- **Railway:** ~$3-4 USD/mes
- **Crédito gratis:** $5 USD/mes
- **Resultado:** **GRATIS** efectivamente

## 🚨 **Troubleshooting**
- **Error "Failed to start":** Verificar variables de entorno
- **Connection timeout:** Verificar puerto y URL
- **Out of memory:** Oracle XE necesita ~512MB RAM mínimo

## 🎉 **¡Listo!**
Ahora tienes Oracle XE corriendo en la nube, gratis, y accesible desde cualquier lugar. 