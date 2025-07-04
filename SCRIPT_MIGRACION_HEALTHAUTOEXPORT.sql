-- ===============================================
-- SCRIPT DE MIGRACIÓN PARA HEALTH AUTO EXPORT
-- HealthPredict - Integración con HealthyApps.dev
-- Fecha: 2025-01-17
-- ===============================================

-- 1. AGREGAR NUEVOS CAMPOS A LA TABLA DATOS_VITALES
-- =====================================================

-- Verificar si las columnas ya existen antes de agregarlas
DO $$
BEGIN
    -- Agregar FECHA_MEDICION si no existe
    IF NOT EXISTS (SELECT 1 FROM information_schema.columns 
                   WHERE table_name = 'DATOS_VITALES' 
                   AND column_name = 'FECHA_MEDICION') THEN
        ALTER TABLE DATOS_VITALES ADD COLUMN FECHA_MEDICION TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP;
        RAISE NOTICE 'Columna FECHA_MEDICION agregada a DATOS_VITALES';
    END IF;

    -- Agregar DISPOSITIVO si no existe
    IF NOT EXISTS (SELECT 1 FROM information_schema.columns 
                   WHERE table_name = 'DATOS_VITALES' 
                   AND column_name = 'DISPOSITIVO') THEN
        ALTER TABLE DATOS_VITALES ADD COLUMN DISPOSITIVO VARCHAR(100);
        RAISE NOTICE 'Columna DISPOSITIVO agregada a DATOS_VITALES';
    END IF;

    -- Agregar FUENTE si no existe
    IF NOT EXISTS (SELECT 1 FROM information_schema.columns 
                   WHERE table_name = 'DATOS_VITALES' 
                   AND column_name = 'FUENTE') THEN
        ALTER TABLE DATOS_VITALES ADD COLUMN FUENTE VARCHAR(100);
        RAISE NOTICE 'Columna FUENTE agregada a DATOS_VITALES';
    END IF;
END $$;

-- Actualizar registros existentes para establecer FECHA_MEDICION = FECHA_REGISTRO
UPDATE DATOS_VITALES 
SET FECHA_MEDICION = FECHA_REGISTRO 
WHERE FECHA_MEDICION IS NULL OR FECHA_MEDICION = '1900-01-01';

-- 2. CREAR TABLA HEALTH_AUTO_EXPORT_CONFIGS
-- ==========================================

CREATE TABLE IF NOT EXISTS HEALTH_AUTO_EXPORT_CONFIGS (
    ID SERIAL PRIMARY KEY,
    USUARIO_ID INTEGER NOT NULL,
    API_KEY VARCHAR(100) NOT NULL,
    IS_ACTIVE BOOLEAN NOT NULL DEFAULT TRUE,
    CREATED_AT TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    LAST_SYNC_AT TIMESTAMP NULL,
    ALLOWED_DATA_TYPES VARCHAR(1000) NULL,
    DEVICE_INFO VARCHAR(500) NULL,
    SYNC_INTERVAL_MINUTES INTEGER NOT NULL DEFAULT 60,
    
    -- Restricciones
    CONSTRAINT FK_HEALTH_AUTO_EXPORT_CONFIGS_USUARIOS 
        FOREIGN KEY (USUARIO_ID) REFERENCES USUARIOS(ID) ON DELETE CASCADE
);

-- 3. CREAR ÍNDICES PARA OPTIMIZAR RENDIMIENTO
-- ============================================

-- Índice único para API Key
CREATE UNIQUE INDEX IF NOT EXISTS IX_HEALTH_AUTO_EXPORT_CONFIGS_API_KEY 
ON HEALTH_AUTO_EXPORT_CONFIGS (API_KEY);

-- Índice para búsquedas por usuario
CREATE INDEX IF NOT EXISTS IX_HEALTH_AUTO_EXPORT_CONFIGS_USUARIO 
ON HEALTH_AUTO_EXPORT_CONFIGS (USUARIO_ID);

