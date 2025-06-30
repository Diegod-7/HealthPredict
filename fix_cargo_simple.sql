-- SOLUCIÓN RÁPIDA: Agregar columna CARGO faltante
-- Ejecutar este script en tu cliente PostgreSQL

-- 1. Verificar estructura actual
SELECT column_name 
FROM information_schema.columns 
WHERE table_name = 'USUARIOS' 
ORDER BY ordinal_position;

-- 2. Agregar columna CARGO (si ya existe, PostgreSQL mostrará un error pero no romperá nada)
ALTER TABLE "USUARIOS" ADD COLUMN "CARGO" varchar(100);

-- 3. Agregar columnas adicionales que también podrían faltar
ALTER TABLE "USUARIOS" ADD COLUMN "DEPARTAMENTO" varchar(100);
ALTER TABLE "USUARIOS" ADD COLUMN "ROL" varchar(20) DEFAULT 'Trabajador';
ALTER TABLE "USUARIOS" ADD COLUMN "JEFE_ID" integer;
ALTER TABLE "USUARIOS" ADD COLUMN "ES_ACTIVO" boolean DEFAULT true;

-- 4. Actualizar datos existentes con valores por defecto
UPDATE "USUARIOS" SET "CARGO" = 'Desarrollador' WHERE "CARGO" IS NULL;
UPDATE "USUARIOS" SET "DEPARTAMENTO" = 'Desarrollo' WHERE "DEPARTAMENTO" IS NULL;
UPDATE "USUARIOS" SET "ROL" = 'Trabajador' WHERE "ROL" IS NULL;
UPDATE "USUARIOS" SET "ES_ACTIVO" = true WHERE "ES_ACTIVO" IS NULL;

-- 5. Verificar resultado
SELECT "ID", "NOMBRE", "APELLIDO", "EMAIL", "ROL", "CARGO", "DEPARTAMENTO" 
FROM "USUARIOS"; 