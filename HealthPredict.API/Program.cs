using HealthPredict.DAL;
using HealthPredict.BLL;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using DinkToPdf;
using DinkToPdf.Contracts;
using HealthPredict.API.Services;
using HealthPredict.API;
using System.Reflection;

// ✅ CONFIGURACIÓN PARA POSTGRESQL Y ZONAS HORARIAS
AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

var builder = WebApplication.CreateBuilder(args);

// Configuración de servicios
builder.Services.AddControllers()
    .AddApplicationPart(Assembly.GetExecutingAssembly())
    .AddControllersAsServices();

// Configuración de la base de datos PostgreSQL
var connectionString = Environment.GetEnvironmentVariable("DATABASE_URL") 
                      ?? builder.Configuration.GetConnectionString("PostgreSQLConnection");

Console.WriteLine($"🔍 PostgreSQL Connection String encontrado: {!string.IsNullOrEmpty(connectionString)}");

if (string.IsNullOrEmpty(connectionString))
{
    Console.WriteLine("❌ ERROR: No se encontró el string de conexión de PostgreSQL");
    // Usar una conexión por defecto para evitar crash
    connectionString = "Host=localhost;Database=healthpredict;Username=postgres;Password=password";
    Console.WriteLine("⚠️ Usando conexión por defecto");
}

builder.Services.AddDbContext<HealthPredictContext>(options => 
{
    options.UseNpgsql(connectionString);
    
    // Logging solo en desarrollo
    if (builder.Environment.IsDevelopment())
    {
        options.EnableSensitiveDataLogging();
        options.LogTo(Console.WriteLine, LogLevel.Information);
    }
});

// Registro de servicios
builder.Services.AddScoped<UsuarioService>();
builder.Services.AddScoped<DatoVitalService>();
builder.Services.AddScoped<AlertaService>();
builder.Services.AddScoped<ReporteService>();

// Configuración de CORS
builder.Services.AddCors(options => {
    options.AddPolicy("AllowAngularApp", policy => {
        if (builder.Environment.IsDevelopment())
        {
            policy.AllowAnyOrigin()
                  .AllowAnyMethod()
                  .AllowAnyHeader();
        }
        else
        {
            policy.WithOrigins("http://localhost:4200", "https://healthpredict-l1hu.onrender.com")
                  .AllowAnyMethod()
                  .AllowAnyHeader()
                  .AllowCredentials();
        }
    });
});

// Configuración de Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "HealthPredict API", Version = "v1" });
});

var app = builder.Build();

Console.WriteLine("🔍 Controladores registrados:");
var controllerActionDescriptorProvider = app.Services.GetService<Microsoft.AspNetCore.Mvc.Infrastructure.IActionDescriptorCollectionProvider>();
if (controllerActionDescriptorProvider != null)
{
    var actions = controllerActionDescriptorProvider.ActionDescriptors.Items;
    foreach (var action in actions.Take(10)) // Mostrar solo los primeros 10
    {
        Console.WriteLine($"   - {action.DisplayName}");
    }
    Console.WriteLine($"   Total de acciones: {actions.Count}");
}

// Configuración del pipeline de solicitudes HTTP
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "HealthPredict API V1");
    });
}

app.UseCors("AllowAngularApp");
app.UseAuthorization();

// ✅ ENDPOINT RAÍZ PERSONALIZADO
app.MapGet("/", () => new { 
    message = "HealthPredict API", 
    version = "1.0.0",
    status = "Funcionando correctamente",
    endpoints = new {
        swagger = "/swagger",
        usuarios = "/api/Usuarios",
        datosVitales = "/api/DatosVitales",
        alertas = "/api/Alertas",
        graficos = "/api/Graficos",
        reportes = "/api/Reportes"
    }
});

app.MapControllers();

// Inicializar la base de datos
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var logger = services.GetRequiredService<ILogger<Program>>();
    
    try
    {
        Console.WriteLine("🔄 Inicializando base de datos...");
        var dbContext = services.GetRequiredService<HealthPredictContext>();
        
        // Probar conexión
        dbContext.Database.CanConnect();
        Console.WriteLine("✅ Conexión a BD exitosa");
        
        // Aplicar migraciones
        dbContext.Database.Migrate();
        Console.WriteLine("✅ Migraciones aplicadas");
        
        // Inicializar datos
        DbInitializer.InitializeAsync(dbContext).Wait();
        Console.WriteLine("✅ Datos inicializados");
        
            // Comentado temporalmente para debug
    // DataSeeder.SeedDataAsync(dbContext).Wait();
    Console.WriteLine("✅ DataSeeder deshabilitado temporalmente");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"❌ Error al inicializar BD: {ex.Message}");
        logger.LogError(ex, "Error al inicializar la base de datos");
    }
}

Console.WriteLine("🚀 API iniciada correctamente");
app.Run();
