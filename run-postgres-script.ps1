# Script para ejecutar el SQL de inicialización usando psql
Write-Host "🐘 EJECUTANDO SCRIPT SQL EN POSTGRESQL" -ForegroundColor Cyan
Write-Host "======================================" -ForegroundColor Cyan

# Configuración de conexión
$pgHost = "ep-royal-dream-a4izfmtv-pooler.us-east-1.aws.neon.tech"
$database = "neondb"
$username = "neondb_owner"
$password = "npg_h0oeEy6tmXsf"

Write-Host "🔍 Verificando si psql está disponible..." -ForegroundColor Yellow

# Verificar si psql está instalado
try {
    $psqlVersion = psql --version 2>$null
    if ($LASTEXITCODE -eq 0) {
        Write-Host "✅ psql encontrado: $psqlVersion" -ForegroundColor Green
    } else {
        throw "psql no encontrado"
    }
}
catch {
    Write-Host "❌ psql no está instalado o no está en el PATH" -ForegroundColor Red
    Write-Host "📥 Para instalar PostgreSQL client:" -ForegroundColor Yellow
    Write-Host "   1. Descarga desde: https://www.postgresql.org/download/windows/" -ForegroundColor White
    Write-Host "   2. O instala con Chocolatey: choco install postgresql" -ForegroundColor White
    Write-Host "   3. O instala con Scoop: scoop install postgresql" -ForegroundColor White
    Write-Host ""
    Write-Host "🔄 ALTERNATIVA: Ejecutar SQL manualmente" -ForegroundColor Cyan
    Write-Host "=================================" -ForegroundColor Cyan
    Write-Host "Puedes copiar y pegar este SQL en cualquier cliente PostgreSQL:" -ForegroundColor White
    Write-Host ""
    Get-Content "init-postgres-psql.sql" | Write-Host -ForegroundColor Gray
    exit
}

Write-Host "`n🚀 Ejecutando script SQL..." -ForegroundColor Green

# Configurar variable de entorno para la contraseña
$env:PGPASSWORD = $password

# Ejecutar el script SQL
try {
    Write-Host "Conectando a PostgreSQL..." -ForegroundColor Yellow
    $result = psql -h $pgHost -d $database -U $username -f "init-postgres-psql.sql" 2>&1
    
    if ($LASTEXITCODE -eq 0) {
        Write-Host "✅ Script ejecutado exitosamente!" -ForegroundColor Green
        Write-Host "Resultado:" -ForegroundColor Cyan
        $result | Write-Host -ForegroundColor White
        
        Write-Host "`n🎉 ¡USUARIOS CREADOS!" -ForegroundColor Green
        Write-Host "===================" -ForegroundColor Green
        Write-Host "Ya puedes usar estas credenciales para login:" -ForegroundColor White
        Write-Host ""
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
        Write-Host ""
        Write-Host "🌐 URL de login: https://healthpredict-l1hu.onrender.com/api/usuarios/authenticate" -ForegroundColor Cyan
    } else {
        Write-Host "❌ Error al ejecutar el script:" -ForegroundColor Red
        $result | Write-Host -ForegroundColor Yellow
    }
}
catch {
    Write-Host "❌ Error al ejecutar psql:" -ForegroundColor Red
    Write-Host $_.Exception.Message -ForegroundColor Yellow
}
finally {
    # Limpiar variable de entorno
    Remove-Item env:PGPASSWORD -ErrorAction SilentlyContinue
}

Write-Host "`n✅ Script completado." -ForegroundColor Green 