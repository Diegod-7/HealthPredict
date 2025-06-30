# 🚀 DATOS DE PRUEBA - PANEL SUPERVISOR HealthPredict

## 📋 **Resumen de Funcionalidades a Probar**

El Panel Supervisor incluye:
- ✅ **Vista general de todos los trabajadores**
- ✅ **Alertas de riesgo en tiempo real**
- ✅ **Métricas de bienestar por departamento**
- ✅ **Reportes de tendencias de salud**
- ✅ **Identificación de trabajadores en riesgo**

---

## 🔧 **Pasos de Configuración**

### **1. Solucionar el problema de login**
Ejecuta primero el script de actualización de usuarios:
```sql
-- update_usuarios_simple.sql
```

### **2. Cargar datos de prueba principales**
Ejecuta el script completo de datos:
```sql
-- datos_prueba_panel_supervisor.sql
```

### **3. Agregar datos históricos (opcional)**
Para pruebas más completas de tendencias:
```sql
-- datos_historicos_tendencias.sql
```

---

## 👥 **Usuarios de Prueba Creados**

### **🔑 Credenciales de Login:**

| Usuario | Email | Password | Rol | Departamento | Estado Salud |
|---------|-------|----------|-----|--------------|--------------|
| **Carlos Rodriguez** | carlos.rodriguez@healthpredict.com | admin123 | **Jefe** | Administración | Estrés ejecutivo |
| **Diego Diaz** | diego.diaz@healthpredict.com | diego123 | Trabajador | Desarrollo | **⚠️ CRÍTICO** |
| **Matias Maripangue** | matias.maripangue@healthpredict.com | matias123 | Trabajador | Desarrollo | Riesgo moderado |
| **Iahn Vera** | iahn.vera@healthpredict.com | iahn123 | Trabajador | Desarrollo | ✅ Excelente |
| **Ana Martinez** | ana.martinez@healthpredict.com | ana123 | Trabajador | Marketing | Estrés moderado |
| **Luis Garcia** | luis.garcia@healthpredict.com | luis123 | Trabajador | Marketing | ✅ Buena condición |
| **Sofia Lopez** | sofia.lopez@healthpredict.com | sofia123 | Trabajador | RRHH | ✅ Excelente |
| **Pedro Ruiz** | pedro.ruiz@healthpredict.com | pedro123 | Trabajador | RRHH | Sobrepeso |

---

## 📊 **Escenarios de Prueba Configurados**

### **🚨 Escenario 1: Crisis de Salud (Diego)**
**Login:** diego.diaz@healthpredict.com / diego123

**Datos simulados:**
- Presión arterial: 165/108 mmHg (**CRÍTICA**)
- Frecuencia cardíaca: 115 bpm (**CRÍTICA**)
- Saturación oxígeno: 91% (**BAJA**)
- Horas de sueño: 2.8 horas (**CRÍTICA**)
- Nivel estrés: 10/10 (**MÁXIMO**)
- 5 alertas críticas sin leer

**Para probar:**
- Login como Carlos (jefe) debería ver alertas críticas
- Diego aparece como "trabajador en riesgo máximo"
- Tendencia negativa clara en gráficos

### **📈 Escenario 2: Tendencia Preocupante (Matias)**
**Login:** matias.maripangue@healthpredict.com / matias123

**Datos simulados:**
- Aumento gradual de presión: 125→145 mmHg
- Aumento de peso: +2.5 kg en 25 días
- Reducción actividad física: -30%
- Nivel estrés: incremento de 3→7

**Para probar:**
- Alertas de tendencia y monitoreo
- Gráficos de evolución temporal
- Comparación departamental

### **🏆 Escenario 3: Empleado Modelo (Iahn & Sofia)**
**Login:** iahn.vera@healthpredict.com / iahn123

**Datos simulados:**
- Todos los valores en rango óptimo
- Actividad física alta: 12,000+ pasos
- Sueño adecuado: 8+ horas
- Estrés bajo: 1-2/10
- Alertas positivas de felicitación

### **💼 Escenario 4: Estrés Ejecutivo (Carlos)**
**Login:** carlos.rodriguez@healthpredict.com / admin123

