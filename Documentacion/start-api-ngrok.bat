@echo off
echo ========================================
echo    HealthPredict API + ngrok
echo ========================================
echo.

echo 1. Compilando imagen Docker...
docker build -t healthpredict-api .
if %errorlevel% neq 0 (
    echo ❌ Error al compilar Docker
    pause
    exit /b 1
)

echo.
echo 2. Iniciando API en Docker (puerto 5000)...
start "HealthPredict API" cmd /k "docker run -p 5000:80 healthpredict-api"

echo.
echo 3. Esperando 15 segundos para que la API inicie...
timeout /t 15 /nobreak > nul

echo.
echo 4. Iniciando ngrok...
echo   💡 Copia la URL HTTPS que aparezca en la ventana de ngrok
echo   💡 Úsala en tus apps Angular/Android/iOS
echo.
start "ngrok Tunnel" cmd /k "ngrok http 5000"

echo.
echo ✅ ¡Listo! Tu API está expuesta online
echo.
echo 📱 Para usar en tus apps:
echo    - Copia la URL de ngrok (ej: https://abc123.ngrok.io)
echo    - Úsala como apiUrl en environment.ts
echo.
echo 🔄 Para reiniciar: cierra las ventanas y ejecuta este script otra vez
echo.
pause 