-- DATOS DE PRUEBA CORREGIDOS PARA PANEL SUPERVISOR
-- Usando la estructura real de DATOS_VITALES (TIPO_DATO, VALOR, UNIDAD)

-- =====================================================
-- 1. DATOS VITALES VARIADOS (últimos 30 días)
-- =====================================================

-- Diego (ID 7) - Trabajador con deterioro progresivo
-- Semana 1 - Valores normales
INSERT INTO "DATOS_VITALES" ("USUARIO_ID", "FECHA_REGISTRO", "TIPO_DATO", "VALOR", "UNIDAD", "DISPOSITIVO_ORIGEN", "NOTAS") VALUES
(7, '2025-01-01 08:30:00', 'Frecuencia Cardíaca', 72, 'bpm', 'Apple Watch', 'Medición matutina'),
(7, '2025-01-01 08:30:00', 'Presión Sistólica', 120, 'mmHg', 'Monitor Omron', 'Medición matutina'),
(7, '2025-01-01 08:30:00', 'Presión Diastólica', 80, 'mmHg', 'Monitor Omron', 'Medición matutina'),
(7, '2025-01-01 08:30:00', 'Temperatura', 36.5, '°C', 'Termómetro Digital', 'Temperatura normal'),
(7, '2025-01-01 08:30:00', 'Peso', 75.2, 'kg', 'Báscula Digital', 'Peso estable'),
(7, '2025-01-01 08:30:00', 'Pasos Diarios', 8500, 'pasos', 'Apple Watch', 'Actividad normal'),
(7, '2025-01-01 08:30:00', 'Horas de Sueño', 7.5, 'horas', 'Apple Watch', 'Sueño reparador'),
(7, '2025-01-01 08:30:00', 'Nivel de Estrés', 3, 'escala 1-10', 'Autoevaluación', 'Estrés bajo'),
(7, '2025-01-01 08:30:00', 'Saturación Oxígeno', 98, '%', 'Oxímetro', 'Normal'),

-- Semana 2 - Inicio de problemas
(7, '2025-01-08 08:00:00', 'Frecuencia Cardíaca', 85, 'bpm', 'Apple Watch', 'Incremento notable'),
(7, '2025-01-08 08:00:00', 'Presión Sistólica', 135, 'mmHg', 'Monitor Omron', 'Presión elevándose'),
(7, '2025-01-08 08:00:00', 'Presión Diastólica', 88, 'mmHg', 'Monitor Omron', 'Presión elevándose'),
(7, '2025-01-08 08:00:00', 'Temperatura', 37.1, '°C', 'Termómetro Digital', 'Ligera fiebre'),
(7, '2025-01-08 08:00:00', 'Peso', 75.5, 'kg', 'Báscula Digital', 'Aumento ligero'),
(7, '2025-01-08 08:00:00', 'Pasos Diarios', 6500, 'pasos', 'Apple Watch', 'Reducción actividad'),
(7, '2025-01-08 08:00:00', 'Horas de Sueño', 5.5, 'horas', 'Apple Watch', 'Sueño insuficiente'),
(7, '2025-01-08 08:00:00', 'Nivel de Estrés', 6, 'escala 1-10', 'Autoevaluación', 'Estrés aumentando'),
(7, '2025-01-08 08:00:00', 'Saturación Oxígeno', 96, '%', 'Oxímetro', 'Ligera reducción'),

