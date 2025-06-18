@echo off
echo ========================================
echo    HealthPredict API + PostgreSQL
echo ========================================
echo.

cd HealthPredict.API

echo 1. Compilando aplicación...
dotnet build
if %errorlevel% neq 0 (
    echo ❌ Error al compilar
    pause
    exit /b 1
)

echo.
echo 2. Iniciando API en puerto 5000...
echo   💡 Swagger disponible en: http://localhost:5000/swagger
echo   💡 API Base URL: http://localhost:5000/api
echo.

start "HealthPredict API" cmd /k "dotnet run --urls=http://localhost:5000"

echo.
echo 3. Esperando 20 segundos para que la API inicie...
timeout /t 20 /nobreak > nul

echo.
echo 4. Probando conexión...
powershell -Command "try { $response = Invoke-WebRequest -Uri 'http://localhost:5000/swagger' -UseBasicParsing -TimeoutSec 10; Write-Host '✅ API funcionando correctamente!' -ForegroundColor Green; Write-Host 'Status Code:' $response.StatusCode } catch { Write-Host '❌ Error:' $_.Exception.Message -ForegroundColor Red }"

echo.
echo 5. Probando endpoint de usuarios...
powershell -Command "try { $response = Invoke-WebRequest -Uri 'http://localhost:5000/api/usuarios' -UseBasicParsing -TimeoutSec 10; Write-Host '✅ Endpoint usuarios funcionando!' -ForegroundColor Green; Write-Host 'Status Code:' $response.StatusCode } catch { Write-Host '❌ Error en usuarios:' $_.Exception.Message -ForegroundColor Red }"

echo.
echo ========================================
echo ✅ ¡API con PostgreSQL funcionando!
echo 📱 Puedes usar esta URL en tus apps:
echo    http://localhost:5000/api
echo 🌐 Swagger: http://localhost:5000/swagger
echo ========================================
pause 