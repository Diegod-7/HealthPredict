# Script para insertar usuarios directamente en PostgreSQL
# Requiere el módulo PostgreSQL para PowerShell

Write-Host "🐘 INICIALIZACIÓN DIRECTA EN POSTGRESQL" -ForegroundColor Cyan
Write-Host "=======================================" -ForegroundColor Cyan

# Configuración de conexión a PostgreSQL
$connectionString = "Host=ep-royal-dream-a4izfmtv-pooler.us-east-1.aws.neon.tech;Database=neondb;Username=neondb_owner;Password=npg_h0oeEy6tmXsf;SSL Mode=Require;Trust Server Certificate=true"

# Verificar si el módulo PostgreSQL está instalado
try {
    Import-Module -Name Npgsql -ErrorAction Stop
    Write-Host "✅ Módulo PostgreSQL cargado" -ForegroundColor Green
}
catch {
    Write-Host "❌ Módulo PostgreSQL no encontrado. Instalando..." -ForegroundColor Yellow
    try {
        Install-Module -Name Npgsql -Force -Scope CurrentUser -AllowClobber
        Import-Module -Name Npgsql
        Write-Host "✅ Módulo PostgreSQL instalado y cargado" -ForegroundColor Green
    }
    catch {
        Write-Host "❌ No se pudo instalar el módulo PostgreSQL" -ForegroundColor Red
        Write-Host "Ejecuta manualmente: Install-Module -Name Npgsql -Force" -ForegroundColor Yellow
        exit
    }
}

# SQL para insertar usuarios
$sqlInserts = @"
-- Insertar usuarios en la tabla USUARIOS
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
"@

# SQL para verificar usuarios existentes
$sqlCheck = "SELECT COUNT(*) as total FROM `"USUARIOS`";"

# SQL para mostrar usuarios creados
$sqlShow = "SELECT `"ID`", `"NOMBRE`", `"APELLIDO`", `"EMAIL`", `"ROL`" FROM `"USUARIOS`" ORDER BY `"ID`";"

Write-Host "`n🔍 Conectando a PostgreSQL..." -ForegroundColor Yellow

try {
    # Crear conexión
    $connection = New-Object Npgsql.NpgsqlConnection($connectionString)
    $connection.Open()
    Write-Host "✅ Conexión exitosa a PostgreSQL" -ForegroundColor Green

    # Verificar usuarios existentes
    Write-Host "`n📊 Verificando usuarios existentes..." -ForegroundColor Cyan
    $command = New-Object Npgsql.NpgsqlCommand($sqlCheck, $connection)
    $totalUsuarios = $command.ExecuteScalar()
    Write-Host "Total usuarios actuales: $totalUsuarios" -ForegroundColor White

    if ($totalUsuarios -gt 0) {
        Write-Host "`n⚠️ Ya existen usuarios en la base de datos." -ForegroundColor Yellow
        Write-Host "¿Deseas continuar y agregar usuarios adicionales? (s/n): " -NoNewline -ForegroundColor Yellow
        $respuesta = Read-Host
        if ($respuesta -ne 's' -and $respuesta -ne 'S') {
            Write-Host "Operación cancelada." -ForegroundColor Yellow
            $connection.Close()
            exit
        }
    }

    # Insertar usuarios
    Write-Host "`n🚀 Insertando usuarios..." -ForegroundColor Green
    $command = New-Object Npgsql.NpgsqlCommand($sqlInserts, $connection)
    $filasAfectadas = $command.ExecuteNonQuery()
    
    if ($filasAfectadas -gt 0) {
        Write-Host "✅ $filasAfectadas usuarios insertados exitosamente" -ForegroundColor Green
    } else {
        Write-Host "⚠️ No se insertaron nuevos usuarios (posiblemente ya existen)" -ForegroundColor Yellow
    }

    # Mostrar usuarios finales
    Write-Host "`n📋 Usuarios en la base de datos:" -ForegroundColor Cyan
    $command = New-Object Npgsql.NpgsqlCommand($sqlShow, $connection)
    $reader = $command.ExecuteReader()
    
    Write-Host "ID | NOMBRE    | APELLIDO   | EMAIL                              | ROL" -ForegroundColor White
    Write-Host "---|-----------|------------|------------------------------------|-----------" -ForegroundColor Gray
    
    while ($reader.Read()) {
        $id = $reader["ID"]
        $nombre = $reader["NOMBRE"]
        $apellido = $reader["APELLIDO"]  
        $email = $reader["EMAIL"]
        $rol = $reader["ROL"]
        Write-Host "$id  | $nombre | $apellido | $email | $rol" -ForegroundColor White
    }
    $reader.Close()

    Write-Host "`n🎉 ¡ÉXITO! Usuarios disponibles para login:" -ForegroundColor Green
    Write-Host "==========================================" -ForegroundColor Green
    Write-Host "👔 JEFE:" -ForegroundColor Blue
    Write-Host "   Email: carlos.rodriguez@healthpredict.com" -ForegroundColor White
    Write-Host "   Password: admin123" -ForegroundColor White
    Write-Host ""
    Write-Host "👨‍💻 TRABAJADORES:" -ForegroundColor Blue
    Write-Host "   Email: diego.diaz@healthpredict.com" -ForegroundColor White
    Write-Host "   Password: diego123" -ForegroundColor White
    Write-Host ""
    Write-Host "   Email: matias.maripangue@healthpredict.com" -ForegroundColor White
    Write-Host "   Password: matias123" -ForegroundColor White
    Write-Host ""
    Write-Host "   Email: iahn.vera@healthpredict.com" -ForegroundColor White
    Write-Host "   Password: iahn123" -ForegroundColor White

    Write-Host "`n🚀 ¡Ya puedes probar el login en tu aplicación!" -ForegroundColor Green
    Write-Host "URL de login: https://healthpredict-l1hu.onrender.com/api/usuarios/authenticate" -ForegroundColor Cyan

    $connection.Close()
}
catch {
    Write-Host "❌ Error al conectar/ejecutar en PostgreSQL:" -ForegroundColor Red
    Write-Host $_.Exception.Message -ForegroundColor Yellow
    
    if ($connection -and $connection.State -eq 'Open') {
        $connection.Close()
    }
}

Write-Host "`n✅ Script completado." -ForegroundColor Green 