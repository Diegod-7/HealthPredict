# Script para verificar el estado de la aplicación en Render
# Autor: HealthPredict Team
# Fecha: 2025

Write-Host "🔍 DIAGNÓSTICO DE HEALTHPREDICT EN RENDER" -ForegroundColor Cyan
Write-Host "=========================================" -ForegroundColor Cyan

# Configurar variables
$ApiUrl = "https://healthpredict-api.onrender.com"
$Endpoints = @(
    "/api/Usuarios/diagnostico-bd",
    "/api/Usuarios",
    "/api/DatosVitales",
    "/swagger"
)

Write-Host "`n📋 INSTRUCCIONES PARA VER LOGS EN RENDER:" -ForegroundColor Yellow
Write-Host "1. Ve a https://dashboard.render.com" -ForegroundColor White
Write-Host "2. Selecciona tu servicio 'HealthPredict API'" -ForegroundColor White
Write-Host "3. Haz clic en la pestaña 'Logs' en el menú lateral" -ForegroundColor White
Write-Host "4. Busca estos mensajes clave:" -ForegroundColor White
Write-Host "   🔍 PostgreSQL Connection String encontrado: [true/false]" -ForegroundColor Gray
Write-Host "   ✅ Conexión a la base de datos exitosa" -ForegroundColor Gray
Write-Host "   ❌ ERROR CRÍTICO al inicializar la base de datos" -ForegroundColor Gray
Write-Host "   🔄 Iniciando inicialización de la base de datos..." -ForegroundColor Gray

Write-Host "`n🌐 VERIFICANDO ESTADO DE LA API..." -ForegroundColor Green

# Función para hacer peticiones HTTP
function Test-Endpoint {
    param(
        [string]$Url,
        [string]$Name
    )
    
    try {
        Write-Host "Testing $Name..." -NoNewline
        $response = Invoke-RestMethod -Uri $Url -Method Get -TimeoutSec 10
        Write-Host " ✅ OK" -ForegroundColor Green
        return $response
    }
    catch {
        Write-Host " ❌ ERROR" -ForegroundColor Red
        Write-Host "   Error: $($_.Exception.Message)" -ForegroundColor Yellow
        return $null
    }
}

# Verificar cada endpoint
foreach ($endpoint in $Endpoints) {
    $fullUrl = $ApiUrl + $endpoint
    $endpointName = $endpoint.Replace("/api/", "").Replace("/", " -> ")
    
    if ($endpoint -eq "/api/Usuarios/diagnostico-bd") {
        Write-Host "`n🔧 DIAGNÓSTICO DE BASE DE DATOS:" -ForegroundColor Magenta
        $diagnostico = Test-Endpoint -Url $fullUrl -Name "Diagnóstico BD"
        
        if ($diagnostico) {
            Write-Host "📊 Resultados del diagnóstico:" -ForegroundColor Cyan
            Write-Host "   Timestamp: $($diagnostico.diagnostico.timestamp)" -ForegroundColor White
            Write-Host "   Connection String: $($diagnostico.diagnostico.connectionString)" -ForegroundColor White
            Write-Host "   Environment: $($diagnostico.diagnostico.environment)" -ForegroundColor White
            
            if ($diagnostico.conexion) {
                Write-Host "   Conexión: $($diagnostico.conexion.mensaje)" -ForegroundColor White
            }
            
            if ($diagnostico.baseDatos) {
                Write-Host "   Total Usuarios: $($diagnostico.baseDatos.totalUsuarios)" -ForegroundColor White
                Write-Host "   Total Tablas: $($diagnostico.baseDatos.totalTablas)" -ForegroundColor White
            }
            
            Write-Host "   Estado General: $($diagnostico.estado)" -ForegroundColor White
            
            if ($diagnostico.error) {
                Write-Host "   ❌ ERROR DETECTADO:" -ForegroundColor Red
                Write-Host "      Tipo: $($diagnostico.error.tipo)" -ForegroundColor Yellow
                Write-Host "      Mensaje: $($diagnostico.error.mensaje)" -ForegroundColor Yellow
            }
        }
    }
    else {
        Test-Endpoint -Url $fullUrl -Name $endpointName | Out-Null
    }
}

Write-Host "`n🔧 COMANDOS ÚTILES PARA RENDER:" -ForegroundColor Magenta
Write-Host "================================" -ForegroundColor Magenta
Write-Host "• Para forzar reinicio: Ve a Settings -> Manual Deploy" -ForegroundColor White
Write-Host "• Para ver variables de entorno: Settings -> Environment" -ForegroundColor White
Write-Host "• Para inicializar datos: POST /api/Usuarios/forzar-inicializacion" -ForegroundColor White

Write-Host "`n🚀 SOLUCIONES COMUNES:" -ForegroundColor Green
Write-Host "======================" -ForegroundColor Green
Write-Host "1. Si no hay conexión DB:" -ForegroundColor Yellow
Write-Host "   - Verificar variable DATABASE_URL en Render" -ForegroundColor White
Write-Host "   - Comprobar que Neon DB esté activo" -ForegroundColor White

Write-Host "`n2. Si hay usuarios pero no inicia sesión:" -ForegroundColor Yellow
Write-Host "   - Probar con: diego.diaz@healthpredict.com / diego123" -ForegroundColor White
Write-Host "   - O con: carlos.rodriguez@healthpredict.com / admin123" -ForegroundColor White

Write-Host "`n3. Para reinicializar datos:" -ForegroundColor Yellow
Write-Host "   - Usar endpoint: POST /api/Usuarios/forzar-inicializacion" -ForegroundColor White

Write-Host "`n📱 ENDPOINTS DE PRUEBA:" -ForegroundColor Blue
Write-Host "======================" -ForegroundColor Blue
Write-Host "• Swagger UI: $ApiUrl/swagger" -ForegroundColor White
Write-Host "• Diagnóstico: $ApiUrl/api/Usuarios/diagnostico-bd" -ForegroundColor White
Write-Host "• Usuarios: $ApiUrl/api/Usuarios" -ForegroundColor White
Write-Host "• Login: POST $ApiUrl/api/Usuarios/authenticate" -ForegroundColor White

Write-Host "`n✅ Diagnóstico completado!" -ForegroundColor Green
Write-Host "Si necesitas mas ayuda, revisa los logs en Render Dashboard." -ForegroundColor Cyan 