# Script para probar conexiones con HealthPredict API en producción

Write-Host "========================================" -ForegroundColor Green
Write-Host "   HealthPredict - Test Conexiones" -ForegroundColor Green  
Write-Host "========================================" -ForegroundColor Green
Write-Host ""

$baseUrl = "https://healthpredict-l1hu.onrender.com"

# Test 1: Swagger
Write-Host "🔍 1. Probando Swagger..." -ForegroundColor Yellow
try {
    $response = Invoke-WebRequest -Uri "$baseUrl/swagger" -UseBasicParsing -TimeoutSec 30
    if ($response.StatusCode -eq 200) {
        Write-Host "✅ Swagger funcionando correctamente" -ForegroundColor Green
    }
} catch {
    Write-Host "❌ Error en Swagger: $($_.Exception.Message)" -ForegroundColor Red
}

# Test 2: API Health Check
Write-Host ""
Write-Host "🔍 2. Probando API Health..." -ForegroundColor Yellow
try {
    $response = Invoke-WebRequest -Uri "$baseUrl/api/usuarios" -UseBasicParsing -TimeoutSec 30
    if ($response.StatusCode -eq 200) {
        Write-Host "✅ API funcionando correctamente" -ForegroundColor Green
        Write-Host "   Respuesta: $($response.Content)" -ForegroundColor Gray
    }
} catch {
    Write-Host "❌ Error en API: $($_.Exception.Message)" -ForegroundColor Red
}

# Test 3: Datos Vitales
Write-Host ""
Write-Host "🔍 3. Probando endpoint DatosVitales..." -ForegroundColor Yellow
try {
    $response = Invoke-WebRequest -Uri "$baseUrl/api/datosvitales" -UseBasicParsing -TimeoutSec 30
    if ($response.StatusCode -eq 200) {
        Write-Host "✅ DatosVitales funcionando correctamente" -ForegroundColor Green
        Write-Host "   Respuesta: $($response.Content)" -ForegroundColor Gray
    }
} catch {
    Write-Host "❌ Error en DatosVitales: $($_.Exception.Message)" -ForegroundColor Red
}

# Test 4: Alertas
Write-Host ""
Write-Host "🔍 4. Probando endpoint Alertas..." -ForegroundColor Yellow
try {
    $response = Invoke-WebRequest -Uri "$baseUrl/api/alertas" -UseBasicParsing -TimeoutSec 30
    if ($response.StatusCode -eq 200) {
        Write-Host "✅ Alertas funcionando correctamente" -ForegroundColor Green
        Write-Host "   Respuesta: $($response.Content)" -ForegroundColor Gray
    }
} catch {
    Write-Host "❌ Error en Alertas: $($_.Exception.Message)" -ForegroundColor Red
}

# Test 5: Frontend Angular (si está corriendo)
Write-Host ""
Write-Host "🔍 5. Probando Frontend Angular..." -ForegroundColor Yellow
try {
    $response = Invoke-WebRequest -Uri "http://localhost:4200" -UseBasicParsing -TimeoutSec 10
    if ($response.StatusCode -eq 200) {
        Write-Host "✅ Frontend Angular funcionando correctamente" -ForegroundColor Green
    }
} catch {
    Write-Host "⚠️ Frontend Angular no está corriendo localmente" -ForegroundColor Yellow
}

Write-Host ""
Write-Host "========================================" -ForegroundColor Green
Write-Host "   Pruebas Completadas" -ForegroundColor Green
Write-Host "========================================" -ForegroundColor Green
Write-Host ""
Write-Host "🔗 URLs importantes:" -ForegroundColor Cyan
Write-Host "   API Base: $baseUrl/api" -ForegroundColor Gray
Write-Host "   Swagger: $baseUrl/swagger" -ForegroundColor Gray
Write-Host "   Frontend: http://localhost:4200" -ForegroundColor Gray
Write-Host "" 