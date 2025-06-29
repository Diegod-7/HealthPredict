# Script simple para crear un usuario de prueba
Write-Host "🚀 Creando usuario de prueba..." -ForegroundColor Green

# Verificar estado actual
$diagnostico = Invoke-RestMethod -Uri "https://healthpredict-l1hu.onrender.com/api/Usuarios/diagnostico-bd" -Method Get
Write-Host "Total usuarios actuales: $($diagnostico.baseDatos.totalUsuarios)" -ForegroundColor Cyan

# Crear usuario Diego
$body = @{
    nombre = "Diego"
    apellido = "Diaz"
    email = "diego.diaz@healthpredict.com"
    password = "diego123"
    fechaNacimiento = "1992-08-22T00:00:00"
    genero = "Masculino"
    altura = 175
    peso = 75
    rol = "Trabajador"
    departamento = "Desarrollo"
    cargo = "Desarrollador Full Stack"
} | ConvertTo-Json

Write-Host "Creando usuario Diego..." -ForegroundColor Yellow

try {
    $resultado = Invoke-RestMethod -Uri "https://healthpredict-l1hu.onrender.com/api/Usuarios" -Method Post -Body $body -ContentType "application/json"
    Write-Host "✅ Usuario creado exitosamente!" -ForegroundColor Green
    Write-Host "Email: diego.diaz@healthpredict.com" -ForegroundColor White
    Write-Host "Password: diego123" -ForegroundColor White
}
catch {
    Write-Host "❌ Error al crear usuario:" -ForegroundColor Red
    Write-Host $_.Exception.Message -ForegroundColor Yellow
    
    # Mostrar más detalles del error
    if ($_.ErrorDetails) {
        Write-Host "Detalles del error:" -ForegroundColor Red
        Write-Host $_.ErrorDetails.Message -ForegroundColor Yellow
    }
}

Write-Host "Script completado." -ForegroundColor Green 