-- Índice para datos vitales por fuente
CREATE INDEX IF NOT EXISTS IX_DATOS_VITALES_FUENTE 
ON DATOS_VITALES (FUENTE);

-- Índice para datos vitales por fecha de medición
CREATE INDEX IF NOT EXISTS IX_DATOS_VITALES_FECHA_MEDICION 
ON DATOS_VITALES (FECHA_MEDICION);

-- 4. CREAR CONFIGURACIÓN INICIAL PARA USUARIO 7
-- ==============================================

-- Generar API Key para usuario 7
DO $$
DECLARE
    api_key VARCHAR(100);
BEGIN
    -- Generar API Key única
    api_key := UPPER(REPLACE(gen_random_uuid()::text, '-', ''));
    
    -- Insertar configuración si no existe
    INSERT INTO HEALTH_AUTO_EXPORT_CONFIGS (
        USUARIO_ID, 
        API_KEY, 
        IS_ACTIVE, 
        CREATED_AT, 
        ALLOWED_DATA_TYPES, 
        SYNC_INTERVAL_MINUTES
    )
    SELECT 
        7,
        api_key,
        TRUE,
        CURRENT_TIMESTAMP,
        '["stepcount","heartrate","bloodpressuresystolic","bloodpressurediastolic","bloodglucose","bodyweight","bodytemperature","oxygensaturation","sleepanalysis","activeenergyburned","distancewalking","vo2max","restingheartrate","walkingheartrateaverage","respiratoryrate"]',
        60
    WHERE NOT EXISTS (
        SELECT 1 FROM HEALTH_AUTO_EXPORT_CONFIGS WHERE USUARIO_ID = 7
    );
    
    IF FOUND THEN
        RAISE NOTICE 'Configuración de Health Auto Export creada para usuario 7 con API Key: %', api_key;
    ELSE
        RAISE NOTICE 'La configuración de Health Auto Export ya existe para usuario 7';
    END IF;
END $$;

-- 5. VERIFICAR USUARIO 7 EXISTE
-- ==============================

DO $$
BEGIN
    IF NOT EXISTS (SELECT 1 FROM USUARIOS WHERE ID = 7) THEN
        -- Crear usuario 7 si no existe
        INSERT INTO USUARIOS (
            ID, NOMBRE, APELLIDO, EMAIL, PASSWORD, 
            FECHA_NACIMIENTO, GENERO, ALTURA, PESO, 
            FECHA_REGISTRO, ULTIMO_ACCESO, ES_PROFESIONAL_MEDICO,
            ROL, ES_ACTIVO
        ) VALUES (
            7, 'Usuario', 'Health Auto Export', 'healthautoexport@healthpredict.com', 
            'password123', '1990-01-01', 'Otro', 170, 70, 
            CURRENT_TIMESTAMP, CURRENT_TIMESTAMP, FALSE,
            'Trabajador', TRUE
        );
        
        RAISE NOTICE 'Usuario 7 creado para Health Auto Export';
    ELSE
        RAISE NOTICE 'Usuario 7 ya existe';
    END IF;
END $$;

-- 6. CREAR FUNCIÓN PARA LIMPIAR DATOS ANTIGUOS
-- =============================================

CREATE OR REPLACE FUNCTION cleanup_old_health_data()
RETURNS INTEGER AS $$
DECLARE
    deleted_count INTEGER;
BEGIN
    -- Eliminar datos vitales más antiguos de 2 años
    DELETE FROM DATOS_VITALES 
    WHERE FECHA_MEDICION < CURRENT_DATE - INTERVAL '2 years'
    AND FUENTE LIKE '%Health Auto Export%';
    
    GET DIAGNOSTICS deleted_count = ROW_COUNT;
    
    RETURN deleted_count;
END;
$$ LANGUAGE plpgsql;

