-- SQL CORREGIDO para insertar usuarios (resuelve problema de foreign keys)
-- Primero inserta el jefe, luego los trabajadores con referencia correcta

-- Verificar estado inicial
SELECT current_database() as "Base de Datos Actual";
SELECT COUNT(*) as "Total Usuarios Antes" FROM "USUARIOS";

-- PASO 1: Insertar al JEFE primero (sin JEFE_ID)
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

-- Verificar que el jefe se insertó
SELECT "ID", "NOMBRE", "EMAIL" FROM "USUARIOS" WHERE "EMAIL" = 'carlos.rodriguez@healthpredict.com';

-- PASO 2: Obtener el ID del jefe para usarlo en las referencias
-- (En PostgreSQL, podemos usar una subconsulta para obtener el ID)

-- PASO 3: Insertar trabajadores con referencia al jefe
INSERT INTO "USUARIOS" (
    "NOMBRE", "APELLIDO", "EMAIL", "PASSWORD", 
    "FECHA_NACIMIENTO", "GENERO", "ALTURA", "PESO", 
    "FECHA_REGISTRO", "ULTIMO_ACCESO", "ES_PROFESIONAL_MEDICO",
    "ROL", "DEPARTAMENTO", "CARGO", "JEFE_ID", "ES_ACTIVO"
)
SELECT 'Diego', 'Diaz', 'diego.diaz@healthpredict.com', 'diego123',
       '1992-08-22', 'Masculino', 175, 75.0,
       NOW(), NOW(), false,
       'Trabajador', 'Desarrollo', 'Desarrollador Full Stack', 
       (SELECT "ID" FROM "USUARIOS" WHERE "EMAIL" = 'carlos.rodriguez@healthpredict.com'), true
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
       'Trabajador', 'Desarrollo', 'Desarrollador Backend', 
       (SELECT "ID" FROM "USUARIOS" WHERE "EMAIL" = 'carlos.rodriguez@healthpredict.com'), true
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
       'Trabajador', 'Desarrollo', 'Desarrollador Frontend', 
       (SELECT "ID" FROM "USUARIOS" WHERE "EMAIL" = 'carlos.rodriguez@healthpredict.com'), true
WHERE NOT EXISTS (
    SELECT 1 FROM "USUARIOS" WHERE "EMAIL" = 'iahn.vera@healthpredict.com'
);

-- Verificar resultado final
SELECT COUNT(*) as "Total Usuarios Después" FROM "USUARIOS";
SELECT "ID", "NOMBRE", "APELLIDO", "EMAIL", "ROL", "JEFE_ID" FROM "USUARIOS" ORDER BY "ID";

-- Verificar relaciones jefe-trabajador
SELECT 
    t."NOMBRE" as "Trabajador",
    t."EMAIL" as "Email_Trabajador",
    j."NOMBRE" as "Jefe",
    j."EMAIL" as "Email_Jefe"
FROM "USUARIOS" t
LEFT JOIN "USUARIOS" j ON t."JEFE_ID" = j."ID"
ORDER BY t."ID";

-- CREDENCIALES PARA LOGIN:
-- carlos.rodriguez@healthpredict.com / admin123 (Jefe)
-- diego.diaz@healthpredict.com / diego123 (Trabajador)
-- matias.maripangue@healthpredict.com / matias123 (Trabajador)
-- iahn.vera@healthpredict.com / iahn123 (Trabajador) 