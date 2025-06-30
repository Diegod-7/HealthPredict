# SOLUCIÓN RÁPIDA: Agregar columna CARGO con PowerShell
# Ejecutar este script en PowerShell

$connectionString = "Host=ep-royal-dream-a4izfmtv-pooler.us-east-1.aws.neon.tech;Database=neondb;Username=neondb_owner;Password=npg_h0oeEy6tmXsf;SSL Mode=Require"

try {
    Write-Host "🔧 Conectando a PostgreSQL..." -ForegroundColor Yellow
    
    # Instalar Npgsql si no está disponible
    if (-not (Get-Module -ListAvailable -Name "Npgsql")) {
        Write-Host "📦 Instalando Npgsql..." -ForegroundColor Blue
        Install-Package Npgsql -Force -Scope CurrentUser
    }
    
    Add-Type -Path (Get-Package Npgsql).Source
    
    $connection = New-Object Npgsql.NpgsqlConnection($connectionString)
    $connection.Open()
    
    Write-Host "✅ Conectado exitosamente" -ForegroundColor Green
    
    # Verificar si columna CARGO existe
    $checkCommand = $connection.CreateCommand()
    $checkCommand.CommandText = "SELECT COUNT(*) FROM information_schema.columns WHERE table_name = 'USUARIOS' AND column_name = 'CARGO'"
    $exists = $checkCommand.ExecuteScalar()
    
    if ($exists -eq 0) {
        Write-Host "➕ Agregando columna CARGO..." -ForegroundColor Blue
        
        $addCommand = $connection.CreateCommand()
        $addCommand.CommandText = 'ALTER TABLE "USUARIOS" ADD COLUMN "CARGO" varchar(100)'
        $addCommand.ExecuteNonQuery()
        
        Write-Host "✅ Columna CARGO agregada exitosamente" -ForegroundColor Green
    } else {
        Write-Host "ℹ️  La columna CARGO ya existe" -ForegroundColor Cyan
    }
    
    # Actualizar registros con valores por defecto
    Write-Host "🔄 Actualizando registros existentes..." -ForegroundColor Blue
    
    $updateCommand = $connection.CreateCommand()
    $updateCommand.CommandText = 'UPDATE "USUARIOS" SET "CARGO" = ''Desarrollador'' WHERE "CARGO" IS NULL'
    $updated = $updateCommand.ExecuteNonQuery()
    
    Write-Host "✅ Se actualizaron $updated registros" -ForegroundColor Green
    
    # Verificar resultado
    $verifyCommand = $connection.CreateCommand()
    $verifyCommand.CommandText = 'SELECT "ID", "NOMBRE", "APELLIDO", "CARGO" FROM "USUARIOS" LIMIT 5'
    $reader = $verifyCommand.ExecuteReader()
    
    Write-Host "`n📊 Primeros 5 usuarios:" -ForegroundColor Cyan
    while ($reader.Read()) {
        Write-Host "  ID: $($reader['ID']) | $($reader['NOMBRE']) $($reader['APELLIDO']) | Cargo: $($reader['CARGO'])" -ForegroundColor White
    }
    
    $connection.Close()
    Write-Host "`n🎉 ¡Problema resuelto! Ahora puedes hacer login." -ForegroundColor Green
    
} catch {
    Write-Host "❌ Error: $($_.Exception.Message)" -ForegroundColor Red
} 