-- 7. CREAR VISTA PARA ESTADÍSTICAS DE HEALTH AUTO EXPORT
-- =======================================================

CREATE OR REPLACE VIEW V_HEALTH_AUTO_EXPORT_STATS AS
SELECT 
    u.ID as USUARIO_ID,
    u.NOMBRE || ' ' || u.APELLIDO as USUARIO_NOMBRE,
    c.API_KEY,
    c.IS_ACTIVE,
    c.CREATED_AT,
    c.LAST_SYNC_AT,
    c.SYNC_INTERVAL_MINUTES,
    COUNT(dv.ID) as TOTAL_DATOS,
    COUNT(CASE WHEN dv.FECHA_MEDICION >= CURRENT_DATE - INTERVAL '7 days' THEN 1 END) as DATOS_ULTIMA_SEMANA,
    COUNT(CASE WHEN dv.FECHA_MEDICION >= CURRENT_DATE - INTERVAL '30 days' THEN 1 END) as DATOS_ULTIMO_MES,
    COUNT(DISTINCT dv.TIPO_DATO) as TIPOS_DATOS_UNICOS,
    MAX(dv.FECHA_MEDICION) as ULTIMA_MEDICION
FROM HEALTH_AUTO_EXPORT_CONFIGS c
LEFT JOIN USUARIOS u ON c.USUARIO_ID = u.ID
LEFT JOIN DATOS_VITALES dv ON u.ID = dv.USUARIO_ID AND dv.FUENTE LIKE '%Health Auto Export%'
GROUP BY u.ID, u.NOMBRE, u.APELLIDO, c.API_KEY, c.IS_ACTIVE, c.CREATED_AT, c.LAST_SYNC_AT, c.SYNC_INTERVAL_MINUTES;

-- 8. MOSTRAR INFORMACIÓN DE LA CONFIGURACIÓN CREADA
-- ==================================================

SELECT 
    'Health Auto Export configurado exitosamente' as MENSAJE,
    API_KEY as API_KEY_GENERADA,
    'Usuario ID: 7' as USUARIO,
    CREATED_AT as FECHA_CREACION
FROM HEALTH_AUTO_EXPORT_CONFIGS 
WHERE USUARIO_ID = 7 
ORDER BY CREATED_AT DESC 
LIMIT 1;

-- 9. ENDPOINTS DISPONIBLES
-- =========================

SELECT 
    'Endpoints disponibles para Health Auto Export:' as INFORMACION
UNION ALL
SELECT 'POST /api/HealthAutoExport/data - Recibir datos individuales'
UNION ALL
SELECT 'POST /api/HealthAutoExport/batch - Recibir lotes de datos'
UNION ALL
SELECT 'POST /api/HealthAutoExport/simple - Recibir datos sin autenticación'
UNION ALL
SELECT 'POST /api/HealthAutoExport/json - Recibir datos en JSON genérico'
UNION ALL
SELECT 'GET /api/HealthAutoExport/test - Probar conectividad'
UNION ALL
SELECT 'GET /api/HealthAutoExport/stats - Ver estadísticas'
UNION ALL
SELECT 'GET /api/HealthAutoExport/config - Ver configuración'
UNION ALL
SELECT 'POST /api/HealthAutoExport/generate-api-key - Generar nueva API Key';

-- ===============================================
-- SCRIPT COMPLETADO EXITOSAMENTE
-- ===============================================

RAISE NOTICE '✅ Migración de Health Auto Export completada exitosamente';
RAISE NOTICE '📱 Configura tu aplicación Health Auto Export con estos endpoints:';
RAISE NOTICE '🔗 URL Base: https://tu-dominio.com/api/HealthAutoExport/';
RAISE NOTICE '🔑 Usa el endpoint /generate-api-key para obtener una API Key';
RAISE NOTICE '📊 Todos los datos se almacenarán para el usuario ID: 7'; 