-- Crisis actual (datos más recientes)
(7, '2025-01-26 07:30:00', 'Frecuencia Cardíaca', 115, 'bpm', 'Apple Watch', 'TAQUICARDIA - Crítico'),
(7, '2025-01-26 07:30:00', 'Presión Sistólica', 165, 'mmHg', 'Monitor Omron', 'HIPERTENSIÓN - Crítico'),
(7, '2025-01-26 07:30:00', 'Presión Diastólica', 108, 'mmHg', 'Monitor Omron', 'HIPERTENSIÓN - Crítico'),
(7, '2025-01-26 07:30:00', 'Temperatura', 38.8, '°C', 'Termómetro Digital', 'Fiebre alta'),
(7, '2025-01-26 07:30:00', 'Peso', 77.5, 'kg', 'Báscula Digital', 'Aumento preocupante'),
(7, '2025-01-26 07:30:00', 'Pasos Diarios', 2100, 'pasos', 'Apple Watch', 'SEDENTARISMO - Crítico'),
(7, '2025-01-26 07:30:00', 'Horas de Sueño', 2.8, 'horas', 'Apple Watch', 'INSOMNIO - Crítico'),
(7, '2025-01-26 07:30:00', 'Nivel de Estrés', 10, 'escala 1-10', 'Autoevaluación', 'ESTRÉS MÁXIMO - Crítico'),
(7, '2025-01-26 07:30:00', 'Saturación Oxígeno', 91, '%', 'Oxímetro', 'HIPOXEMIA - Crítico');

-- Matias (ID 8) - Tendencia preocupante gradual
INSERT INTO "DATOS_VITALES" ("USUARIO_ID", "FECHA_REGISTRO", "TIPO_DATO", "VALOR", "UNIDAD", "DISPOSITIVO_ORIGEN", "NOTAS") VALUES
-- Datos de enero (progresión)
(8, '2025-01-01 09:00:00', 'Frecuencia Cardíaca', 70, 'bpm', 'Fitbit', 'Normal'),
(8, '2025-01-01 09:00:00', 'Presión Sistólica', 125, 'mmHg', 'Monitor Digital', 'Normal'),
(8, '2025-01-01 09:00:00', 'Presión Diastólica', 85, 'mmHg', 'Monitor Digital', 'Normal'),
(8, '2025-01-01 09:00:00', 'Peso', 82.0, 'kg', 'Báscula', 'Peso base'),
(8, '2025-01-01 09:00:00', 'Pasos Diarios', 9500, 'pasos', 'Fitbit', 'Buena actividad'),
(8, '2025-01-01 09:00:00', 'Horas de Sueño', 7.8, 'horas', 'Fitbit', 'Sueño adecuado'),
(8, '2025-01-01 09:00:00', 'Nivel de Estrés', 3, 'escala 1-10', 'Autoevaluación', 'Bajo'),

-- Datos recientes (empeoramiento)
(8, '2025-01-26 09:00:00', 'Frecuencia Cardíaca', 87, 'bpm', 'Fitbit', 'Incremento gradual'),
(8, '2025-01-26 09:00:00', 'Presión Sistólica', 145, 'mmHg', 'Monitor Digital', 'Tendencia al alza'),
(8, '2025-01-26 09:00:00', 'Presión Diastólica', 97, 'mmHg', 'Monitor Digital', 'Tendencia al alza'),
(8, '2025-01-26 09:00:00', 'Peso', 84.5, 'kg', 'Báscula', 'Aumento de 2.5kg'),
(8, '2025-01-26 09:00:00', 'Pasos Diarios', 6500, 'pasos', 'Fitbit', 'Reducción del 30%'),
(8, '2025-01-26 09:00:00', 'Horas de Sueño', 5.5, 'horas', 'Fitbit', 'Sueño insuficiente'),
(8, '2025-01-26 09:00:00', 'Nivel de Estrés', 7, 'escala 1-10', 'Autoevaluación', 'Estrés alto');

