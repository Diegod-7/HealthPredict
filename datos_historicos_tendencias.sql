-- DATOS HISTÓRICOS PARA TENDENCIAS Y REPORTES
-- Ejecutar después del script principal de datos de prueba

-- =====================================================
-- DATOS HISTÓRICOS ÚLTIMOS 60 DÍAS
-- =====================================================

-- Diego - Deterioro progresivo (simulando burnout)
INSERT INTO "DATOS_VITALES" ("USUARIO_ID", "FECHA_REGISTRO", "FRECUENCIA_CARDIACA", "PRESION_SISTOLICA", "PRESION_DIASTOLICA", "TEMPERATURA", "PESO", "PASOS_DIARIOS", "HORAS_SUENO", "NIVEL_ESTRES", "SATURACION_OXIGENO") VALUES
-- Diciembre 2024 - Inicio normal
(7, '2024-12-01 08:30:00', 68, 118, 78, 36.4, 74.8, 9800, 8.2, 2, 99),
(7, '2024-12-05 08:45:00', 70, 120, 80, 36.5, 74.9, 9500, 8.0, 3, 98),
(7, '2024-12-10 09:00:00', 72, 122, 82, 36.5, 75.0, 9200, 7.8, 3, 98),
(7, '2024-12-15 08:20:00', 75, 125, 83, 36.6, 75.1, 8800, 7.5, 4, 97),
(7, '2024-12-20 09:15:00', 78, 128, 85, 36.7, 75.2, 8200, 7.0, 5, 97),
(7, '2024-12-25 08:30:00', 82, 132, 88, 36.8, 75.3, 7500, 6.5, 6, 96),
(7, '2024-12-30 09:00:00', 85, 135, 90, 36.9, 75.4, 7000, 6.0, 6, 96);

-- Matias - Estable con ligeras variaciones
INSERT INTO "DATOS_VITALES" ("USUARIO_ID", "FECHA_REGISTRO", "FRECUENCIA_CARDIACA", "PRESION_SISTOLICA", "PRESION_DIASTOLICA", "TEMPERATURA", "PESO", "PASOS_DIARIOS", "HORAS_SUENO", "NIVEL_ESTRES", "SATURACION_OXIGENO") VALUES
(8, '2024-12-01 09:00:00', 68, 122, 83, 36.4, 81.5, 9800, 8.0, 3, 98),
(8, '2024-12-10 08:45:00', 71, 125, 84, 36.5, 81.8, 9200, 7.8, 3, 98),
(8, '2024-12-20 09:15:00', 74, 128, 86, 36.6, 82.2, 8800, 7.5, 4, 97),
(8, '2024-12-30 08:30:00', 76, 130, 87, 36.7, 82.5, 8500, 7.2, 4, 97);

-- Iahn - Consistentemente saludable
INSERT INTO "DATOS_VITALES" ("USUARIO_ID", "FECHA_REGISTRO", "FRECUENCIA_CARDIACA", "PRESION_SISTOLICA", "PRESION_DIASTOLICA", "TEMPERATURA", "PESO", "PASOS_DIARIOS", "HORAS_SUENO", "NIVEL_ESTRES", "SATURACION_OXIGENO") VALUES
(9, '2024-12-01 07:30:00', 60, 112, 73, 36.2, 70.5, 12500, 8.8, 1, 99),
(9, '2024-12-10 08:00:00', 62, 114, 74, 36.3, 70.3, 12200, 8.5, 2, 99),
(9, '2024-12-20 07:45:00', 64, 116, 75, 36.4, 70.1, 11800, 8.3, 2, 99),
(9, '2024-12-30 08:15:00', 63, 115, 74, 36.3, 70.0, 12000, 8.6, 1, 99);

-- Carlos - Estrés ejecutivo creciente
INSERT INTO "DATOS_VITALES" ("USUARIO_ID", "FECHA_REGISTRO", "FRECUENCIA_CARDIACA", "PRESION_SISTOLICA", "PRESION_DIASTOLICA", "TEMPERATURA", "PESO", "PASOS_DIARIOS", "HORAS_SUENO", "NIVEL_ESTRES", "SATURACION_OXIGENO") VALUES
(6, '2024-12-01 10:00:00', 72, 125, 82, 36.5, 79.8, 8500, 7.5, 4, 98),
(6, '2024-12-10 09:30:00', 76, 130, 84, 36.6, 80.1, 7800, 7.0, 5, 97),
(6, '2024-12-20 10:15:00', 80, 135, 87, 36.7, 80.4, 7200, 6.5, 6, 97),
(6, '2024-12-30 09:45:00', 85, 140, 90, 36.8, 80.7, 6500, 6.0, 7, 96);

