# 🚀 Inicio Rápido - API Local Online

## 🎯 **¿Qué vamos a hacer?**
Exponer tu API HealthPredict local para que sea accesible desde internet, evitando los problemas de Oracle Cloud + Render.

## 📋 **Requisitos**
- ✅ Docker instalado
- ✅ Tu base de datos Oracle Cloud funcionando
- ✅ Una cuenta en ngrok.com O cloudflare.com

## ⚡ **Opción 1: ngrok (Más Fácil)**

### **Instalación ngrok:**
1. **Ir a:** [ngrok.com/download](https://ngrok.com/download)
2. **Descargar** ngrok para Windows
3. **Crear cuenta** y obtener tu token
4. **Extraer** `ngrok.exe` a `C:\ngrok\` (o donde quieras)
5. **Agregar a PATH** (opcional pero recomendado)

### **Configurar ngrok:**
```bash
# Autenticar (reemplaza con tu token real)
ngrok config add-authtoken tu_token_aqui
```

### **Ejecutar:**
```bash
# Doble clic en el archivo
start-api-ngrok.bat
```

## ☁️ **Opción 2: Cloudflare Tunnel (Más Profesional)**

### **Instalación Cloudflare:**
```bash
# Windows 11 con winget
winget install cloudflare.cloudflared

# O descargar desde: https://github.com/cloudflare/cloudflared/releases
```

### **Ejecutar:**
```bash
# Doble clic en el archivo
start-api-cloudflare.bat
```

## 🌐 **Resultado**
Obtendrás una URL como:
- **ngrok:** `https://abc123.ngrok.io`
- **Cloudflare:** `https://abc-def.trycloudflare.com`

## 📱 **Usar en tus Apps**

### **Angular:**
```typescript
// src/environments/environment.ts
export const environment = {
  production: false,
  apiUrl: 'https://abc123.ngrok.io/api'  // ← Reemplaza con tu URL
};
```

### **Android:**
```kotlin
// En tu clase de configuración
const val BASE_URL = "https://abc123.ngrok.io/api/"
```

### **iOS:**
```swift
// En tu APIService
let baseURL = "https://abc123.ngrok.io/api"
```

## 🔍 **Verificar que Funciona**
1. **Abrir:** `https://tu-url.ngrok.io/swagger`
2. **Probar:** `GET /api/DatosVitales` → Debe devolver `[]`
3. **Ver logs** en la ventana de Docker

## ⚡ **Ventajas de esta Solución**
- ✅ **Oracle funciona** (conexión local)
- ✅ **API accesible online** (ngrok/Cloudflare)
- ✅ **Sin problemas de deploy** (todo local)
- ✅ **Fácil de reiniciar** (solo ejecutar el .bat)
- ✅ **Gratis** (con limitaciones en ngrok)

## 🚨 **Limitaciones**
- 🔄 **ngrok gratis:** URL cambia cada reinicio
- ⏱️ **ngrok gratis:** Sesiones de 2 horas
- 💻 **Depende de tu PC:** Debe estar encendida

## 💰 **Upgrade Opciones**
- **ngrok Pro:** $8/mes → URL fija + sin límites
- **Cloudflare:** Gratis siempre
- **VPS:** $5/mes → Control total

## 🔧 **Troubleshooting**
- **Error Docker:** Verificar que Docker Desktop esté corriendo
- **Error ngrok:** Verificar token de autenticación
- **Error 500:** Verificar logs de Oracle en Docker
- **No conecta:** Verificar firewall/antivirus 