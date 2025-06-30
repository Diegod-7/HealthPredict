# 🚀 Guía Oracle Cloud + Render - Paso a Paso

## 📋 **Checklist de Configuración**

### **1. Oracle Cloud Console**

1. **Ve a Oracle Cloud Console** → **Database** → **Autonomous Database**
2. **Selecciona `healthpredict`**
3. **Network** → **Access Control List**
4. **Cambiar a "Secure access from everywhere"** ✅
5. **Guardar cambios**

### **2. Render Dashboard**

1. **Ve a tu servicio en Render**
2. **Environment** → **Add Environment Variable**
3. **Agregar:**
   ```
   Key: ORACLE_CONNECTION_STRING
   Value: [usar uno de los formatos de abajo]
   ```

### **3. Connection Strings para Probar**

#### **🎯 Formato 1 (Recomendado - TP Service):**
```
Data Source=adb.sa-saopaulo-1.oraclecloud.com:1522/g993e4289ace002_healthpredict_tp.adb.oraclecloud.com;User Id=ADMIN;Password=y4mti4edyxEWzwU;SSL Mode=Require;
```

#### **🔄 Formato 2 (Alternativo - LOW Service):**
```
Data Source=adb.sa-saopaulo-1.oraclecloud.com:1522/g993e4289ace002_healthpredict_low.adb.oraclecloud.com;User Id=ADMIN;Password=y4mti4edyxEWzwU;SSL Mode=Require;
```

#### **🛠️ Formato 3 (TNS sin SSL verification):**
```
Data Source=(description= (retry_count=20)(retry_delay=3)(address=(protocol=tcps)(port=1522)(host=adb.sa-saopaulo-1.oraclecloud.com))(connect_data=(service_name=g993e4289ace002_healthpredict_tp.adb.oraclecloud.com)));User Id=ADMIN;Password=y4mti4edyxEWzwU;
```

## 🔍 **Verificación**

### **Logs en Render:**
1. **Deploy** → **View Logs**
2. **Buscar mensajes:**
   - ✅ `Connection String configurado correctamente`
   - ❌ `ERROR: No se encontró el string de conexión`

### **Probar Endpoints:**
1. `GET /api/DatosVitales` → Debe devolver `[]` (no error 500)
2. `GET /swagger` → Debe cargar correctamente

## 🚨 **Troubleshooting**

### **Si sigue dando Error 500:**

1. **Verificar Oracle Cloud ACL:**
   - Debe estar en "Secure access from everywhere"
   - No debe tener restricciones de IP

2. **Probar en orden:**
   - Formato 1 (TP Service)
   - Formato 2 (LOW Service) 
   - Formato 3 (TNS sin SSL)

3. **Verificar logs de Render:**
   - Buscar mensajes de Oracle
   - Verificar que el connection string se carga

### **Comandos de Debug:**

```bash
# Ver variables de entorno en Render
echo $ORACLE_CONNECTION_STRING

# Verificar conectividad desde Render
ping adb.sa-saopaulo-1.oraclecloud.com
```

## 📞 **Contacto Oracle Support**

Si ningún formato funciona:
1. **Oracle Cloud Console** → **Support** → **Create Service Request**
2. **Tipo:** Autonomous Database
3. **Problema:** Connection from external service (Render.com)
4. **Incluir:** Service names y host information

## ✅ **Éxito**

Cuando funcione verás:
- ✅ Logs: "Connection String configurado correctamente"
- ✅ `/api/DatosVitales` devuelve `[]` sin error
- ✅ Swagger carga completamente
- ✅ No hay errores 500 en endpoints de base de datos 