-- Iahn (ID 9) - Trabajador modelo (consistentemente saludable)
INSERT INTO "DATOS_VITALES" ("USUARIO_ID", "FECHA_REGISTRO", "TIPO_DATO", "VALOR", "UNIDAD", "DISPOSITIVO_ORIGEN", "NOTAS") VALUES
(9, '2025-01-26 07:40:00', 'Frecuencia Cardíaca', 63, 'bpm', 'Garmin', 'Excelente condición'),
(9, '2025-01-26 07:40:00', 'Presión Sistólica', 116, 'mmHg', 'Monitor Manual', 'Óptima'),
(9, '2025-01-26 07:40:00', 'Presión Diastólica', 76, 'mmHg', 'Monitor Manual', 'Óptima'),
(9, '2025-01-26 07:40:00', 'Temperatura', 36.3, '°C', 'Termómetro', 'Normal'),
(9, '2025-01-26 07:40:00', 'Peso', 69.0, 'kg', 'Báscula Smart', 'Peso ideal'),
(9, '2025-01-26 07:40:00', 'Pasos Diarios', 12100, 'pasos', 'Garmin', 'Superó meta diaria'),
(9, '2025-01-26 07:40:00', 'Horas de Sueño', 8.4, 'horas', 'Garmin', 'Sueño reparador'),
(9, '2025-01-26 07:40:00', 'Nivel de Estrés', 1, 'escala 1-10', 'Autoevaluación', 'Muy relajado'),
(9, '2025-01-26 07:40:00', 'Saturación Oxígeno', 99, '%', 'Oxímetro', 'Excelente');

-- Carlos (ID 6) - Jefe con estrés ejecutivo
INSERT INTO "DATOS_VITALES" ("USUARIO_ID", "FECHA_REGISTRO", "TIPO_DATO", "VALOR", "UNIDAD", "DISPOSITIVO_ORIGEN", "NOTAS") VALUES
(6, '2025-01-26 10:00:00', 'Frecuencia Cardíaca', 98, 'bpm', 'Samsung Watch', 'Estrés ejecutivo'),
(6, '2025-01-26 10:00:00', 'Presión Sistólica', 155, 'mmHg', 'Monitor Automático', 'Hipertensión ejecutiva'),
(6, '2025-01-26 10:00:00', 'Presión Diastólica', 102, 'mmHg', 'Monitor Automático', 'Hipertensión ejecutiva'),
(6, '2025-01-26 10:00:00', 'Temperatura', 37.3, '°C', 'Termómetro', 'Ligera elevación'),
(6, '2025-01-26 10:00:00', 'Peso', 82.5, 'kg', 'Báscula', 'Aumento por sedentarismo'),
(6, '2025-01-26 10:00:00', 'Pasos Diarios', 4500, 'pasos', 'Samsung Watch', 'Muy sedentario'),
(6, '2025-01-26 10:00:00', 'Horas de Sueño', 4.2, 'horas', 'Samsung Watch', 'Insomnio ejecutivo'),
(6, '2025-01-26 10:00:00', 'Nivel de Estrés', 9, 'escala 1-10', 'Autoevaluación', 'Estrés muy alto'),
(6, '2025-01-26 10:00:00', 'Saturación Oxígeno', 94, '%', 'Oxímetro', 'Ligeramente baja');

-- =====================================================
-- 2. USUARIOS ADICIONALES PARA DEPARTAMENTOS
-- =====================================================

-- Crear usuarios adicionales para métricas departamentales
INSERT INTO "USUARIOS" ("NOMBRE", "APELLIDO", "EMAIL", "PASSWORD", "FECHA_NACIMIENTO", "GENERO", "ALTURA", "PESO", "FECHA_REGISTRO", "ULTIMO_ACCESO", "ES_PROFESIONAL_MEDICO", "ROL", "DEPARTAMENTO", "CARGO", "JEFE_ID", "ES_ACTIVO") VALUES
('Ana', 'Martinez', 'ana.martinez@healthpredict.com', 'ana123', '1990-05-15', 'Femenino', 165, 58.0, NOW(), NOW(), false, 'Trabajador', 'Marketing', 'Especialista Marketing', 6, true),
('Luis', 'Garcia', 'luis.garcia@healthpredict.com', 'luis123', '1988-11-20', 'Masculino', 175, 72.0, NOW(), NOW(), false, 'Trabajador', 'Marketing', 'Analista Digital', 6, true),
('Sofia', 'Lopez', 'sofia.lopez@healthpredict.com', 'sofia123', '1992-02-10', 'Femenino', 160, 55.0, NOW(), NOW(), false, 'Trabajador', 'RRHH', 'Especialista RRHH', 6, true),
('Pedro', 'Ruiz', 'pedro.ruiz@healthpredict.com', 'pedro123', '1985-08-30', 'Masculino', 180, 85.0, NOW(), NOW(), false, 'Trabajador', 'RRHH', 'Coordinador Bienestar', 6, true);

