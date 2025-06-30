-- SCRIPT FINAL - DATOS DE PRUEBA PANEL SUPERVISOR
-- Estructura corregida para ALERTAS (DESCRIPCION, SEVERIDAD)

-- Datos vitales para Diego (ID 7) - Crisis de salud
INSERT INTO "DATOS_VITALES" ("USUARIO_ID", "FECHA_REGISTRO", "TIPO_DATO", "VALOR", "UNIDAD", "DISPOSITIVO_ORIGEN", "NOTAS") VALUES
(7, '2025-01-26 07:30:00', 'Frecuencia Cardíaca', 115, 'bpm', 'Apple Watch', 'CRÍTICO - Taquicardia'),
(7, '2025-01-26 07:30:00', 'Presión Sistólica', 165, 'mmHg', 'Monitor', 'CRÍTICO - Hipertensión'),
(7, '2025-01-26 07:30:00', 'Presión Diastólica', 108, 'mmHg', 'Monitor', 'CRÍTICO - Hipertensión'),
(7, '2025-01-26 07:30:00', 'Nivel de Estrés', 10, 'escala 1-10', 'Autoevaluación', 'CRÍTICO - Estrés máximo'),
(7, '2025-01-26 07:30:00', 'Horas de Sueño', 2.8, 'horas', 'Apple Watch', 'CRÍTICO - Insomnio'),
(7, '2025-01-26 07:30:00', 'Pasos Diarios', 2100, 'pasos', 'Apple Watch', 'CRÍTICO - Sedentarismo');

-- Datos vitales para Matias (ID 8) - Tendencia preocupante
INSERT INTO "DATOS_VITALES" ("USUARIO_ID", "FECHA_REGISTRO", "TIPO_DATO", "VALOR", "UNIDAD", "DISPOSITIVO_ORIGEN", "NOTAS") VALUES  
(8, '2025-01-26 09:00:00', 'Frecuencia Cardíaca', 87, 'bpm', 'Fitbit', 'Incremento gradual'),
(8, '2025-01-26 09:00:00', 'Presión Sistólica', 145, 'mmHg', 'Monitor', 'Tendencia alta'),
(8, '2025-01-26 09:00:00', 'Presión Diastólica', 97, 'mmHg', 'Monitor', 'Tendencia alta'),
(8, '2025-01-26 09:00:00', 'Nivel de Estrés', 7, 'escala 1-10', 'Autoevaluación', 'Estrés alto'),
(8, '2025-01-26 09:00:00', 'Horas de Sueño', 5.5, 'horas', 'Fitbit', 'Sueño insuficiente');

-- Datos vitales para Iahn (ID 9) - Empleado modelo
INSERT INTO "DATOS_VITALES" ("USUARIO_ID", "FECHA_REGISTRO", "TIPO_DATO", "VALOR", "UNIDAD", "DISPOSITIVO_ORIGEN", "NOTAS") VALUES
(9, '2025-01-26 07:40:00', 'Frecuencia Cardíaca', 63, 'bpm', 'Garmin', 'Excelente'),
(9, '2025-01-26 07:40:00', 'Presión Sistólica', 116, 'mmHg', 'Monitor', 'Óptima'),
(9, '2025-01-26 07:40:00', 'Presión Diastólica', 76, 'mmHg', 'Monitor', 'Óptima'),
(9, '2025-01-26 07:40:00', 'Nivel de Estrés', 1, 'escala 1-10', 'Autoevaluación', 'Muy relajado'),
(9, '2025-01-26 07:40:00', 'Horas de Sueño', 8.4, 'horas', 'Garmin', 'Sueño reparador'),
(9, '2025-01-26 07:40:00', 'Pasos Diarios', 12100, 'pasos', 'Garmin', 'Muy activo');

