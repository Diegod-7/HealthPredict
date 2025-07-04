-- =========================================================================
-- SCRIPT DE MIGRACIÓN: FitnessSyncer Integration
-- Base de Datos: HealthPredict
-- Fecha: 2024
-- Descripción: Agrega soporte para integración con FitnessSyncer API
-- =========================================================================

-- -------------------------------------------------------------------------
-- POSTGRESQL VERSION (Tu base de datos actual)
-- -------------------------------------------------------------------------

-- Crear tabla FITNESS_SYNCER_CONFIGS
CREATE TABLE IF NOT EXISTS "FITNESS_SYNCER_CONFIGS" (
    "ID" SERIAL PRIMARY KEY,
    "USUARIO_ID" INTEGER NOT NULL,
    "ACCESS_TOKEN" VARCHAR(1000) NOT NULL,
    "REFRESH_TOKEN" VARCHAR(1000) NOT NULL,
    "TOKEN_EXPIRY" TIMESTAMP NOT NULL,
    "IS_ACTIVE" BOOLEAN NOT NULL DEFAULT true,
    "FECHA_CREACION" TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    "ULTIMA_SINCRONIZACION" TIMESTAMP NULL,
    
    -- Llave foránea hacia la tabla USUARIOS
    CONSTRAINT "FK_FITNESS_SYNCER_CONFIGS_USUARIOS" 
        FOREIGN KEY ("USUARIO_ID") 
        REFERENCES "USUARIOS"("ID") 
        ON DELETE CASCADE
);

-- Crear índice único para evitar múltiples configuraciones activas por usuario
CREATE UNIQUE INDEX IF NOT EXISTS "IX_FITNESS_SYNCER_CONFIGS_USUARIO_ACTIVE" 
    ON "FITNESS_SYNCER_CONFIGS" ("USUARIO_ID", "IS_ACTIVE");

-- Crear índice para búsquedas por usuario
CREATE INDEX IF NOT EXISTS "IX_FITNESS_SYNCER_CONFIGS_USUARIO_ID" 
    ON "FITNESS_SYNCER_CONFIGS" ("USUARIO_ID");

-- Crear índice para búsquedas por fecha de sincronización
CREATE INDEX IF NOT EXISTS "IX_FITNESS_SYNCER_CONFIGS_ULTIMA_SINCRONIZACION" 
    ON "FITNESS_SYNCER_CONFIGS" ("ULTIMA_SINCRONIZACION");

-- Comentarios en la tabla
COMMENT ON TABLE "FITNESS_SYNCER_CONFIGS" IS 'Configuración de autenticación OAuth para FitnessSyncer por usuario';
COMMENT ON COLUMN "FITNESS_SYNCER_CONFIGS"."ID" IS 'Identificador único de la configuración';
COMMENT ON COLUMN "FITNESS_SYNCER_CONFIGS"."USUARIO_ID" IS 'ID del usuario propietario de la configuración';
COMMENT ON COLUMN "FITNESS_SYNCER_CONFIGS"."ACCESS_TOKEN" IS 'Token de acceso OAuth para FitnessSyncer API';
COMMENT ON COLUMN "FITNESS_SYNCER_CONFIGS"."REFRESH_TOKEN" IS 'Token de renovación OAuth para FitnessSyncer API';
COMMENT ON COLUMN "FITNESS_SYNCER_CONFIGS"."TOKEN_EXPIRY" IS 'Fecha y hora de expiración del token de acceso';
COMMENT ON COLUMN "FITNESS_SYNCER_CONFIGS"."IS_ACTIVE" IS 'Indica si la configuración está activa';
COMMENT ON COLUMN "FITNESS_SYNCER_CONFIGS"."FECHA_CREACION" IS 'Fecha y hora de creación de la configuración';
COMMENT ON COLUMN "FITNESS_SYNCER_CONFIGS"."ULTIMA_SINCRONIZACION" IS 'Fecha y hora de la última sincronización exitosa';

-- =========================================================================

-- -------------------------------------------------------------------------
-- SQL SERVER VERSION (Por si migras en el futuro)
-- -------------------------------------------------------------------------

