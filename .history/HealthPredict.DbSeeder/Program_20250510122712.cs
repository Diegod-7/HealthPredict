using HealthPredict.DAL;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;
using System.Threading.Tasks;

namespace HealthPredict.DbSeeder
{
    class Program
    {
        static async Task Main(string[] args)
        {
            try
            {
                // Cargar configuración desde appsettings.json
                var configuration = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.json")
                    .Build();

                // Obtener cadena de conexión
                var connectionString = configuration.GetConnectionString("OracleConnection");

                // Crear opciones para el contexto
                var optionsBuilder = new DbContextOptionsBuilder<HealthPredictContext>();
                optionsBuilder.UseOracle(connectionString, 
                    options => options.UseOracleSQLCompatibility("11"));

                // Crear instancia del contexto
                using (var context = new HealthPredictContext(optionsBuilder.Options))
                {
                    Console.WriteLine("Iniciando población de la base de datos con datos de prueba...");
                    
                    // Inicializar la base de datos
                    await DbInitializer.Initialize(context);
                    
                    Console.WriteLine("¡Datos de prueba cargados con éxito!");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al cargar datos de prueba: {ex.Message}");
                Console.WriteLine(ex.StackTrace);
            }

            Console.WriteLine("Presiona cualquier tecla para salir...");
            Console.ReadKey();
        }
    }
} 