-- Datos vitales para Carlos (ID 6) - Jefe con estrés
INSERT INTO "DATOS_VITALES" ("USUARIO_ID", "FECHA_REGISTRO", "TIPO_DATO", "VALOR", "UNIDAD", "DISPOSITIVO_ORIGEN", "NOTAS") VALUES
(6, '2025-01-26 10:00:00', 'Frecuencia Cardíaca', 98, 'bpm', 'Samsung Watch', 'Estrés ejecutivo'),
(6, '2025-01-26 10:00:00', 'Presión Sistólica', 155, 'mmHg', 'Monitor', 'Hipertensión'),
(6, '2025-01-26 10:00:00', 'Presión Diastólica', 102, 'mmHg', 'Monitor', 'Hipertensión'),
(6, '2025-01-26 10:00:00', 'Nivel de Estrés', 9, 'escala 1-10', 'Autoevaluación', 'Muy alto'),
(6, '2025-01-26 10:00:00', 'Horas de Sueño', 4.2, 'horas', 'Samsung Watch', 'Insomnio'),
(6, '2025-01-26 10:00:00', 'Pasos Diarios', 4500, 'pasos', 'Samsung Watch', 'Sedentario');

-- Alertas para todos los usuarios (ESTRUCTURA CORREGIDA)
INSERT INTO "ALERTAS" ("USUARIO_ID", "TIPO_ALERTA", "DESCRIPCION", "SEVERIDAD", "FECHA_CREACION", "LEIDA", "RESUELTA") VALUES
-- Alertas críticas para Diego
(7, 'Presión Alta', 'Presión 165/108 mmHg - REQUIERE ATENCIÓN MÉDICA INMEDIATA', 'Crítica', '2025-01-26 07:35:00', false, false),
(7, 'Taquicardia', 'Frecuencia cardíaca 115 bpm - Posible taquicardia', 'Crítica', '2025-01-26 07:35:00', false, false),
(7, 'Insomnio Severo', 'Solo 2.8 horas de sueño - Impacto crítico en salud', 'Alta', '2025-01-26 07:35:00', false, false),
-- Alertas moderadas para Matias
(8, 'Presión Elevándose', 'Presión 145/97 mmHg - Monitorear tendencia', 'Media', '2025-01-26 09:05:00', false, false),
(8, 'Estrés Alto', 'Nivel de estrés 7/10 - Considerar intervención', 'Media', '2025-01-26 09:05:00', false, false),
-- Alertas para Carlos (jefe)
(6, 'Estrés Ejecutivo', 'Estrés 9/10 - Afecta liderazgo del equipo', 'Media', '2025-01-26 10:05:00', false, false),
(6, 'Sueño Insuficiente', 'Solo 4.2 horas - Impacta decisiones gerenciales', 'Media', '2025-01-26 10:05:00', false, false),
-- Alertas positivas para Iahn
(9, 'Empleado Modelo', '¡Métricas de salud perfectas! Ejemplo para el equipo', 'Baja', '2025-01-26 07:45:00', true, true),
(9, 'Meta Superada', 'Superó meta de pasos: 12,100 pasos', 'Baja', '2025-01-26 07:45:00', true, true);

-- Verificar datos insertados
SELECT 
    u."NOMBRE" || ' ' || u."APELLIDO" as trabajador,
    u."DEPARTAMENTO",
    COUNT(DISTINCT dv."ID") as datos_vitales,
    COUNT(DISTINCT a."ID") as alertas,
    COUNT(CASE WHEN a."SEVERIDAD" = 'Crítica' THEN 1 END) as alertas_criticas
FROM "USUARIOS" u
LEFT JOIN "DATOS_VITALES" dv ON u."ID" = dv."USUARIO_ID"
LEFT JOIN "ALERTAS" a ON u."ID" = a."USUARIO_ID"
WHERE u."ID" IN (6, 7, 8, 9)
GROUP BY u."ID", u."NOMBRE", u."APELLIDO", u."DEPARTAMENTO"
ORDER BY u."ID"; 