**Datos simulados:**
- Estrés ejecutivo: 9/10
- Sueño insuficiente: 4.2 horas
- Sedentarismo: 4,500 pasos/día
- Presión elevada: 155/102 mmHg

---

## 🎯 **Pruebas Específicas del Panel Supervisor**

### **Como Jefe (Carlos):**

1. **Dashboard Principal:**
   - Total trabajadores: 7
   - Alertas críticas: 3 (Diego)
   - Alertas pendientes: 6
   - Departamentos: 3 (Desarrollo, Marketing, RRHH)

2. **Vista de Trabajadores en Riesgo:**
   - Diego: CRÍTICO (múltiples alertas)
   - Matias: MEDIO (tendencias preocupantes)
   - Pedro: MEDIO (sobrepeso)

3. **Métricas por Departamento:**
   - **Desarrollo:** 3 trabajadores, estrés promedio alto
   - **Marketing:** 2 trabajadores, condición mixta
   - **RRHH:** 2 trabajadores, extremos (Sofia excelente, Pedro problemas)

4. **Alertas en Tiempo Real:**
   - 3 críticas sin leer (Diego)
   - 2 altas sin leer 
   - 3 medias (1 sin leer)
   - 4 bajas (todas leídas)

5. **Reportes de Tendencias:**
   - Diego: deterioro progresivo 60 días
   - Matias: empeoramiento gradual
   - Iahn: consistentemente excelente
   - Carlos: estrés ejecutivo creciente

---

## 📈 **Datos Estadísticos Esperados**

```
RESUMEN GENERAL:
- Total Usuarios: 8
- Total Datos Vitales: ~50+ registros
- Total Alertas: ~20+ alertas

POR CRITICIDAD:
- Críticas: 3 alertas (todas de Diego)
- Altas: 4 alertas 
- Medias: 7 alertas
- Bajas: 6+ alertas

POR DEPARTAMENTO:
- Desarrollo: 3 trabajadores (estrés promedio: 6.7)
- Marketing: 2 trabajadores (estrés promedio: 4.0)
- RRHH: 2 trabajadores (estrés promedio: 4.0)
```

---

## 🔍 **Queries de Verificación**

Después de ejecutar los scripts, verifica con:

```sql
-- 1. Resumen general
SELECT 
    (SELECT COUNT(*) FROM "USUARIOS") as usuarios,
    (SELECT COUNT(*) FROM "DATOS_VITALES") as datos_vitales,
    (SELECT COUNT(*) FROM "ALERTAS") as alertas;

-- 2. Trabajadores por departamento
SELECT "DEPARTAMENTO", COUNT(*) as trabajadores
FROM "USUARIOS" 
WHERE "ROL" = 'Trabajador'
GROUP BY "DEPARTAMENTO";

-- 3. Alertas críticas pendientes
SELECT u."NOMBRE", a."TIPO_ALERTA", a."NIVEL_CRITICIDAD"
FROM "ALERTAS" a
JOIN "USUARIOS" u ON a."USUARIO_ID" = u."ID"
WHERE a."NIVEL_CRITICIDAD" = 'Crítica' AND a."LEIDA" = false;
```

---

## 🎮 **Flujo de Pruebas Recomendado**

1. **Login como Carlos (Jefe)** - Ver dashboard supervisor completo
2. **Revisar alertas críticas** - Verificar alertas de Diego
3. **Analizar métricas departamentales** - Comparar 3 departamentos
4. **Ver trabajadores en riesgo** - Diego debe aparecer primero
5. **Revisar tendencias** - Gráficos de deterioro/mejora
6. **Login como Diego** - Ver dashboard personal con alertas
7. **Login como Iahn** - Ver dashboard de empleado saludable
8. **Regresar como Carlos** - Marcar alertas como leídas

---

## ⚡ **Ejecución Rápida**

```bash
# 1. Conectar a PostgreSQL
# 2. Ejecutar en orden:
```

```sql
-- update_usuarios_simple.sql (corrige login)
-- datos_prueba_panel_supervisor.sql (datos principales)  
-- datos_historicos_tendencias.sql (opcional - más datos)
```

```bash
# 3. Login en la aplicación como Carlos
# Email: carlos.rodriguez@healthpredict.com
# Password: admin123
```

¡Listo para probar el Panel Supervisor completo! 🚀 