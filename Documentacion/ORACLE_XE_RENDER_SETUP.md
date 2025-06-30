# 🐳 Oracle XE en Render - Usando Docker

## 📋 **¿Cómo funciona?**
Render permite desplegar servicios Docker, incluyendo Oracle XE. Aunque no es la opción más común, es posible.

## 💰 **Costos**
- **Render:** $7 USD/mes por servicio Docker
- **Resultado:** Económico para producción

## 🛠️ **Configuración**

### **1. Crear Dockerfile para Oracle XE**
```dockerfile
FROM gvenzl/oracle-xe:21-slim

# Variables de entorno
ENV ORACLE_PASSWORD=HealthPredict2024
ENV ORACLE_DATABASE=HEALTHPREDICTDB
ENV APP_USER=healthuser  
ENV APP_USER_PASSWORD=healthpass123

# Exponer puerto
EXPOSE 1521

# Comando de inicio
CMD ["oracle-xe"]
```

### **2. Crear en Render**
1. **New** → **Web Service**
2. **Connect Repository** (tu repo con el Dockerfile)
3. **Docker** como Build Command
4. **Port:** 1521

### **3. Configurar Variables de Entorno**
```bash
ORACLE_PASSWORD=HealthPredict2024
ORACLE_DATABASE=HEALTHPREDICTDB
APP_USER=healthuser
APP_USER_PASSWORD=healthpass123
```

## ⚠️ **Limitaciones**
- 💰 **No es gratis** ($7/mes)
- 🔧 **Más complejo** de configurar
- 📊 **Recursos limitados** en plan básico

## 🤔 **¿Vale la Pena?**
Para Oracle específicamente, **Railway es mejor opción** por ser más barato y fácil. 