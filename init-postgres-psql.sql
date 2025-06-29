-- Script SQL para insertar usuarios en PostgreSQL
-- Ejecutar con: psql -h ep-royal-dream-a4izfmtv-pooler.us-east-1.aws.neon.tech -d neondb -U neondb_owner -f init-postgres-psql.sql

-- Verificar usuarios existentes
SELECT 'Usuarios actuales en la base de datos:' as mensaje;
SELECT "ID", "NOMBRE", "APELLIDO", "EMAIL", "ROL" FROM "USUARIOS" ORDER BY "ID";

-- Insertar usuarios (ON CONFLICT previene duplicados)
INSERT INTO "USUARIOS" (
    "NOMBRE", "APELLIDO", "EMAIL", "PASSWORD", 
    "FECHA_NACIMIENTO", "GENERO", "ALTURA", "PESO", 
    "FECHA_REGISTRO", "ULTIMO_ACCESO", "ES_PROFESIONAL_MEDICO",
    "ROL", "DEPARTAMENTO", "CARGO", "JEFE_ID", "ES_ACTIVO"
) VALUES 
-- Jefe
('Carlos', 'Rodriguez', 'carlos.rodriguez@healthpredict.com', 'admin123',
 '1985-03-15', 'Masculino', 178, 80.0,
 NOW(), NOW(), false,
 'Jefe', 'Administración', 'Gerente General', NULL, true),

-- Trabajadores
('Diego', 'Diaz', 'diego.diaz@healthpredict.com', 'diego123',
 '1992-08-22', 'Masculino', 175, 75.0,
 NOW(), NOW(), false,
 'Trabajador', 'Desarrollo', 'Desarrollador Full Stack', 1, true),

('Matias', 'Maripangue', 'matias.maripangue@healthpredict.com', 'matias123',
 '1993-06-05', 'Masculino', 180, 82.0,
 NOW(), NOW(), false,
 'Trabajador', 'Desarrollo', 'Desarrollador Backend', 1, true),

('Iahn', 'Vera', 'iahn.vera@healthpredict.com', 'iahn123',
 '1994-11-10', 'Masculino', 172, 70.0,
 NOW(), NOW(), false,
 'Trabajador', 'Desarrollo', 'Desarrollador Frontend', 1, true)

ON CONFLICT ("EMAIL") DO NOTHING;

-- Mostrar usuarios finales
SELECT 'Usuarios después de la inserción:' as mensaje;
SELECT "ID", "NOMBRE", "APELLIDO", "EMAIL", "ROL" FROM "USUARIOS" ORDER BY "ID";

-- Mostrar credenciales
SELECT '=== CREDENCIALES DE LOGIN ===' as mensaje;
SELECT 
    "NOMBRE" || ' ' || "APELLIDO" as "USUARIO",
    "EMAIL" as "EMAIL", 
    "PASSWORD" as "PASSWORD",
    "ROL" as "ROL"
FROM "USUARIOS" 
ORDER BY "ROL" DESC, "ID"; 