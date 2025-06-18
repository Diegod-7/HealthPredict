using HealthPredict.DAL;
using HealthPredict.BLL;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Oracle.EntityFrameworkCore.Infrastructure;
using DinkToPdf;
using DinkToPdf.Contracts;
using HealthPredict.API.Services;
using HealthPredict.API;
using System.IO;
using System.Collections;

var builder = WebApplication.CreateBuilder(args);

// Configuración de servicios
builder.Services.AddControllers();

// Configuración de la base de datos PostgreSQL
var connectionString = Environment.GetEnvironmentVariable("DATABASE_URL") 
                      ?? builder.Configuration.GetConnectionString("PostgreSQLConnection");

Console.WriteLine($"🔍 PostgreSQL Connection String encontrado: {!string.IsNullOrEmpty(connectionString)}");
Console.WriteLine($"🔍 ASPNETCORE_ENVIRONMENT: {Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")}");

if (string.IsNullOrEmpty(connectionString))
{
    Console.WriteLine("❌ ERROR: No se encontró el string de conexión de PostgreSQL");
    Console.WriteLine("Variables de entorno disponibles:");
    foreach (DictionaryEntry env in Environment.GetEnvironmentVariables())
    {
        var key = env.Key.ToString();
        if (key.Contains("DATABASE") || key.Contains("CONNECTION") || key.Contains("POSTGRES"))
        {
            Console.WriteLine($"   {key}: {env.Value}");
        }
    }
    
    // Usar una conexión por defecto para evitar crash
    connectionString = "Host=localhost;Database=healthpredict;Username=postgres;Password=password";
    Console.WriteLine("⚠️ Usando conexión por defecto (la app funcionará parcialmente)");
}
else
{
    Console.WriteLine($"✅ PostgreSQL Connection String configurado correctamente");
    // Mostrar solo los primeros caracteres por seguridad
    Console.WriteLine($"🔗 Connection String: {connectionString.Substring(0, Math.Min(50, connectionString.Length))}...");
}

builder.Services.AddDbContext<HealthPredictContext>(options => 
{
    options.UseNpgsql(connectionString, npgsqlOptions => 
    {
        // Configurar timeout más largo para conexiones remotas
        npgsqlOptions.CommandTimeout(60);
    });
    
    // Configurar logging para PostgreSQL en desarrollo
    if (builder.Environment.IsDevelopment())
    {
        options.EnableSensitiveDataLogging();
        options.LogTo(Console.WriteLine, LogLevel.Information);
    }
});

// Configurar DinkToPdf
try 
{
    var customAssemblyLoadContext = new CustomAssemblyLoadContext();
    
    // Buscar la librería en varias ubicaciones posibles
    string[] possiblePaths = new[]
    {
        Path.Combine(Directory.GetCurrentDirectory(), "libwkhtmltox.dll"),
        Path.Combine(Directory.GetCurrentDirectory(), "lib", "libwkhtmltox.dll"),
        Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "libwkhtmltox.dll"),
        Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "lib", "libwkhtmltox.dll")
    };
    
    string? libraryPath = possiblePaths.FirstOrDefault(File.Exists);
    
    if (libraryPath == null)
    {
        // Si no se encuentra la librería, mostrar mensaje de error pero seguir ejecutando
        Console.WriteLine("======================================================================");
        Console.WriteLine("ADVERTENCIA: No se encontró la librería libwkhtmltox.dll");
        Console.WriteLine("La funcionalidad de generación de PDFs no estará disponible.");
        Console.WriteLine("Por favor, descargue la librería desde https://wkhtmltopdf.org/downloads.html");
        Console.WriteLine("y colóquela en el directorio raíz de la aplicación o en una carpeta 'lib'.");
        Console.WriteLine("======================================================================");
    }
    else 
    {
        Console.WriteLine($"Cargando librería nativa desde: {libraryPath}");
        customAssemblyLoadContext.LoadUnmanagedLibrary(libraryPath);
        builder.Services.AddSingleton(typeof(IConverter), new SynchronizedConverter(new PdfTools()));
    }
}
catch (Exception ex)
{
    Console.WriteLine($"Error al cargar la librería wkhtmltopdf: {ex.Message}");
    // Continuar sin la funcionalidad de PDF
}

// Registro de servicios
builder.Services.AddScoped<UsuarioService>();
builder.Services.AddScoped<DatoVitalService>();
builder.Services.AddScoped<AlertaService>();
builder.Services.AddScoped<ReporteService>();

// Configuración de CORS
builder.Services.AddCors(options => {
    options.AddPolicy("AllowAngularApp", policy => {
        policy.WithOrigins(builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>())
               .AllowAnyMethod()
               .AllowAnyHeader();
    });
});

// Configuración de Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configurar para escuchar en el puerto que Render asigne
var port = Environment.GetEnvironmentVariable("PORT") ?? "5000";
app.Urls.Add($"http://0.0.0.0:{port}");

// Configuración del pipeline de solicitudes HTTP
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
else
{
    // En producción también habilitamos Swagger para testing
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "HealthPredict API V1");
        c.RoutePrefix = "swagger";
    });
}

// No usar HTTPS redirect en Render (ellos manejan SSL)
if (!app.Environment.IsProduction())
{
    app.UseHttpsRedirection();
}

app.UseCors("AllowAngularApp");
app.UseAuthorization();
app.MapControllers();

// Inicializar la base de datos con datos de prueba
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var dbContext = services.GetRequiredService<HealthPredictContext>();
        DbInitializer.Initialize(dbContext).Wait();
        Console.WriteLine("Base de datos inicializada con datos de prueba.");
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "Ocurrió un error al inicializar la base de datos.");
    }
}

app.Run();
