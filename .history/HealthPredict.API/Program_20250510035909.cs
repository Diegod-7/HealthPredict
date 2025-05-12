using HealthPredict.DAL;
using HealthPredict.BLL;
using Microsoft.EntityFrameworkCore;
using Oracle.EntityFrameworkCore.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// Configuración de servicios
builder.Services.AddControllers();

// Configuración de la base de datos Oracle
builder.Services.AddDbContext<HealthPredictContext>(options => 
    options.UseOracle(builder.Configuration.GetConnectionString("OracleConnection"),
    oracleOptions => oracleOptions.UseOracleSQLCompatibility("11")));

// Configuración de CORS
builder.Services.AddCors(options => {
    options.AddPolicy("AllowAngularApp", policy => {
        policy.WithOrigins(builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>())
               .AllowAnyMethod()
               .AllowAnyHeader();
    });
});

// Registro de servicios
builder.Services.AddScoped<UsuarioService>();
builder.Services.AddScoped<DatoVitalService>();
builder.Services.AddScoped<AlertaService>();

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

app.Run();