-- =====================================================
-- ALERTAS HISTÓRICAS PARA ANÁLISIS DE TENDENCIAS
-- =====================================================

-- Alertas progresivas para Diego (muestra escalamiento del problema)
INSERT INTO "ALERTAS" ("USUARIO_ID", "TIPO_ALERTA", "MENSAJE", "NIVEL_CRITICIDAD", "FECHA_CREACION", "LEIDA", "FECHA_LECTURA") VALUES
(7, 'Actividad Reducida', 'Reducción del 15% en actividad física semanal.', 'Baja', '2024-12-15 09:00:00', true, '2024-12-15 14:30:00'),
(7, 'Sueño Irregular', 'Patrón de sueño irregular detectado.', 'Media', '2024-12-25 08:30:00', true, '2024-12-26 09:15:00'),
(7, 'Estrés en Aumento', 'Nivel de estrés incrementando gradualmente.', 'Media', '2025-01-05 09:00:00', true, '2025-01-05 16:20:00'),
(7, 'Presión Elevada', 'Presión arterial por encima del rango normal.', 'Alta', '2025-01-15 08:25:00', false, NULL),
(7, 'Múltiples Factores', 'Combinación de factores de riesgo detectados.', 'Alta', '2025-01-20 09:10:00', false, NULL);

-- Alertas menores para otros trabajadores
INSERT INTO "ALERTAS" ("USUARIO_ID", "TIPO_ALERTA", "MENSAJE", "NIVEL_CRITICIDAD", "FECHA_CREACION", "LEIDA", "FECHA_LECTURA") VALUES
(8, 'Peso Estable', 'Ligero aumento de peso. Mantener monitoreo.', 'Baja', '2024-12-20 09:30:00', true, '2024-12-21 08:45:00'),
(6, 'Carga Laboral', 'Indicadores de alta carga de trabajo ejecutiva.', 'Media', '2024-12-30 10:00:00', true, '2025-01-02 09:15:00'),
(9, 'Excelente Progreso', 'Mantiene métricas de salud óptimas consistentemente.', 'Baja', '2024-12-31 08:00:00', true, '2025-01-01 07:30:00');

-- =====================================================
-- DATOS ADICIONALES PARA MÉTRICAS DEPARTAMENTALES
-- =====================================================

-- Más datos para análisis por departamento (últimos 15 días)
INSERT INTO "DATOS_VITALES" ("USUARIO_ID", "FECHA_REGISTRO", "FRECUENCIA_CARDIACA", "PRESION_SISTOLICA", "PRESION_DIASTOLICA", "TEMPERATURA", "PESO", "PASOS_DIARIOS", "HORAS_SUENO", "NIVEL_ESTRES", "SATURACION_OXIGENO") VALUES
-- Marketing - Ana (datos adicionales)
((SELECT "ID" FROM "USUARIOS" WHERE "EMAIL" = 'ana.martinez@healthpredict.com'), '2025-01-20 09:30:00', 72, 122, 80, 36.4, 58.0, 8800, 7.2, 4, 98),
((SELECT "ID" FROM "USUARIOS" WHERE "EMAIL" = 'ana.martinez@healthpredict.com'), '2025-01-15 08:45:00', 70, 120, 78, 36.3, 57.8, 9200, 7.5, 3, 98),
((SELECT "ID" FROM "USUARIOS" WHERE "EMAIL" = 'ana.martinez@healthpredict.com'), '2025-01-10 09:00:00', 68, 118, 76, 36.2, 57.5, 9500, 8.0, 2, 99),

