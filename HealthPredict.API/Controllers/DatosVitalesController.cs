using HealthPredict.BLL;
using HealthPredict.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HealthPredict.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DatosVitalesController : ControllerBase
    {
        private readonly DatoVitalService _datoVitalService;
        private readonly AlertaService _alertaService;

        public DatosVitalesController(DatoVitalService datoVitalService, AlertaService alertaService)
        {
            _datoVitalService = datoVitalService;
            _alertaService = alertaService;
        }

        // GET: api/DatosVitales
        [HttpGet]
        public async Task<ActionResult<IEnumerable<DatoVital>>> GetAllDatosVitales()
        {
            var datosVitales = await _datoVitalService.GetAllDatosVitalesAsync();
            return Ok(datosVitales);
        }

        // GET: api/DatosVitales/Usuario/5
        [HttpGet("Usuario/{usuarioId}")]
        public async Task<ActionResult<IEnumerable<DatoVital>>> GetDatosVitalesByUsuario(int usuarioId)
        {
            var datosVitales = await _datoVitalService.GetDatosVitalesByUsuarioAsync(usuarioId);
            return Ok(datosVitales);
        }

        // GET: api/DatosVitales/5
        [HttpGet("{id}")]
        public async Task<ActionResult<DatoVital>> GetDatoVital(int id)
        {
            var datoVital = await _datoVitalService.GetDatoVitalByIdAsync(id);

            if (datoVital == null)
            {
                return NotFound();
            }

            return Ok(datoVital);
        }

        // POST: api/DatosVitales
        [HttpPost]
        public async Task<ActionResult<DatoVital>> CreateDatoVital(DatoVital datoVital)
        {
            var createdDatoVital = await _datoVitalService.CreateDatoVitalAsync(datoVital);
            
            // Verificar si el valor está fuera de rango
            bool fueraDeRango = await _datoVitalService.VerificarValorFueraDeRango(createdDatoVital);
            if (fueraDeRango)
            {
                // Generar alerta automática
                await _alertaService.GenerarAlertaPorDatoVitalAsync(
                    createdDatoVital, 
                    "valor_anormal", 
                    "media");
            }
            
            return CreatedAtAction(nameof(GetDatoVital), new { id = createdDatoVital.Id }, createdDatoVital);
        }

        // POST: api/DatosVitales/Sync/HealthKit
        [HttpPost("Sync/HealthKit")]
        public async Task<ActionResult> SyncHealthKitData([FromBody] List<HealthKitDataRequest> healthKitData)
        {
            try
            {
                // LOGGING DETALLADO - Para debugging Android
                Console.WriteLine($"🔍 [ANDROID DEBUG] Recibiendo datos de sincronización...");
                Console.WriteLine($"🔍 [ANDROID DEBUG] Cantidad de datos recibidos: {healthKitData?.Count ?? 0}");
                
                if (healthKitData == null || !healthKitData.Any())
                {
                    Console.WriteLine($"❌ [ANDROID DEBUG] No se recibieron datos para sincronizar");
                    return BadRequest(new { 
                        error = "No se recibieron datos para sincronizar", 
                        detalle = "La lista de datos está vacía o es null",
                        timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
                    });
                }

                var datosVitales = new List<DatoVital>();
                var erroresValidacion = new List<string>();
                
                for (int i = 0; i < healthKitData.Count; i++)
                {
                    var item = healthKitData[i];
                    Console.WriteLine($"🔍 [ANDROID DEBUG] Procesando dato {i + 1}: Usuario={item.UsuarioId}, Tipo={item.TipoHealthKit}, Valor={item.Valor}");
                    
                    // Validaciones detalladas
                    if (item.UsuarioId <= 0)
                    {
                        erroresValidacion.Add($"Dato {i + 1}: UsuarioId inválido ({item.UsuarioId})");
                        continue;
                    }
                    
                    if (string.IsNullOrEmpty(item.TipoHealthKit))
                    {
                        erroresValidacion.Add($"Dato {i + 1}: TipoHealthKit vacío");
                        continue;
                    }
                    
                    if (string.IsNullOrEmpty(item.Unidad))
                    {
                        erroresValidacion.Add($"Dato {i + 1}: Unidad vacía");
                        continue;
                    }

                    var datoVital = new DatoVital
                    {
                        UsuarioId = item.UsuarioId,
                        FechaRegistro = item.FechaRegistro,
                        TipoDato = MapHealthKitType(item.TipoHealthKit),
                        Valor = item.Valor,
                        Unidad = item.Unidad,
                        DispositivoOrigen = "Android Health",
                        Notas = $"Sincronizado desde Android - {item.TipoHealthKit}"
                    };
                    
                    datosVitales.Add(datoVital);
                }

                if (erroresValidacion.Any())
                {
                    Console.WriteLine($"❌ [ANDROID DEBUG] Errores de validación encontrados: {string.Join(", ", erroresValidacion)}");
                    return BadRequest(new { 
                        error = "Errores de validación en los datos", 
                        detalle = erroresValidacion,
                        timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
                    });
                }

                if (!datosVitales.Any())
                {
                    Console.WriteLine($"❌ [ANDROID DEBUG] No hay datos válidos para procesar");
                    return BadRequest(new { 
                        error = "No hay datos válidos para procesar", 
                        detalle = "Todos los datos fallaron la validación",
                        timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
                    });
                }

                Console.WriteLine($"✅ [ANDROID DEBUG] Guardando {datosVitales.Count} datos válidos en la base de datos...");
                var result = await _datoVitalService.CreateDatosVitalesEnLoteAsync(datosVitales);
                
                // Verificar alertas para datos sincronizados
                int alertasGeneradas = 0;
                foreach (var dato in result)
                {
                    bool fueraDeRango = await _datoVitalService.VerificarValorFueraDeRango(dato);
                    if (fueraDeRango)
                    {
                        await _alertaService.GenerarAlertaPorDatoVitalAsync(
                            dato, 
                            "valor_anormal_android", 
                            "media");
                        alertasGeneradas++;
                    }
                }

                Console.WriteLine($"✅ [ANDROID DEBUG] Sincronización completada: {result.Count} datos guardados, {alertasGeneradas} alertas generadas");

                return Ok(new { 
                    mensaje = "Datos sincronizados exitosamente desde Android", 
                    cantidadRecibida = healthKitData.Count,
                    cantidadProcesada = result.Count,
                    alertasGeneradas = alertasGeneradas,
                    timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ [ANDROID DEBUG] Error en sincronización: {ex.Message}");
                Console.WriteLine($"❌ [ANDROID DEBUG] Stack trace: {ex.StackTrace}");
                
                return BadRequest(new { 
                    error = "Error interno al sincronizar datos", 
                    detalle = ex.Message,
                    stackTrace = ex.StackTrace,
                    timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
                });
            }
        }

        // GET: api/DatosVitales/LastSync/{usuarioId}
        [HttpGet("LastSync/{usuarioId}")]
        public async Task<ActionResult<DateTime?>> GetLastSyncDate(int usuarioId)
        {
            var lastSync = await _datoVitalService.GetUltimaFechaSincronizacionAsync(usuarioId);
            return Ok(lastSync);
        }

        // PUT: api/DatosVitales/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateDatoVital(int id, DatoVital datoVital)
        {
            if (id != datoVital.Id)
            {
                return BadRequest();
            }

            await _datoVitalService.UpdateDatoVitalAsync(datoVital);
            return NoContent();
        }

        // DELETE: api/DatosVitales/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDatoVital(int id)
        {
            var result = await _datoVitalService.DeleteDatoVitalAsync(id);
            if (!result)
            {
                return NotFound();
            }

            return NoContent();
        }

        // GET: api/DatosVitales/Estadisticas
        [HttpGet("Estadisticas")]
        public async Task<ActionResult<Dictionary<string, decimal>>> GetEstadisticas(
            [FromQuery] int usuarioId, 
            [FromQuery] string tipoDato, 
            [FromQuery] DateTime fechaInicio, 
            [FromQuery] DateTime fechaFin)
        {
            var estadisticas = await _datoVitalService.GetEstadisticasAsync(usuarioId, tipoDato, fechaInicio, fechaFin);
            return Ok(estadisticas);
        }

        private string MapHealthKitType(string healthKitType)
        {
            return healthKitType switch
            {
                "HKQuantityTypeIdentifierHeartRate" => "frecuencia_cardiaca",
                "HKQuantityTypeIdentifierBloodPressureSystolic" => "presion_sistolica",
                "HKQuantityTypeIdentifierBloodPressureDiastolic" => "presion_diastolica",
                "HKQuantityTypeIdentifierBodyTemperature" => "temperatura_corporal",
                "HKQuantityTypeIdentifierOxygenSaturation" => "saturacion_oxigeno",
                "HKQuantityTypeIdentifierStepCount" => "pasos",
                "HKQuantityTypeIdentifierDistanceWalkingRunning" => "distancia_caminada",
                "HKQuantityTypeIdentifierActiveEnergyBurned" => "calorias_activas",
                "HKQuantityTypeIdentifierBodyMass" => "peso",
                "HKQuantityTypeIdentifierHeight" => "altura",
                "HKQuantityTypeIdentifierBodyMassIndex" => "indice_masa_corporal",
                "HKQuantityTypeIdentifierRespiratoryRate" => "frecuencia_respiratoria",
                _ => healthKitType.ToLower()
            };
        }
    }

    public class HealthKitDataRequest
    {
        public int UsuarioId { get; set; }
        public DateTime FechaRegistro { get; set; }
        public string TipoHealthKit { get; set; }
        public decimal Valor { get; set; }
        public string Unidad { get; set; }
    }
} 