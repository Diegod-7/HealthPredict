@echo off
echo ========================================
echo  HealthPredict API + Cloudflare Tunnel
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
echo 4. Iniciando Cloudflare Tunnel...
echo   💡 Copia la URL HTTPS que aparezca en la ventana de Cloudflare
echo   💡 Úsala en tus apps Angular/Android/iOS
echo.
start "Cloudflare Tunnel" cmd /k "cloudflared tunnel --url http://localhost:5000"

echo.
echo ✅ ¡Listo! Tu API está expuesta online con Cloudflare
echo.
echo 📱 Para usar en tus apps:
echo    - Copia la URL de Cloudflare (ej: https://abc-def.trycloudflare.com)
echo    - Úsala como apiUrl en environment.ts
echo.
echo ⚡ Ventajas de Cloudflare:
echo    - Completamente GRATIS
echo    - Sin límites de tiempo
echo    - Mejor rendimiento
echo.
echo 🔄 Para reiniciar: cierra las ventanas y ejecuta este script otra vez
echo.
pause 