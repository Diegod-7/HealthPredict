-- SQL SIMPLE: Insertar usuarios SIN relaciones de jefe
-- Esto evita completamente el problema de foreign keys
-- Después se pueden agregar las relaciones si es necesario

-- Verificar estado inicial
SELECT current_database() as "Base de Datos Actual";
SELECT COUNT(*) as "Total Usuarios Antes" FROM "USUARIOS";

-- Insertar TODOS los usuarios con JEFE_ID = NULL
INSERT INTO "USUARIOS" (
    "NOMBRE", "APELLIDO", "EMAIL", "PASSWORD", 
    "FECHA_NACIMIENTO", "GENERO", "ALTURA", "PESO", 
    "FECHA_REGISTRO", "ULTIMO_ACCESO", "ES_PROFESIONAL_MEDICO",
    "ROL", "DEPARTAMENTO", "CARGO", "JEFE_ID", "ES_ACTIVO"
)
SELECT 'Carlos', 'Rodriguez', 'carlos.rodriguez@healthpredict.com', 'admin123',
       '1985-03-15', 'Masculino', 178, 80.0,
       NOW(), NOW(), false,
       'Jefe', 'Administración', 'Gerente General', NULL, true
WHERE NOT EXISTS (
    SELECT 1 FROM "USUARIOS" WHERE "EMAIL" = 'carlos.rodriguez@healthpredict.com'
);

INSERT INTO "USUARIOS" (
    "NOMBRE", "APELLIDO", "EMAIL", "PASSWORD", 
    "FECHA_NACIMIENTO", "GENERO", "ALTURA", "PESO", 
    "FECHA_REGISTRO", "ULTIMO_ACCESO", "ES_PROFESIONAL_MEDICO",
    "ROL", "DEPARTAMENTO", "CARGO", "JEFE_ID", "ES_ACTIVO"
)
SELECT 'Diego', 'Diaz', 'diego.diaz@healthpredict.com', 'diego123',
       '1992-08-22', 'Masculino', 175, 75.0,
       NOW(), NOW(), false,
       'Trabajador', 'Desarrollo', 'Desarrollador Full Stack', NULL, true
WHERE NOT EXISTS (
    SELECT 1 FROM "USUARIOS" WHERE "EMAIL" = 'diego.diaz@healthpredict.com'
);

INSERT INTO "USUARIOS" (
    "NOMBRE", "APELLIDO", "EMAIL", "PASSWORD", 
    "FECHA_NACIMIENTO", "GENERO", "ALTURA", "PESO", 
    "FECHA_REGISTRO", "ULTIMO_ACCESO", "ES_PROFESIONAL_MEDICO",
    "ROL", "DEPARTAMENTO", "CARGO", "JEFE_ID", "ES_ACTIVO"
)
SELECT 'Matias', 'Maripangue', 'matias.maripangue@healthpredict.com', 'matias123',
       '1993-06-05', 'Masculino', 180, 82.0,
       NOW(), NOW(), false,
       'Trabajador', 'Desarrollo', 'Desarrollador Backend', NULL, true
WHERE NOT EXISTS (
    SELECT 1 FROM "USUARIOS" WHERE "EMAIL" = 'matias.maripangue@healthpredict.com'
);

INSERT INTO "USUARIOS" (
    "NOMBRE", "APELLIDO", "EMAIL", "PASSWORD", 
    "FECHA_NACIMIENTO", "GENERO", "ALTURA", "PESO", 
    "FECHA_REGISTRO", "ULTIMO_ACCESO", "ES_PROFESIONAL_MEDICO",
    "ROL", "DEPARTAMENTO", "CARGO", "JEFE_ID", "ES_ACTIVO"
)
SELECT 'Iahn', 'Vera', 'iahn.vera@healthpredict.com', 'iahn123',
       '1994-11-10', 'Masculino', 172, 70.0,
       NOW(), NOW(), false,
       'Trabajador', 'Desarrollo', 'Desarrollador Frontend', NULL, true
WHERE NOT EXISTS (
    SELECT 1 FROM "USUARIOS" WHERE "EMAIL" = 'iahn.vera@healthpredict.com'
);

-- Verificar resultado
SELECT COUNT(*) as "Total Usuarios Después" FROM "USUARIOS";
SELECT "ID", "NOMBRE", "APELLIDO", "EMAIL", "ROL" FROM "USUARIOS" ORDER BY "ID";

-- CREDENCIALES PARA LOGIN:
-- carlos.rodriguez@healthpredict.com / admin123 (Jefe)
-- diego.diaz@healthpredict.com / diego123 (Trabajador)
-- matias.maripangue@healthpredict.com / matias123 (Trabajador)
-- iahn.vera@healthpredict.com / iahn123 (Trabajador) 