# Script simple para insertar usuarios en PostgreSQL
Write-Host "Insertando usuarios en PostgreSQL..." -ForegroundColor Green

# Datos de conexión
$pgHost = "ep-royal-dream-a4izfmtv-pooler.us-east-1.aws.neon.tech"
$database = "neondb"
$username = "neondb_owner"
$password = "npg_h0oeEy6tmXsf"

# Configurar contraseña
$env:PGPASSWORD = $password

Write-Host "Ejecutando SQL..." -ForegroundColor Yellow

# Comando SQL directo
$sqlCommand = @"
INSERT INTO "USUARIOS" (
    "NOMBRE", "APELLIDO", "EMAIL", "PASSWORD", 
    "FECHA_NACIMIENTO", "GENERO", "ALTURA", "PESO", 
    "FECHA_REGISTRO", "ULTIMO_ACCESO", "ES_PROFESIONAL_MEDICO",
    "ROL", "DEPARTAMENTO", "CARGO", "JEFE_ID", "ES_ACTIVO"
) VALUES 
('Carlos', 'Rodriguez', 'carlos.rodriguez@healthpredict.com', 'admin123',
 '1985-03-15', 'Masculino', 178, 80.0,
 NOW(), NOW(), false,
 'Jefe', 'Administracion', 'Gerente General', NULL, true),
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
"@

try {
    psql -h $pgHost -d $database -U $username -c $sqlCommand
    
    if ($LASTEXITCODE -eq 0) {
        Write-Host "Usuarios insertados exitosamente!" -ForegroundColor Green
        Write-Host ""
        Write-Host "CREDENCIALES DE LOGIN:" -ForegroundColor Cyan
        Write-Host "carlos.rodriguez@healthpredict.com / admin123 (Jefe)" -ForegroundColor White
        Write-Host "diego.diaz@healthpredict.com / diego123 (Trabajador)" -ForegroundColor White
        Write-Host "matias.maripangue@healthpredict.com / matias123 (Trabajador)" -ForegroundColor White
        Write-Host "iahn.vera@healthpredict.com / iahn123 (Trabajador)" -ForegroundColor White
    } else {
        Write-Host "Error al insertar usuarios" -ForegroundColor Red
    }
} catch {
    Write-Host "Error: psql no encontrado o no instalado" -ForegroundColor Red
    Write-Host "Instala PostgreSQL client desde: https://www.postgresql.org/download/" -ForegroundColor Yellow
}

# Limpiar contraseña
Remove-Item env:PGPASSWORD -ErrorAction SilentlyContinue

Write-Host "Script completado." -ForegroundColor Green 