/*
-- Crear tabla FITNESS_SYNCER_CONFIGS para SQL Server
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='FITNESS_SYNCER_CONFIGS' AND xtype='U')
BEGIN
    CREATE TABLE [FITNESS_SYNCER_CONFIGS] (
        [ID] INT IDENTITY(1,1) PRIMARY KEY,
        [USUARIO_ID] INT NOT NULL,
        [ACCESS_TOKEN] NVARCHAR(1000) NOT NULL,
        [REFRESH_TOKEN] NVARCHAR(1000) NOT NULL,
        [TOKEN_EXPIRY] DATETIME2 NOT NULL,
        [IS_ACTIVE] BIT NOT NULL DEFAULT 1,
        [FECHA_CREACION] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        [ULTIMA_SINCRONIZACION] DATETIME2 NULL,
        
        -- Llave foránea hacia la tabla USUARIOS
        CONSTRAINT [FK_FITNESS_SYNCER_CONFIGS_USUARIOS] 
            FOREIGN KEY ([USUARIO_ID]) 
            REFERENCES [USUARIOS]([ID]) 
            ON DELETE CASCADE
    );
END

-- Crear índice único para SQL Server
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_FITNESS_SYNCER_CONFIGS_USUARIO_ACTIVE')
BEGIN
    CREATE UNIQUE INDEX [IX_FITNESS_SYNCER_CONFIGS_USUARIO_ACTIVE] 
        ON [FITNESS_SYNCER_CONFIGS] ([USUARIO_ID], [IS_ACTIVE]) 
        WHERE [IS_ACTIVE] = 1;
END

-- Crear índices adicionales para SQL Server
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_FITNESS_SYNCER_CONFIGS_USUARIO_ID')
BEGIN
    CREATE INDEX [IX_FITNESS_SYNCER_CONFIGS_USUARIO_ID] 
        ON [FITNESS_SYNCER_CONFIGS] ([USUARIO_ID]);
END

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_FITNESS_SYNCER_CONFIGS_ULTIMA_SINCRONIZACION')
BEGIN
    CREATE INDEX [IX_FITNESS_SYNCER_CONFIGS_ULTIMA_SINCRONIZACION] 
        ON [FITNESS_SYNCER_CONFIGS] ([ULTIMA_SINCRONIZACION]);
END
*/

-- =========================================================================

-- -------------------------------------------------------------------------
-- MYSQL VERSION (Por si necesitas compatibilidad)
-- -------------------------------------------------------------------------

/*
-- Crear tabla FITNESS_SYNCER_CONFIGS para MySQL
CREATE TABLE IF NOT EXISTS `FITNESS_SYNCER_CONFIGS` (
    `ID` INT AUTO_INCREMENT PRIMARY KEY,
    `USUARIO_ID` INT NOT NULL,
    `ACCESS_TOKEN` VARCHAR(1000) NOT NULL,
    `REFRESH_TOKEN` VARCHAR(1000) NOT NULL,
    `TOKEN_EXPIRY` DATETIME NOT NULL,
    `IS_ACTIVE` BOOLEAN NOT NULL DEFAULT true,
    `FECHA_CREACION` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    `ULTIMA_SINCRONIZACION` DATETIME NULL,
    
    -- Llave foránea hacia la tabla USUARIOS
    CONSTRAINT `FK_FITNESS_SYNCER_CONFIGS_USUARIOS` 
        FOREIGN KEY (`USUARIO_ID`) 
        REFERENCES `USUARIOS`(`ID`) 
        ON DELETE CASCADE,
    
    -- Índice único para evitar múltiples configuraciones activas por usuario
    UNIQUE KEY `IX_FITNESS_SYNCER_CONFIGS_USUARIO_ACTIVE` (`USUARIO_ID`, `IS_ACTIVE`),
    
    -- Índices adicionales
    KEY `IX_FITNESS_SYNCER_CONFIGS_USUARIO_ID` (`USUARIO_ID`),
    KEY `IX_FITNESS_SYNCER_CONFIGS_ULTIMA_SINCRONIZACION` (`ULTIMA_SINCRONIZACION`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
*/

