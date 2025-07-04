# 🚨 SOLUCIÓN CORS: Vercel → Render

## ❌ Error Actual
```
Access to XMLHttpRequest at 'https://healthpredict-l1hu.onrender.com/api/usuarios/authenticate' 
from origin 'https://health-predict-eggtvl0sc-diego-diazs-projects-dabcb856.vercel.app' 
has been blocked by CORS policy: Response to preflight request doesn't pass access control check: 
No 'Access-Control-Allow-Origin' header is present on the requested resource.
```

## ✅ Configuraciones Aplicadas

### 1. **Program.cs - Configuración CORS Dinámica**
- ✅ Configuración flexible para dominios Vercel
- ✅ Logging de requests CORS
- ✅ Endpoint de testing: `/api/cors-test`

### 2. **appsettings.Production.json**
- ✅ Lista actualizada de dominios permitidos
- ✅ Incluye tu dominio específico de Vercel

## 🔍 PASOS DE VERIFICACIÓN

### **Paso 1: Verificar Backend (Render)**
1. **Deploy** los cambios a Render
2. **Verificar logs** de Render para mensajes CORS
3. **Probar endpoint test**: `https://healthpredict-l1hu.onrender.com/api/cors-test`

### **Paso 2: Testing CORS**
```bash
# Desde terminal/PowerShell, probar CORS manualmente:
curl -H "Origin: https://health-predict-eggtvl0sc-diego-diazs-projects-dabcb856.vercel.app" \
     -H "Access-Control-Request-Method: POST" \
     -H "Access-Control-Request-Headers: Content-Type" \
     -X OPTIONS \
     https://healthpredict-l1hu.onrender.com/api/usuarios/authenticate
```

### **Paso 3: Verificar Logs en Render**
Buscar en los logs de Render:
```
🌐 CORS Request from: https://health-predict-eggtvl0sc-diego-diazs-projects-dabcb856.vercel.app
✅ CORS Response: Access-Control-Allow-Origin = https://health-predict-eggtvl0sc-diego-diazs-projects-dabcb856.vercel.app
```

## 🚀 PASOS PARA DESPLEGAR

### **1. Commitear y pushear cambios:**
```bash
git add .
git commit -m "fix: Configuración CORS para Vercel"
git push origin main
```

### **2. Verificar deployment en Render:**
- Ir a dashboard de Render
- Verificar que el deploy se complete exitosamente
- Revisar logs del deploy

### **3. Probar desde Vercel:**
- Abrir la aplicación en Vercel
- Intentar login
- Verificar Network tab en DevTools

## 🛠️ TROUBLESHOOTING ADICIONAL

### **Problema 1: Aún hay error CORS**
**Solución A - Configuración temporal permisiva:**
En `Program.cs`, cambiar temporalmente:
```csharp
if (builder.Environment.IsDevelopment())
{
    policy.AllowAnyOrigin()
          .AllowAnyMethod()
          .AllowAnyHeader();
}
else
{
    // TEMPORAL - SOLO PARA TESTING
    policy.AllowAnyOrigin()
          .AllowAnyMethod()
          .AllowAnyHeader();
}
```

**Solución B - Variables de entorno en Render:**
1. Ir a Render Dashboard
2. Environment Variables
3. Agregar:
   - `ASPNETCORE_ENVIRONMENT` = `Production`
   - `CORS_ALLOWED_ORIGINS` = `https://health-predict-eggtvl0sc-diego-diazs-projects-dabcb856.vercel.app`

### **Problema 2: Preflight requests fallan**
Verificar que el endpoint maneje OPTIONS:
```csharp
app.MapMethods("/api/usuarios/authenticate", new[] { "OPTIONS" }, () => Results.Ok());
```

### **Problema 3: Headers específicos**
Agregar headers específicos si es necesario:
```csharp
policy.WithHeaders("Content-Type", "Authorization", "X-Requested-With");
```

## 🔧 TESTING ENDPOINTS

### **1. Test básico CORS:**
```
GET https://healthpredict-l1hu.onrender.com/api/cors-test
```

### **2. Test preflight:**
```
OPTIONS https://healthpredict-l1hu.onrender.com/api/usuarios/authenticate
```

### **3. Test completo desde browser:**
```javascript
// En console de Vercel app:
fetch('https://healthpredict-l1hu.onrender.com/api/cors-test')
  .then(r => r.json())
  .then(console.log)
  .catch(console.error);
```

## 📋 CHECKLIST POST-DEPLOY

- [ ] ✅ Cambios pusheados a GitHub
- [ ] ✅ Render deploy completado
- [ ] ✅ Logs de Render muestran configuración CORS
- [ ] ✅ Endpoint `/api/cors-test` responde correctamente
- [ ] ✅ Preflight requests funcionan
- [ ] ✅ Login desde Vercel funciona
- [ ] ✅ No hay errores CORS en console

## 🆘 SI NADA FUNCIONA

**Solución de emergencia - CORS permisivo temporal:**
```csharp
// En Program.cs - SOLO PARA DEBUGGING
builder.Services.AddCors(options => {
    options.AddDefaultPolicy(policy => {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

app.UseCors(); // Sin nombre de policy
```

**⚠️ IMPORTANTE:** Esta solución permisiva es SOLO para identificar el problema. Una vez que funcione, volver a la configuración específica por seguridad. 