-- Datos vitales para usuarios adicionales
-- Ana (Marketing) - Estrés moderado
INSERT INTO "DATOS_VITALES" ("USUARIO_ID", "FECHA_REGISTRO", "TIPO_DATO", "VALOR", "UNIDAD", "DISPOSITIVO_ORIGEN", "NOTAS") VALUES
((SELECT "ID" FROM "USUARIOS" WHERE "EMAIL" = 'ana.martinez@healthpredict.com'), '2025-01-26 09:30:00', 'Frecuencia Cardíaca', 75, 'bpm', 'Apple Watch', 'Normal con estrés'),
((SELECT "ID" FROM "USUARIOS" WHERE "EMAIL" = 'ana.martinez@healthpredict.com'), '2025-01-26 09:30:00', 'Presión Sistólica', 125, 'mmHg', 'Monitor', 'Límite normal'),
((SELECT "ID" FROM "USUARIOS" WHERE "EMAIL" = 'ana.martinez@healthpredict.com'), '2025-01-26 09:30:00', 'Presión Diastólica', 82, 'mmHg', 'Monitor', 'Límite normal'),
((SELECT "ID" FROM "USUARIOS" WHERE "EMAIL" = 'ana.martinez@healthpredict.com'), '2025-01-26 09:30:00', 'Peso', 58.2, 'kg', 'Báscula', 'Estable'),
((SELECT "ID" FROM "USUARIOS" WHERE "EMAIL" = 'ana.martinez@healthpredict.com'), '2025-01-26 09:30:00', 'Pasos Diarios', 8500, 'pasos', 'Apple Watch', 'Actividad moderada'),
((SELECT "ID" FROM "USUARIOS" WHERE "EMAIL" = 'ana.martinez@healthpredict.com'), '2025-01-26 09:30:00', 'Horas de Sueño', 7.0, 'horas', 'Apple Watch', 'Sueño suficiente'),
((SELECT "ID" FROM "USUARIOS" WHERE "EMAIL" = 'ana.martinez@healthpredict.com'), '2025-01-26 09:30:00', 'Nivel de Estrés', 5, 'escala 1-10', 'Autoevaluación', 'Estrés moderado');

-- Luis (Marketing) - Buena condición
INSERT INTO "DATOS_VITALES" ("USUARIO_ID", "FECHA_REGISTRO", "TIPO_DATO", "VALOR", "UNIDAD", "DISPOSITIVO_ORIGEN", "NOTAS") VALUES
((SELECT "ID" FROM "USUARIOS" WHERE "EMAIL" = 'luis.garcia@healthpredict.com'), '2025-01-26 08:45:00', 'Frecuencia Cardíaca', 68, 'bpm', 'Fitbit', 'Excelente'),
((SELECT "ID" FROM "USUARIOS" WHERE "EMAIL" = 'luis.garcia@healthpredict.com'), '2025-01-26 08:45:00', 'Presión Sistólica', 118, 'mmHg', 'Monitor', 'Óptima'),
((SELECT "ID" FROM "USUARIOS" WHERE "EMAIL" = 'luis.garcia@healthpredict.com'), '2025-01-26 08:45:00', 'Presión Diastólica', 78, 'mmHg', 'Monitor', 'Óptima'),
((SELECT "ID" FROM "USUARIOS" WHERE "EMAIL" = 'luis.garcia@healthpredict.com'), '2025-01-26 08:45:00', 'Peso', 72.5, 'kg', 'Báscula', 'Peso ideal'),
((SELECT "ID" FROM "USUARIOS" WHERE "EMAIL" = 'luis.garcia@healthpredict.com'), '2025-01-26 08:45:00', 'Pasos Diarios', 10200, 'pasos', 'Fitbit', 'Muy activo'),
((SELECT "ID" FROM "USUARIOS" WHERE "EMAIL" = 'luis.garcia@healthpredict.com'), '2025-01-26 08:45:00', 'Horas de Sueño', 8.2, 'horas', 'Fitbit', 'Sueño reparador'),
((SELECT "ID" FROM "USUARIOS" WHERE "EMAIL" = 'luis.garcia@healthpredict.com'), '2025-01-26 08:45:00', 'Nivel de Estrés', 3, 'escala 1-10', 'Autoevaluación', 'Relajado');