-- Marketing - Luis (datos adicionales)
((SELECT "ID" FROM "USUARIOS" WHERE "EMAIL" = 'luis.garcia@healthpredict.com'), '2025-01-20 08:45:00', 70, 120, 80, 36.5, 72.8, 10500, 8.0, 3, 99),
((SELECT "ID" FROM "USUARIOS" WHERE "EMAIL" = 'luis.garcia@healthpredict.com'), '2025-01-15 09:15:00', 67, 117, 77, 36.3, 72.5, 10800, 8.2, 2, 99),
((SELECT "ID" FROM "USUARIOS" WHERE "EMAIL" = 'luis.garcia@healthpredict.com'), '2025-01-10 08:30:00', 65, 115, 75, 36.2, 72.2, 11000, 8.5, 2, 99),

-- RRHH - Sofia (datos adicionales)
((SELECT "ID" FROM "USUARIOS" WHERE "EMAIL" = 'sofia.lopez@healthpredict.com'), '2025-01-20 09:15:00', 67, 117, 77, 36.4, 55.8, 11200, 8.3, 2, 99),
((SELECT "ID" FROM "USUARIOS" WHERE "EMAIL" = 'sofia.lopez@healthpredict.com'), '2025-01-15 08:50:00', 65, 115, 75, 36.3, 55.6, 11800, 8.6, 1, 99),
((SELECT "ID" FROM "USUARIOS" WHERE "EMAIL" = 'sofia.lopez@healthpredict.com'), '2025-01-10 09:30:00', 63, 113, 73, 36.2, 55.4, 12000, 8.8, 1, 99),

-- RRHH - Pedro (datos adicionales)
((SELECT "ID" FROM "USUARIOS" WHERE "EMAIL" = 'pedro.ruiz@healthpredict.com'), '2025-01-20 10:30:00', 85, 142, 92, 36.7, 86.8, 5500, 6.2, 6, 96),
((SELECT "ID" FROM "USUARIOS" WHERE "EMAIL" = 'pedro.ruiz@healthpredict.com'), '2025-01-15 10:00:00', 82, 138, 89, 36.6, 86.2, 5800, 6.5, 5, 97),
((SELECT "ID" FROM "USUARIOS" WHERE "EMAIL" = 'pedro.ruiz@healthpredict.com'), '2025-01-10 09:45:00', 80, 135, 87, 36.5, 85.8, 6200, 6.8, 5, 97);

-- =====================================================
-- CONSULTAS DE ANÁLISIS PARA EL PANEL SUPERVISOR
-- =====================================================

-- 1. Resumen ejecutivo por criticidad de alertas
SELECT 
    'RESUMEN ALERTAS' as reporte,
    "NIVEL_CRITICIDAD",
    COUNT(*) as total,
    COUNT(CASE WHEN "LEIDA" = false THEN 1 END) as pendientes,
    ROUND(
        (COUNT(CASE WHEN "LEIDA" = false THEN 1 END) * 100.0 / COUNT(*)), 1
    ) as porcentaje_pendientes
FROM "ALERTAS"
WHERE "FECHA_CREACION" >= CURRENT_DATE - INTERVAL '30 days'
GROUP BY "NIVEL_CRITICIDAD"
ORDER BY 
    CASE "NIVEL_CRITICIDAD" 
        WHEN 'Crítica' THEN 1 
        WHEN 'Alta' THEN 2 
        WHEN 'Media' THEN 3 
        WHEN 'Baja' THEN 4 
    END;

-- 2. Trabajadores en riesgo (múltiples factores)
SELECT 
    'TRABAJADORES EN RIESGO' as reporte,
    u."NOMBRE" || ' ' || u."APELLIDO" as trabajador,
    u."DEPARTAMENTO",
    u."CARGO",
    COUNT(a."ID") as alertas_activas,
    MAX(CASE WHEN a."NIVEL_CRITICIDAD" = 'Crítica' THEN 'CRÍTICO'
             WHEN a."NIVEL_CRITICIDAD" = 'Alta' THEN 'ALTO'
             WHEN a."NIVEL_CRITICIDAD" = 'Media' THEN 'MEDIO'
             ELSE 'BAJO' END) as nivel_riesgo_max,
    dv."NIVEL_ESTRES" as estres_actual,
    dv."FRECUENCIA_CARDIACA" as fc_actual
