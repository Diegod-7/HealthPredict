# 🚀 Oracle XE en Railway - Base de Datos Gratis

## 📋 **¿Qué es Railway?**
Railway es una plataforma que permite desplegar bases de datos en contenedores Docker de forma gratuita. Perfecto para Oracle XE.

## 💰 **Costos**
- **Gratis:** $5 USD de crédito mensual
- **Oracle XE:** Consume ~$3-4 USD/mes
- **Resultado:** Prácticamente gratis

## 🛠️ **Configuración Paso a Paso**

### **1. Crear Cuenta en Railway**
1. **Ir a:** [railway.app](https://railway.app)
2. **Sign up** con GitHub
3. **Verificar** cuenta

### **2. Crear Servicio Oracle XE**
1. **New Project** → **Deploy from Docker Image**
2. **Docker Image:** `gvenzl/oracle-xe:21-slim`
3. **Service Name:** `oracle-healthpredict`

### **3. Configurar Variables de Entorno**
```bash
ORACLE_PASSWORD=HealthPredict2024
ORACLE_DATABASE=HEALTHPREDICTDB
APP_USER=healthuser
APP_USER_PASSWORD=healthpass123
```

### **4. Configurar Puerto**
- **Port:** `1521`
- **Protocolo:** TCP

### **5. Obtener Connection String**
Después del deploy, Railway te dará:
```
Host: oracle-healthpredict-production.up.railway.app
Port: 12345 (puerto público asignado)
Database: HEALTHPREDICTDB
User: healthuser
Password: healthpass123
```

### **6. Connection String Final**
```
Data Source=oracle-healthpredict-production.up.railway.app:12345/HEALTHPREDICTDB;User Id=healthuser;Password=healthpass123;
```

## ✅ **Ventajas**
- ✅ **Gratis** (con límites generosos)
- ✅ **Oracle XE completo**
- ✅ **No requiere tarjeta de crédito**
- ✅ **Fácil de configurar**
- ✅ **Acceso desde cualquier lugar**

## 🚨 **Limitaciones**
- 🔄 **Límite mensual:** $5 USD de crédito
- 💾 **Storage:** Limitado por el plan gratuito
- 🚦 **Conexiones:** Limitadas pero suficientes para desarrollo 