-- Sofia (RRHH) - Excelente condición
INSERT INTO "DATOS_VITALES" ("USUARIO_ID", "FECHA_REGISTRO", "TIPO_DATO", "VALOR", "UNIDAD", "DISPOSITIVO_ORIGEN", "NOTAS") VALUES
((SELECT "ID" FROM "USUARIOS" WHERE "EMAIL" = 'sofia.lopez@healthpredict.com'), '2025-01-26 09:15:00', 'Frecuencia Cardíaca', 65, 'bpm', 'Garmin', 'Atlética'),
((SELECT "ID" FROM "USUARIOS" WHERE "EMAIL" = 'sofia.lopez@healthpredict.com'), '2025-01-26 09:15:00', 'Presión Sistólica', 115, 'mmHg', 'Monitor', 'Perfecta'),
((SELECT "ID" FROM "USUARIOS" WHERE "EMAIL" = 'sofia.lopez@healthpredict.com'), '2025-01-26 09:15:00', 'Presión Diastólica', 75, 'mmHg', 'Monitor', 'Perfecta'),
((SELECT "ID" FROM "USUARIOS" WHERE "EMAIL" = 'sofia.lopez@healthpredict.com'), '2025-01-26 09:15:00', 'Peso', 55.5, 'kg', 'Báscula', 'Peso perfecto'),
((SELECT "ID" FROM "USUARIOS" WHERE "EMAIL" = 'sofia.lopez@healthpredict.com'), '2025-01-26 09:15:00', 'Pasos Diarios', 11500, 'pasos', 'Garmin', 'Muy activa'),
((SELECT "ID" FROM "USUARIOS" WHERE "EMAIL" = 'sofia.lopez@healthpredict.com'), '2025-01-26 09:15:00', 'Horas de Sueño', 8.5, 'horas', 'Garmin', 'Sueño perfecto'),
((SELECT "ID" FROM "USUARIOS" WHERE "EMAIL" = 'sofia.lopez@healthpredict.com'), '2025-01-26 09:15:00', 'Nivel de Estrés', 2, 'escala 1-10', 'Autoevaluación', 'Muy relajada');

-- Pedro (RRHH) - Problemas de peso
INSERT INTO "DATOS_VITALES" ("USUARIO_ID", "FECHA_REGISTRO", "TIPO_DATO", "VALOR", "UNIDAD", "DISPOSITIVO_ORIGEN", "NOTAS") VALUES
((SELECT "ID" FROM "USUARIOS" WHERE "EMAIL" = 'pedro.ruiz@healthpredict.com'), '2025-01-26 10:30:00', 'Frecuencia Cardíaca', 88, 'bpm', 'Apple Watch', 'Elevada por sobrepeso'),
((SELECT "ID" FROM "USUARIOS" WHERE "EMAIL" = 'pedro.ruiz@healthpredict.com'), '2025-01-26 10:30:00', 'Presión Sistólica', 145, 'mmHg', 'Monitor', 'Hipertensión leve'),
((SELECT "ID" FROM "USUARIOS" WHERE "EMAIL" = 'pedro.ruiz@healthpredict.com'), '2025-01-26 10:30:00', 'Presión Diastólica', 95, 'mmHg', 'Monitor', 'Hipertensión leve'),
((SELECT "ID" FROM "USUARIOS" WHERE "EMAIL" = 'pedro.ruiz@healthpredict.com'), '2025-01-26 10:30:00', 'Peso', 87.2, 'kg', 'Báscula', 'Sobrepeso'),
((SELECT "ID" FROM "USUARIOS" WHERE "EMAIL" = 'pedro.ruiz@healthpredict.com'), '2025-01-26 10:30:00', 'Pasos Diarios', 5200, 'pasos', 'Apple Watch', 'Sedentario'),
((SELECT "ID" FROM "USUARIOS" WHERE "EMAIL" = 'pedro.ruiz@healthpredict.com'), '2025-01-26 10:30:00', 'Horas de Sueño', 6.0, 'horas', 'Apple Watch', 'Sueño insuficiente'),
((SELECT "ID" FROM "USUARIOS" WHERE "EMAIL" = 'pedro.ruiz@healthpredict.com'), '2025-01-26 10:30:00', 'Nivel de Estrés', 6, 'escala 1-10', 'Autoevaluación', 'Estrés moderado-alto');

