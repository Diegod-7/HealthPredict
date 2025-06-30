# 🚀 Docker + ngrok - Exponer API Local Online

## 📋 **¿Qué es ngrok?**
ngrok crea un túnel seguro desde tu máquina local hacia internet, permitiendo que tu API sea accesible desde cualquier lugar.

## 🛠️ **Instalación y Configuración**

### **1. Instalar ngrok**
1. **Ir a:** [ngrok.com](https://ngrok.com)
2. **Crear cuenta gratuita**
3. **Descargar ngrok** para Windows
4. **Extraer** `ngrok.exe` en una carpeta (ej: `C:\ngrok\`)

### **2. Configurar ngrok**
```bash
# Autenticar con tu token (obtenlo de ngrok.com)
ngrok config add-authtoken TU_TOKEN_AQUI
```

### **3. Ejecutar tu API con Docker**
```bash
# Compilar la imagen
docker build -t healthpredict-api .

# Ejecutar en puerto 5000
docker run -p 5000:80 healthpredict-api
```

### **4. Exponer con ngrok**
```bash
# En otra terminal
ngrok http 5000
```

## 🌐 **Resultado**
ngrok te dará una URL como:
```
https://abc123.ngrok.io → http://localhost:5000
```

## 📱 **Uso desde Angular/Android/iOS**
```typescript
// En environment.ts
export const environment = {
  production: false,
  apiUrl: 'https://abc123.ngrok.io/api'
};
```

## ⚡ **Ventajas**
- ✅ **Gratis** (con limitaciones)
- ✅ **Fácil de usar**
- ✅ **HTTPS automático**
- ✅ **No requiere configuración de red**
- ✅ **Funciona detrás de firewalls**

## 🔒 **Limitaciones Gratuitas**
- 🔄 URL cambia cada vez que reinicias
- ⏱️ Sesiones de 2 horas
- 🚦 Límite de conexiones

## 💰 **ngrok Pro (Opcional)**
- 🔗 URL fija (ej: `healthpredict.ngrok.io`)
- ⏰ Sin límite de tiempo
- 🚀 Más conexiones

---

# 🐳 **Scripts de Automatización**

## **start-api.bat**
```batch
@echo off
echo Iniciando HealthPredict API con Docker...
docker build -t healthpredict-api .
docker run -p 5000:80 healthpredict-api
```

## **expose-api.bat**
```batch
@echo off
echo Exponiendo API con ngrok...
ngrok http 5000
```

## **full-setup.bat**
```batch
@echo off
echo === HealthPredict API + ngrok ===
echo.
echo 1. Iniciando Docker...
start "Docker API" cmd /k "docker build -t healthpredict-api . && docker run -p 5000:80 healthpredict-api"
echo.
echo 2. Esperando 10 segundos...
timeout /t 10 /nobreak
echo.
echo 3. Iniciando ngrok...
start "ngrok" cmd /k "ngrok http 5000"
echo.
echo ✅ API expuesta online!
echo Copia la URL de ngrok y úsala en tus apps
pause
``` 