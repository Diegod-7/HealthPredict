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

var builder = WebApplication.CreateBuilder(args);

// Configuración de servicios
builder.Services.AddControllers();

// Configuración de la base de datos Oracle
builder.Services.AddDbContext<HealthPredictContext>(options => 
    options.UseOracle(builder.Configuration.GetConnectionString("OracleConnection"),
    oracleOptions => oracleOptions.UseOracleSQLCompatibility("11")));

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

// Configuración del pipeline de solicitudes HTTP
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
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