-- =====================================================
-- 3. ALERTAS DE RIESGO VARIADAS
-- =====================================================

-- Alertas CRÍTICAS para Diego
INSERT INTO "ALERTAS" ("USUARIO_ID", "TIPO_ALERTA", "MENSAJE", "NIVEL_CRITICIDAD", "FECHA_CREACION", "LEIDA", "FECHA_LECTURA") VALUES
(7, 'Presión Alta', 'Presión arterial peligrosamente alta: 165/108 mmHg. Requiere atención médica inmediata.', 'Crítica', '2025-01-26 07:35:00', false, NULL),
(7, 'Frecuencia Cardíaca', 'Frecuencia cardíaca elevada: 115 bpm en reposo. Posible taquicardia.', 'Crítica', '2025-01-26 07:35:00', false, NULL),
(7, 'Saturación Oxígeno', 'Saturación de oxígeno baja: 91%. Revisar función respiratoria.', 'Alta', '2025-01-26 07:35:00', false, NULL),
(7, 'Falta de Sueño', 'Menos de 3 horas de sueño. Impacto severo en salud y productividad.', 'Alta', '2025-01-25 08:20:00', false, NULL),
(7, 'Estrés Extremo', 'Nivel de estrés máximo (10/10) durante 3 días consecutivos.', 'Crítica', '2025-01-24 09:00:00', false, NULL);

-- Alertas MODERADAS para Matias
INSERT INTO "ALERTAS" ("USUARIO_ID", "TIPO_ALERTA", "MENSAJE", "NIVEL_CRITICIDAD", "FECHA_CREACION", "LEIDA", "FECHA_LECTURA") VALUES
(8, 'Tendencia Presión', 'Tendencia al alza en presión arterial: 145/97 mmHg. Monitorear.', 'Media', '2025-01-26 09:05:00', false, NULL),
(8, 'Peso Aumentando', 'Aumento de peso gradual: +2.5 kg en 25 días. Revisar dieta.', 'Media', '2025-01-25 08:25:00', true, '2025-01-25 14:30:00'),
(8, 'Actividad Reducida', 'Reducción del 30% en actividad física en las últimas 2 semanas.', 'Media', '2025-01-20 09:35:00', true, '2025-01-21 10:15:00');

-- Alertas INFORMATIVAS para Carlos (estrés ejecutivo)
INSERT INTO "ALERTAS" ("USUARIO_ID", "TIPO_ALERTA", "MENSAJE", "NIVEL_CRITICIDAD", "FECHA_CREACION", "LEIDA", "FECHA_LECTURA") VALUES
(6, 'Estrés Ejecutivo', 'Nivel de estrés elevado: 9/10. Considerar técnicas de relajación.', 'Media', '2025-01-26 10:05:00', false, NULL),
(6, 'Sueño Insuficiente', 'Promedio de 4.2 horas de sueño. Impacta liderazgo y decisiones.', 'Media', '2025-01-25 09:25:00', true, '2025-01-25 16:45:00'),
(6, 'Sedentarismo', 'Solo 4,500 pasos diarios. Recomendado: mínimo 8,000 pasos.', 'Baja', '2025-01-24 10:30:00', true, '2025-01-24 17:20:00');

