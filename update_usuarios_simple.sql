-- SCRIPT SIMPLE PARA ACTUALIZAR USUARIOS EXISTENTES
-- Ejecutar en tu cliente PostgreSQL (pgAdmin, DBeaver, etc.)

-- 1. Agregar columnas (si ya existen, se mostrará error pero no afectará los datos)
ALTER TABLE "USUARIOS" ADD COLUMN "ROL" varchar(20);
ALTER TABLE "USUARIOS" ADD COLUMN "DEPARTAMENTO" varchar(100);
ALTER TABLE "USUARIOS" ADD COLUMN "CARGO" varchar(100);
ALTER TABLE "USUARIOS" ADD COLUMN "JEFE_ID" integer;
ALTER TABLE "USUARIOS" ADD COLUMN "ES_ACTIVO" boolean DEFAULT true;

-- 2. Actualizar Carlos Rodriguez (ID 6) - JEFE
UPDATE "USUARIOS" 
SET "ROL" = 'Jefe',
    "DEPARTAMENTO" = 'Administración',
    "CARGO" = 'Gerente General',
    "JEFE_ID" = NULL,
    "ES_ACTIVO" = true
WHERE "ID" = 6;

-- 3. Actualizar Diego Diaz (ID 7) - TRABAJADOR
UPDATE "USUARIOS" 
SET "ROL" = 'Trabajador',
    "DEPARTAMENTO" = 'Desarrollo',
    "CARGO" = 'Desarrollador Full Stack',
    "JEFE_ID" = 6,
    "ES_ACTIVO" = true
WHERE "ID" = 7;

-- 4. Actualizar Matias Maripangue (ID 8) - TRABAJADOR
UPDATE "USUARIOS" 
SET "ROL" = 'Trabajador',
    "DEPARTAMENTO" = 'Desarrollo',
    "CARGO" = 'Desarrollador Backend',
    "JEFE_ID" = 6,
    "ES_ACTIVO" = true
WHERE "ID" = 8;

-- 5. Actualizar Iahn Vera (ID 9) - TRABAJADOR
UPDATE "USUARIOS" 
SET "ROL" = 'Trabajador',
    "DEPARTAMENTO" = 'Desarrollo',
    "CARGO" = 'Desarrollador Frontend',
    "JEFE_ID" = 6,
    "ES_ACTIVO" = true
WHERE "ID" = 9;

-- 6. Verificar los cambios
SELECT "ID", "NOMBRE", "APELLIDO", "EMAIL", "ROL", "DEPARTAMENTO", "CARGO", "JEFE_ID", "ES_ACTIVO"
FROM "USUARIOS" 
WHERE "ID" IN (6, 7, 8, 9)
ORDER BY "ID"; 