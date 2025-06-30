-- Script de solución rápida para el error de columna CARGO faltante
-- Ejecutar en cualquier cliente PostgreSQL (pgAdmin, DBeaver, etc.)

-- Verificar si la columna CARGO existe
SELECT column_name 
FROM information_schema.columns 
WHERE table_name = 'USUARIOS' AND column_name = 'CARGO';

-- Agregar la columna CARGO si no existe 
-- Usar DO block para verificar existencia
DO $$
BEGIN
    IF NOT EXISTS (
        SELECT 1 FROM information_schema.columns 
        WHERE table_name = 'USUARIOS' AND column_name = 'CARGO'
    ) THEN
        ALTER TABLE "USUARIOS" ADD COLUMN "CARGO" varchar(100);
    END IF;
END $$;

-- Verificar que se agregó correctamente
SELECT column_name, data_type, character_maximum_length 
FROM information_schema.columns 
WHERE table_name = 'USUARIOS' AND column_name = 'CARGO';

-- Opcional: Actualizar registros existentes con valores por defecto
UPDATE "USUARIOS" 
SET "CARGO" = 'Desarrollador' 
WHERE "CARGO" IS NULL AND "ROL" = 'Trabajador';

UPDATE "USUARIOS" 
SET "CARGO" = 'Gerente General' 
WHERE "CARGO" IS NULL AND "ROL" = 'Jefe';

-- Verificar los datos
SELECT "ID", "NOMBRE", "APELLIDO", "ROL", "CARGO" FROM "USUARIOS"; 