FROM "USUARIOS" u
JOIN "ALERTAS" a ON u."ID" = a."USUARIO_ID"
LEFT JOIN "DATOS_VITALES" dv ON u."ID" = dv."USUARIO_ID" 
    AND dv."FECHA_REGISTRO" = (
        SELECT MAX("FECHA_REGISTRO") 
        FROM "DATOS_VITALES" 
        WHERE "USUARIO_ID" = u."ID"
    )
WHERE u."ROL" = 'Trabajador' 
    AND a."LEIDA" = false
    AND a."FECHA_CREACION" >= CURRENT_DATE - INTERVAL '7 days'
GROUP BY u."ID", u."NOMBRE", u."APELLIDO", u."DEPARTAMENTO", u."CARGO", 
         dv."NIVEL_ESTRES", dv."FRECUENCIA_CARDIACA"
HAVING COUNT(a."ID") >= 2
ORDER BY COUNT(a."ID") DESC, 
    CASE MAX(a."NIVEL_CRITICIDAD")
        WHEN 'Crítica' THEN 1 
        WHEN 'Alta' THEN 2 
        WHEN 'Media' THEN 3 
        WHEN 'Baja' THEN 4 
    END;

-- 3. Métricas por departamento (últimos 30 días)
SELECT 
    'MÉTRICAS DEPARTAMENTO' as reporte,
    u."DEPARTAMENTO",
    COUNT(DISTINCT u."ID") as total_trabajadores,
    ROUND(AVG(dv."NIVEL_ESTRES"), 1) as estres_promedio,
    ROUND(AVG(dv."HORAS_SUENO"), 1) as sueno_promedio,
    ROUND(AVG(dv."PASOS_DIARIOS"), 0) as pasos_promedio,
    COUNT(a."ID") as total_alertas,
    COUNT(CASE WHEN a."NIVEL_CRITICIDAD" IN ('Crítica', 'Alta') THEN 1 END) as alertas_urgentes
FROM "USUARIOS" u
LEFT JOIN "DATOS_VITALES" dv ON u."ID" = dv."USUARIO_ID" 
    AND dv."FECHA_REGISTRO" >= CURRENT_DATE - INTERVAL '30 days'
LEFT JOIN "ALERTAS" a ON u."ID" = a."USUARIO_ID" 
    AND a."FECHA_CREACION" >= CURRENT_DATE - INTERVAL '30 days'
WHERE u."ROL" = 'Trabajador' AND u."ES_ACTIVO" = true
GROUP BY u."DEPARTAMENTO"
ORDER BY alertas_urgentes DESC, estres_promedio DESC;

-- 4. Tendencias de salud (comparación últimos 7 vs 30 días)
SELECT 
    'TENDENCIAS SALUD' as reporte,
    u."NOMBRE" || ' ' || u."APELLIDO" as trabajador,
    u."DEPARTAMENTO",
    ROUND(
        AVG(CASE WHEN dv."FECHA_REGISTRO" >= CURRENT_DATE - INTERVAL '7 days' 
                 THEN dv."NIVEL_ESTRES" END), 1
    ) as estres_7d,
    ROUND(
        AVG(CASE WHEN dv."FECHA_REGISTRO" >= CURRENT_DATE - INTERVAL '30 days' 
                 THEN dv."NIVEL_ESTRES" END), 1
    ) as estres_30d,
    ROUND(
        (AVG(CASE WHEN dv."FECHA_REGISTRO" >= CURRENT_DATE - INTERVAL '7 days' 
                  THEN dv."NIVEL_ESTRES" END) - 
         AVG(CASE WHEN dv."FECHA_REGISTRO" >= CURRENT_DATE - INTERVAL '30 days' 
                  THEN dv."NIVEL_ESTRES" END)), 1
    ) as tendencia_estres
FROM "USUARIOS" u
JOIN "DATOS_VITALES" dv ON u."ID" = dv."USUARIO_ID"
WHERE u."ROL" = 'Trabajador' 
    AND dv."FECHA_REGISTRO" >= CURRENT_DATE - INTERVAL '30 days'
GROUP BY u."ID", u."NOMBRE", u."APELLIDO", u."DEPARTAMENTO"
HAVING COUNT(dv."ID") >= 3
ORDER BY tendencia_estres DESC; 