-- SCRIPT PARA ACTUALIZAR USUARIOS EXISTENTES
-- Datos actuales: Carlos (ID 6), Diego (ID 7), Matias (ID 8), Iahn (ID 9)

-- 1. Primero agregar las columnas si no existen (ignorar errores si ya existen)
ALTER TABLE "USUARIOS" ADD COLUMN IF NOT EXISTS "ROL" varchar(20);
ALTER TABLE "USUARIOS" ADD COLUMN IF NOT EXISTS "DEPARTAMENTO" varchar(100);
ALTER TABLE "USUARIOS" ADD COLUMN IF NOT EXISTS "CARGO" varchar(100);
ALTER TABLE "USUARIOS" ADD COLUMN IF NOT EXISTS "JEFE_ID" integer;
ALTER TABLE "USUARIOS" ADD COLUMN IF NOT EXISTS "ES_ACTIVO" boolean;

-- 2. Actualizar Carlos Rodriguez (ID 6) - JEFE
UPDATE "USUARIOS" 
SET "ROL" = 'Jefe',
    "DEPARTAMENTO" = 'Administración',
    "CARGO" = 'Gerente General',
    "JEFE_ID" = NULL,
    "ES_ACTIVO" = true
WHERE "ID" = 6 AND "EMAIL" = 'carlos.rodriguez@healthpredict.com';

-- 3. Actualizar Diego Diaz (ID 7) - TRABAJADOR
UPDATE "USUARIOS" 
SET "ROL" = 'Trabajador',
    "DEPARTAMENTO" = 'Desarrollo',
    "CARGO" = 'Desarrollador Full Stack',
    "JEFE_ID" = 6,
    "ES_ACTIVO" = true
WHERE "ID" = 7 AND "EMAIL" = 'diego.diaz@healthpredict.com';

-- 4. Actualizar Matias Maripangue (ID 8) - TRABAJADOR
UPDATE "USUARIOS" 
SET "ROL" = 'Trabajador',
    "DEPARTAMENTO" = 'Desarrollo',
    "CARGO" = 'Desarrollador Backend',
    "JEFE_ID" = 6,
    "ES_ACTIVO" = true
WHERE "ID" = 8 AND "EMAIL" = 'matias.maripangue@healthpredict.com';

-- 5. Actualizar Iahn Vera (ID 9) - TRABAJADOR
UPDATE "USUARIOS" 
SET "ROL" = 'Trabajador',
    "DEPARTAMENTO" = 'Desarrollo',
    "CARGO" = 'Desarrollador Frontend',
    "JEFE_ID" = 6,
    "ES_ACTIVO" = true
WHERE "ID" = 9 AND "EMAIL" = 'iahn.vera@healthpredict.com';

-- 6. Verificar los cambios
SELECT "ID", "NOMBRE", "APELLIDO", "EMAIL", "ROL", "DEPARTAMENTO", "CARGO", "JEFE_ID", "ES_ACTIVO"
FROM "USUARIOS" 
WHERE "ID" IN (6, 7, 8, 9)
ORDER BY "ID";

-- 7. Verificar estructura final de la tabla
SELECT column_name, data_type, is_nullable
FROM information_schema.columns 
WHERE table_name = 'USUARIOS'
ORDER BY ordinal_position; 