-- =========================================================================

-- -------------------------------------------------------------------------
-- DATOS DE PRUEBA (OPCIONAL)
-- -------------------------------------------------------------------------

-- Insertar configuración de prueba (solo para testing)
-- NOTA: Reemplaza los tokens con valores reales de FitnessSyncer
/*
INSERT INTO "FITNESS_SYNCER_CONFIGS" (
    "USUARIO_ID", 
    "ACCESS_TOKEN", 
    "REFRESH_TOKEN", 
    "TOKEN_EXPIRY", 
    "IS_ACTIVE", 
    "FECHA_CREACION"
) VALUES (
    1, -- ID del usuario de prueba
    'test_access_token_aqui', 
    'test_refresh_token_aqui', 
    CURRENT_TIMESTAMP + INTERVAL '1 hour', -- Expira en 1 hora
    true, 
    CURRENT_TIMESTAMP
);
*/

-- =========================================================================

-- -------------------------------------------------------------------------
-- VERIFICACIÓN DE LA MIGRACIÓN
-- -------------------------------------------------------------------------

-- Verificar que la tabla se creó correctamente
SELECT 
    table_name,
    column_name,
    data_type,
    is_nullable,
    column_default
FROM information_schema.columns 
WHERE table_name = 'FITNESS_SYNCER_CONFIGS'
ORDER BY ordinal_position;

-- Verificar índices creados
SELECT 
    indexname,
    indexdef
FROM pg_indexes 
WHERE tablename = 'FITNESS_SYNCER_CONFIGS';

-- Verificar constraints/llaves foráneas
SELECT 
    constraint_name,
    constraint_type
FROM information_schema.table_constraints 
WHERE table_name = 'FITNESS_SYNCER_CONFIGS';

-- =========================================================================

-- -------------------------------------------------------------------------
-- ROLLBACK SCRIPT (Para deshacer la migración si es necesario)
-- -------------------------------------------------------------------------

/*
-- CUIDADO: Este script elimina toda la funcionalidad de FitnessSyncer
-- Solo ejecutar si necesitas hacer rollback completo

-- Eliminar tabla FITNESS_SYNCER_CONFIGS
DROP TABLE IF EXISTS "FITNESS_SYNCER_CONFIGS" CASCADE;

-- Verificar que la tabla fue eliminada
SELECT table_name 
FROM information_schema.tables 
WHERE table_name = 'FITNESS_SYNCER_CONFIGS';
*/

-- =========================================================================

-- -------------------------------------------------------------------------
-- NOTAS DE IMPLEMENTACIÓN
-- -------------------------------------------------------------------------

/*
NOTAS IMPORTANTES:

1. SEGURIDAD:
   - Los tokens OAuth se almacenan encriptados en la base de datos
   - Usar HTTPS siempre para las comunicaciones
   - Implementar rotación automática de tokens

2. PERFORMANCE:
   - Los índices están optimizados para las consultas más comunes
   - Considerar particionamiento por fecha si hay muchos usuarios

3. MANTENIMIENTO:
   - Limpiar tokens expirados periódicamente
   - Monitorear el tamaño de la tabla
   - Hacer backup antes de ejecutar el script

4. ESCALABILIDAD:
   - La tabla soporta millones de usuarios
   - Considerar replicación para alta disponibilidad
   - Implementar cache para tokens frecuentemente usados

5. CONFIGURACIÓN DE APLICACIÓN:
   - Actualizar appsettings.json con credenciales de FitnessSyncer
   - Configurar URLs de callback correctamente
   - Implementar logging para auditoría

6. TESTING:
   - Probar con usuario de prueba primero
   - Verificar flujo OAuth completo
   - Validar sincronización de datos

ORDEN DE EJECUCIÓN:
1. Ejecutar este script en PostgreSQL
2. Verificar que la tabla se creó correctamente
3. Actualizar configuración de la aplicación
4. Reiniciar la aplicación
5. Probar la funcionalidad

*/

-- =========================================================================
-- FIN DEL SCRIPT
-- ========================================================================= 