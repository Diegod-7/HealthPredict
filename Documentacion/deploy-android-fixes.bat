@echo off
echo ========================================
echo   HealthPredict - Deploy Android Fixes
echo ========================================
echo.

echo 1. Compilando backend con mejoras de logging...
cd HealthPredict.API
dotnet build
if %errorlevel% neq 0 (
    echo ❌ Error al compilar backend
    pause
    exit /b 1
)

echo.
echo 2. Haciendo commit de los cambios...
cd ..
git add .
git commit -m "Mejorar logging Android - Debugging detallado para errores 400"

echo.
echo 3. Intentando push (opcional)...
git push
if %errorlevel% neq 0 (
    echo ⚠️ No se pudo hacer push automático
    echo 💡 Haz push manual cuando tengas acceso
)

echo.
echo ✅ ¡Cambios aplicados!
echo.
echo 🔧 Próximos pasos:
echo   1. Compilar app Android en Android Studio
echo   2. Probar sincronización
echo   3. Revisar logs detallados en Logcat
echo.
echo 📱 Para ver logs en Android:
echo   - Abrir Android Studio
echo   - Logcat tab
echo   - Filtrar por: "ANDROID DEBUG"
echo.
echo 🌐 Para ver logs del backend:
echo   - Ir a Render dashboard
echo   - Ver logs del servicio
echo   - Buscar: "[ANDROID DEBUG]"
echo.
pause 