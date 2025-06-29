# Script para inicializar usuarios directamente via SQL
# Usar solo si los endpoints de la API fallan

Write-Host "🔧 INICIALIZACIÓN DIRECTA DE USUARIOS" -ForegroundColor Cyan
Write-Host "=====================================" -ForegroundColor Cyan

# Datos de los usuarios a crear
$usuarios = @(
    @{
        nombre = "Carlos"
        apellido = "Rodriguez"  
        email = "carlos.rodriguez@healthpredict.com"
        password = "admin123"
        fechaNacimiento = "1985-03-15"
        genero = "Masculino"
        altura = 178
        peso = 80.0
        rol = "Jefe"
        departamento = "Administración"
        cargo = "Gerente General"
        jefeId = $null
    },
    @{
        nombre = "Diego"
        apellido = "Diaz"
        email = "diego.diaz@healthpredict.com"
        password = "diego123"
        fechaNacimiento = "1992-08-22"
        genero = "Masculino"
        altura = 175
        peso = 75.0
        rol = "Trabajador"
        departamento = "Desarrollo"
        cargo = "Desarrollador Full Stack"
        jefeId = 1
    },
    @{
        nombre = "Matias"
        apellido = "Maripangue"
        email = "matias.maripangue@healthpredict.com"
        password = "matias123"
        fechaNacimiento = "1993-06-05"
        genero = "Masculino"
        altura = 180
        peso = 82.0
        rol = "Trabajador"
        departamento = "Desarrollo"
        cargo = "Desarrollador Backend"
        jefeId = 1
    },
    @{
        nombre = "Iahn"
        apellido = "Vera"
        email = "iahn.vera@healthpredict.com"
        password = "iahn123"
        fechaNacimiento = "1994-11-10"
        genero = "Masculino"
        altura = 172
        peso = 70.0
        rol = "Trabajador"
        departamento = "Desarrollo"
        cargo = "Desarrollador Frontend"
        jefeId = 1
    }
)

Write-Host "`n🔍 VERIFICANDO ESTADO ACTUAL..." -ForegroundColor Yellow

# Verificar estado actual
try {
    $diagnostico = Invoke-RestMethod -Uri "https://healthpredict-l1hu.onrender.com/api/Usuarios/diagnostico-bd" -Method Get
    
    Write-Host "📊 Estado actual:" -ForegroundColor Cyan
    Write-Host "   Conexión: $($diagnostico.conexion.mensaje)" -ForegroundColor White
    Write-Host "   Total Usuarios: $($diagnostico.baseDatos.totalUsuarios)" -ForegroundColor White
    Write-Host "   Estado: $($diagnostico.estado)" -ForegroundColor White
    
    if ($diagnostico.baseDatos.totalUsuarios -gt 0) {
        Write-Host "`n✅ Ya hay usuarios en la base de datos!" -ForegroundColor Green
        Write-Host "No es necesario ejecutar la inicialización." -ForegroundColor Green
        exit
    }
}
catch {
    Write-Host "❌ Error al verificar estado: $($_.Exception.Message)" -ForegroundColor Red
    exit
}

Write-Host "`n🚀 INTENTANDO INICIALIZACIÓN VIA API..." -ForegroundColor Green

# Intentar crear cada usuario via API
$usuariosCreados = 0

foreach ($usuario in $usuarios) {
    try {
        $body = @{
            nombre = $usuario.nombre
            apellido = $usuario.apellido
            email = $usuario.email
            password = $usuario.password
            fechaNacimiento = $usuario.fechaNacimiento + "T00:00:00"
            genero = $usuario.genero
            altura = $usuario.altura
            peso = $usuario.peso
            rol = $usuario.rol
            departamento = $usuario.departamento
            cargo = $usuario.cargo
            jefeId = $usuario.jefeId
        } | ConvertTo-Json
        
        Write-Host "Creando usuario: $($usuario.nombre) $($usuario.apellido)..." -NoNewline
        
        $resultado = Invoke-RestMethod -Uri "https://healthpredict-l1hu.onrender.com/api/Usuarios" -Method Post -Body $body -ContentType "application/json"
        
        Write-Host " ✅ OK" -ForegroundColor Green
        $usuariosCreados++
    }
    catch {
        Write-Host " ❌ ERROR" -ForegroundColor Red
        Write-Host "   Error: $($_.Exception.Message)" -ForegroundColor Yellow
    }
}

Write-Host "`n📊 RESULTADO FINAL:" -ForegroundColor Cyan
Write-Host "===================" -ForegroundColor Cyan
Write-Host "Usuarios creados exitosamente: $usuariosCreados de $($usuarios.Count)" -ForegroundColor White

if ($usuariosCreados -gt 0) {
    Write-Host "`n🎉 ¡ÉXITO! Usuarios creados." -ForegroundColor Green
    Write-Host "`n🔐 CREDENCIALES DE PRUEBA:" -ForegroundColor Blue
    Write-Host "=========================" -ForegroundColor Blue
    
    foreach ($usuario in $usuarios[0..$($usuariosCreados-1)]) {
        Write-Host "• $($usuario.nombre) $($usuario.apellido) ($($usuario.rol))" -ForegroundColor White
        Write-Host "  Email: $($usuario.email)" -ForegroundColor Gray
        Write-Host "  Password: $($usuario.password)" -ForegroundColor Gray
        Write-Host ""
    }
    
    Write-Host "🚀 ¡Ya puedes probar el login en tu aplicación!" -ForegroundColor Green
}
else {
    Write-Host "`n❌ No se pudo crear ningún usuario via API." -ForegroundColor Red
    Write-Host "`n🔧 SOLUCIONES RECOMENDADAS:" -ForegroundColor Yellow
    Write-Host "1. Verificar que las variables de entorno estén configuradas en Render" -ForegroundColor White
    Write-Host "2. Hacer Manual Deploy después de configurar variables" -ForegroundColor White
    Write-Host "3. Revisar los logs de Render para errores específicos" -ForegroundColor White
    Write-Host "4. Contactar al equipo de desarrollo con los detalles del error" -ForegroundColor White
}

Write-Host "`n✅ Script completado." -ForegroundColor Green 