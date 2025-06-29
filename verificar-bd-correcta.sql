-- VERIFICAR QUE ESTAMOS EN LA BASE DE DATOS CORRECTA

-- 1. Verificar base de datos actual
SELECT current_database() as "Base de Datos Actual";

-- 2. Verificar que las tablas de HealthPredict existen
SELECT table_name as "Tablas Existentes"
FROM information_schema.tables 
WHERE table_schema = 'public'
ORDER BY table_name;

-- 3. Verificar estructura de tabla USUARIOS
SELECT column_name as "Columna", data_type as "Tipo", is_nullable as "Permite NULL"
FROM information_schema.columns 
WHERE table_name = 'USUARIOS'
ORDER BY ordinal_position;

-- 4. Contar usuarios actuales
SELECT COUNT(*) as "Total Usuarios Actuales" FROM "USUARIOS";

-- 5. Si hay usuarios, mostrarlos
SELECT "ID", "NOMBRE", "APELLIDO", "EMAIL", "ROL" 
FROM "USUARIOS" 
ORDER BY "ID";

-- RESULTADO ESPERADO:
-- - Base de datos: neondb
-- - Tablas: USUARIOS, ALERTAS, DATOS_VITALES, __EFMigrationsHistory
-- - Usuarios: 0 (por eso no puedes hacer login)

-- SI VES ESTO, ESTÁS EN LA BD CORRECTA Y PUEDES PROCEDER CON LA INSERCIÓN 