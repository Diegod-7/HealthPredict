# 🐛 Guía de Debugging Android - HealthPredict

## 🎯 **Cambios Implementados**

### **✅ Backend (.NET) - Logging Mejorado:**
- ✅ Logging detallado en `/api/DatosVitales/Sync/HealthKit`
- ✅ Validación exhaustiva de datos recibidos
- ✅ Mensajes de error específicos con timestamps
- ✅ Stack traces completos en errores
- ✅ Cambio de `Apple Health` a `Android Health` en origen

### **✅ Android App - Error Handling Mejorado:**
- ✅ Logging detallado en `HealthRepository`
- ✅ Mapeo correcto de tipos Android → HealthKit
- ✅ Modelo de datos actualizado (`HealthKitDataRequest`)
- ✅ Respuestas de error parseadas y mostradas
- ✅ UI con mensajes de error detallados

## 📱 **Cómo Ver los Logs en Android Studio**

### **1. Abrir Logcat:**
```
Android Studio → View → Tool Windows → Logcat
```

### **2. Filtros Importantes:**
```
Tag: HealthRepository
Tag: MainViewModel
Search: "ANDROID DEBUG"
```

### **3. Logs que Verás:**
```
🔍 [ANDROID DEBUG] Iniciando sincronización para usuario: 1
🔍 [ANDROID DEBUG] Última sincronización: null
🔍 [ANDROID DEBUG] Obteniendo datos desde: 2024-01-15 10:30:00
🔍 [ANDROID DEBUG] Datos obtenidos: 5
🔍 [ANDROID DEBUG] Dato 0: tipo=HEART_RATE, valor=75.0, unidad=bpm
🔍 [ANDROID DEBUG] Enviando 5 datos al servidor...
🔍 [ANDROID DEBUG] URL: https://healthpredict-l1hu.onrender.com/api/DatosVitales/Sync/HealthKit
```

## 🌐 **Cómo Ver los Logs del Backend en Render**

### **1. Ir a Render Dashboard:**
```
https://dashboard.render.com
→ Tu servicio HealthPredict
→ Logs tab
```

### **2. Buscar Logs de Android:**
```
Filtrar por: "[ANDROID DEBUG]"
```

### **3. Logs que Verás:**
```
🔍 [ANDROID DEBUG] Recibiendo datos de sincronización...
🔍 [ANDROID DEBUG] Cantidad de datos recibidos: 5
🔍 [ANDROID DEBUG] Procesando dato 1: Usuario=1, Tipo=HKQuantityTypeIdentifierHeartRate, Valor=75
✅ [ANDROID DEBUG] Guardando 5 datos válidos en la base de datos...
✅ [ANDROID DEBUG] Sincronización completada: 5 datos guardados, 0 alertas generadas
```

## 🔧 **Solución de Problemas Comunes**

### **❌ Error 400 - Bad Request:**

#### **Posibles Causas:**
1. **Datos vacíos o null**
2. **UsuarioId inválido (≤ 0)**
3. **TipoHealthKit vacío**
4. **Unidad vacía**
5. **Formato de fecha incorrecto**

#### **Cómo Verificar:**
```
1. Ver logs de Android: "Datos obtenidos: X"
2. Ver logs de backend: "Cantidad de datos recibidos: X"
3. Verificar errores de validación en backend
```

### **❌ Error de Conexión:**

#### **Verificar:**
```
1. Internet conectado
2. URL correcta: https://healthpredict-l1hu.onrender.com/
3. Render service activo
4. Firewall/antivirus no bloqueando
```

### **❌ Error de Parsing JSON:**

#### **Verificar:**
```
1. Modelo HealthKitDataRequest correcto
2. Fechas en formato ISO 8601
3. Tipos de datos mapeados correctamente
```

## 📊 **Tipos de Datos Soportados**

### **Mapeo Android → Backend:**
```
Android Type              → HealthKit Type
"HEART_RATE"             → "HKQuantityTypeIdentifierHeartRate"
"STEPS"                  → "HKQuantityTypeIdentifierStepCount"
"WEIGHT"                 → "HKQuantityTypeIdentifierBodyMass"
"HEIGHT"                 → "HKQuantityTypeIdentifierHeight"
"BLOOD_PRESSURE_SYSTOLIC" → "HKQuantityTypeIdentifierBloodPressureSystolic"
"BLOOD_PRESSURE_DIASTOLIC"→ "HKQuantityTypeIdentifierBloodPressureDiastolic"
"BODY_TEMPERATURE"       → "HKQuantityTypeIdentifierBodyTemperature"
"OXYGEN_SATURATION"      → "HKQuantityTypeIdentifierOxygenSaturation"
"CALORIES_BURNED"        → "HKQuantityTypeIdentifierActiveEnergyBurned"
"DISTANCE"               → "HKQuantityTypeIdentifierDistanceWalkingRunning"
"SLEEP_DURATION"         → "HKCategoryTypeIdentifierSleepAnalysis"
```

## 🧪 **Pasos para Probar**

### **1. Compilar App Android:**
```
Android Studio → Build → Make Project
```

### **2. Ejecutar en Dispositivo/Emulador:**
```
Run → Run 'app'
```

### **3. Probar Sincronización:**
```
1. Abrir app
2. Presionar botón "Sincronizar"
3. Ver logs en Logcat
4. Ver resultado en UI
```

### **4. Verificar Backend:**
```
1. Ir a Render logs
2. Buscar "[ANDROID DEBUG]"
3. Verificar datos recibidos
```

## 📱 **Interfaz de Usuario Mejorada**

### **Mensajes de Éxito:**
```
✅ Sincronización exitosa!
📊 Datos insertados: 5
🚨 Alertas generadas: 0
⏰ 2024-01-22 15:30:45
```

### **Mensajes de Error:**
```
❌ ERROR DE SINCRONIZACIÓN

🕐 Timestamp: 2024-01-22 15:30:45

📋 Detalle del error:
Error 400: Errores de validación en los datos
Detalle: ["Dato 1: UsuarioId inválido (0)"]

🔧 Posibles soluciones:
• Verificar conexión a internet
• Reintentar en unos segundos
• Verificar permisos de Health Connect

🌐 Servidor: https://healthpredict-l1hu.onrender.com/
```

## 🎯 **Próximos Pasos**

### **1. Compilar y Probar:**
```bash
# En Android Studio:
Build → Make Project
Run → Run 'app'
```

### **2. Verificar Logs:**
```bash
# Android:
Logcat → Filter: "ANDROID DEBUG"

# Backend:
Render Dashboard → Logs → Filter: "[ANDROID DEBUG]"
```

### **3. Reportar Resultados:**
```
Compartir logs específicos si hay errores:
- Screenshot de error en app
- Logs de Logcat
- Logs de Render
```

---

## 🎉 **¡Debugging Completo Implementado!**

Ahora tienes **visibilidad total** de lo que pasa en tu app Android:
- ✅ **Logs detallados** en cada paso
- ✅ **Errores específicos** con soluciones
- ✅ **Validación exhaustiva** de datos
- ✅ **Interfaz informativa** para el usuario

**¡No más errores 400 misteriosos!** 🚀 