-- Alertas POSITIVAS para empleados modelo
INSERT INTO "ALERTAS" ("USUARIO_ID", "TIPO_ALERTA", "MENSAJE", "NIVEL_CRITICIDAD", "FECHA_CREACION", "LEIDA", "FECHA_LECTURA") VALUES
(9, 'Excelente Condición', 'Métricas de salud óptimas. ¡Sigue así!', 'Baja', '2025-01-26 07:45:00', true, '2025-01-26 08:30:00'),
(9, 'Meta Actividad', 'Superaste la meta diaria: 12,100 pasos. ¡Felicitaciones!', 'Baja', '2025-01-25 20:00:00', true, '2025-01-26 07:15:00');

-- Alertas para nuevos usuarios
INSERT INTO "ALERTAS" ("USUARIO_ID", "TIPO_ALERTA", "MENSAJE", "NIVEL_CRITICIDAD", "FECHA_CREACION", "LEIDA") VALUES
((SELECT "ID" FROM "USUARIOS" WHERE "EMAIL" = 'ana.martinez@healthpredict.com'), 'Estrés Moderado', 'Nivel de estrés en aumento. Considera pausas activas.', 'Media', '2025-01-26 09:35:00', false),
((SELECT "ID" FROM "USUARIOS" WHERE "EMAIL" = 'pedro.ruiz@healthpredict.com'), 'Sobrepeso', 'IMC elevado. Programa de wellness recomendado.', 'Media', '2025-01-26 10:35:00', false),
((SELECT "ID" FROM "USUARIOS" WHERE "EMAIL" = 'sofia.lopez@healthpredict.com'), 'Excelente Salud', 'Métricas perfectas. ¡Ejemplo para el equipo!', 'Baja', '2025-01-26 09:20:00', true);

-- =====================================================
-- 4. CONSULTAS DE VERIFICACIÓN
-- =====================================================

-- Ver resumen de datos creados
SELECT 
    'USUARIOS' as tabla,
    COUNT(*) as total
FROM "USUARIOS"
UNION ALL
SELECT 
    'DATOS_VITALES' as tabla,
    COUNT(*) as total
FROM "DATOS_VITALES"
UNION ALL
SELECT 
    'ALERTAS' as tabla,
    COUNT(*) as total
FROM "ALERTAS";

-- Ver distribución por departamento
SELECT 
    "DEPARTAMENTO",
    COUNT(*) as trabajadores
FROM "USUARIOS" 
WHERE "ROL" = 'Trabajador' AND "ES_ACTIVO" = true
GROUP BY "DEPARTAMENTO"
ORDER BY trabajadores DESC;

-- Ver alertas por criticidad
SELECT 
    "NIVEL_CRITICIDAD",
    COUNT(*) as total_alertas,
    COUNT(CASE WHEN "LEIDA" = false THEN 1 END) as no_leidas
FROM "ALERTAS"
GROUP BY "NIVEL_CRITICIDAD"
ORDER BY 
    CASE "NIVEL_CRITICIDAD" 
        WHEN 'Crítica' THEN 1 
        WHEN 'Alta' THEN 2 
        WHEN 'Media' THEN 3 
        WHEN 'Baja' THEN 4 
    END;

-- Ver últimos datos vitales por usuario
SELECT 
    u."NOMBRE" || ' ' || u."APELLIDO" as trabajador,
    u."DEPARTAMENTO",
    dv."TIPO_DATO",
    dv."VALOR",
    dv."UNIDAD",
    dv."FECHA_REGISTRO"
FROM "USUARIOS" u
JOIN "DATOS_VITALES" dv ON u."ID" = dv."USUARIO_ID"
WHERE u."ROL" = 'Trabajador' 
    AND dv."FECHA_REGISTRO" >= '2025-01-26'
    AND dv."TIPO_DATO" IN ('Nivel de Estrés', 'Frecuencia Cardíaca', 'Presión Sistólica')
ORDER BY u."NOMBRE", dv."TIPO_DATO"; 