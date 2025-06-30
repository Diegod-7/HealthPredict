-- Test: Insertar solo UN usuario para verificar que funciona

-- Verificar BD actual
SELECT current_database() as "Base de Datos Actual";

-- Verificar estructura de tabla
SELECT column_name, data_type, is_nullable
FROM information_schema.columns 
WHERE table_name = 'USUARIOS'
ORDER BY ordinal_position;

-- Verificar usuarios actuales
SELECT COUNT(*) as "Total Usuarios Antes" FROM "USUARIOS";

-- Insertar UN usuario de prueba
INSERT INTO "USUARIOS" (
    "NOMBRE", "APELLIDO", "EMAIL", "PASSWORD", 
    "FECHA_NACIMIENTO", "GENERO", "ALTURA", "PESO", 
    "FECHA_REGISTRO", "ULTIMO_ACCESO", "ES_PROFESIONAL_MEDICO",
    "ROL", "DEPARTAMENTO", "CARGO", "JEFE_ID", "ES_ACTIVO"
) VALUES (
    'Diego', 'Diaz', 'diego.diaz@healthpredict.com', 'diego123',
    '1992-08-22', 'Masculino', 175, 75.0,
    NOW(), NOW(), false,
    'Trabajador', 'Desarrollo', 'Desarrollador Full Stack', NULL, true
);

-- Verificar que se insertó
SELECT COUNT(*) as "Total Usuarios Después" FROM "USUARIOS";
SELECT * FROM "USUARIOS"; 