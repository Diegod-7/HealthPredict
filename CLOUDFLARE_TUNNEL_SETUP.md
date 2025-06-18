# ☁️ Docker + Cloudflare Tunnel - Alternativa Profesional

## 📋 **¿Qué es Cloudflare Tunnel?**
Cloudflare Tunnel expone tu aplicación local de forma segura sin abrir puertos en tu router. Es completamente **GRATIS** y más profesional que ngrok.

## 🛠️ **Instalación y Configuración**

### **1. Instalar cloudflared**
```bash
# Descargar desde: https://github.com/cloudflare/cloudflared/releases
# O usar winget (Windows 11)
winget install cloudflare.cloudflared
```

### **2. Autenticar con Cloudflare**
```bash
cloudflared tunnel login
```

### **3. Crear un túnel**
```bash
# Crear túnel llamado "healthpredict"
cloudflared tunnel create healthpredict

# Configurar DNS (si tienes dominio propio)
cloudflared tunnel route dns healthpredict api.tudominio.com
```

### **4. Ejecutar tu API con Docker**
```bash
# Compilar y ejecutar
docker build -t healthpredict-api .
docker run -p 5000:80 healthpredict-api
```

### **5. Exponer con Cloudflare**
```bash
# Opción 1: URL temporal
cloudflared tunnel --url http://localhost:5000

# Opción 2: Con dominio propio
cloudflared tunnel run healthpredict
```

## 🌐 **Resultado**
Cloudflare te dará una URL como:
```
https://abc-def-ghi.trycloudflare.com → http://localhost:5000
```

## ⚡ **Ventajas vs ngrok**
- ✅ **Completamente GRATIS**
- ✅ **Sin límites de tiempo**
- ✅ **Mejor rendimiento**
- ✅ **Más seguro**
- ✅ **Dominio propio (opcional)**

## 🔧 **Script de Automatización**

### **start-with-cloudflare.bat**
```batch
@echo off
echo === HealthPredict API + Cloudflare Tunnel ===
echo.
echo 1. Iniciando Docker...
start "Docker API" cmd /k "docker build -t healthpredict-api . && docker run -p 5000:80 healthpredict-api"
echo.
echo 2. Esperando 15 segundos...
timeout /t 15 /nobreak
echo.
echo 3. Iniciando Cloudflare Tunnel...
start "Cloudflare" cmd /k "cloudflared tunnel --url http://localhost:5000"
echo.
echo ✅ API expuesta con Cloudflare!
echo La URL aparecerá en la ventana de Cloudflare
pause
``` 