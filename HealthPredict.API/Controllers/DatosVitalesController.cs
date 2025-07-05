using HealthPredict.BLL;
using HealthPredict.Models;
using HealthPredict.DAL;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace HealthPredict.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DatosVitalesController : ControllerBase
    {
        private readonly DatoVitalService _datoVitalService;
        private readonly AlertaService _alertaService;
        private readonly ILogger<DatosVitalesController> _logger;
        private readonly HealthPredictContext _context;

        public DatosVitalesController(DatoVitalService datoVitalService, AlertaService alertaService, ILogger<DatosVitalesController> logger, HealthPredictContext context)
        {
            _datoVitalService = datoVitalService;
            _alertaService = alertaService;
            _logger = logger;
            _context = context;
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

        /// <summary>
        /// Obtener resumen de pasos del día actual
        /// </summary>
        [HttpGet("pasos-hoy/{usuarioId}")]
        public async Task<ActionResult<object>> GetPasosHoy(int usuarioId)
        {
            try
            {
                var hoy = DateTime.Today;
                var mañana = hoy.AddDays(1);

                // Obtener todos los registros de pasos del día actual
                var pasosHoy = await _context.DatosVitales
                    .Where(d => d.UsuarioId == usuarioId && 
                               d.TipoDato == "Pasos" && 
                               d.FechaMedicion >= hoy && 
                               d.FechaMedicion < mañana)
                    .OrderBy(d => d.FechaMedicion)
                    .ToListAsync();

                if (!pasosHoy.Any())
                {
                    return Ok(new
                    {
                        fecha = hoy.ToString("yyyy-MM-dd"),
                        totalPasos = 0,
                        registros = 0,
                        ultimaActualizacion = (DateTime?)null,
                        datosGrafico = new List<object>()
                    });
                }

                // Agrupar por hora y minuto exactos para evitar duplicados
                var pasosPorMinuto = pasosHoy
                    .GroupBy(d => new { 
                        Fecha = d.FechaMedicion.Date,
                        Hora = d.FechaMedicion.Hour,
                        Minuto = d.FechaMedicion.Minute
                    })
                    .Select(g => new
                    {
                        FechaHora = new DateTime(g.Key.Fecha.Year, g.Key.Fecha.Month, g.Key.Fecha.Day, g.Key.Hora, g.Key.Minuto, 0),
                        Pasos = g.Sum(d => (int)Math.Round(d.Valor)),
                        Registros = g.Count()
                    })
                    .OrderBy(p => p.FechaHora)
                    .ToList();

                // Calcular total de pasos
                var totalPasos = pasosPorMinuto.Sum(p => p.Pasos);

                // Crear datos para el gráfico (agrupados por hora para mejor visualización)
                var pasosPorHora = pasosPorMinuto
                    .GroupBy(p => p.FechaHora.Hour)
                    .Select(g => new
                    {
                        hora = g.Key,
                        horaTexto = $"{g.Key:00}:00",
                        pasos = g.Sum(p => p.Pasos),
                        registros = g.Sum(p => p.Registros)
                    })
                    .OrderBy(p => p.hora)
                    .ToList();

                var ultimaActualizacion = pasosHoy.Max(d => d.FechaRegistro);

                return Ok(new
                {
                    fecha = hoy.ToString("yyyy-MM-dd"),
                    totalPasos = totalPasos,
                    registros = pasosHoy.Count,
                    ultimaActualizacion = ultimaActualizacion,
                    datosGrafico = pasosPorHora
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error obteniendo pasos del día actual para usuario {UsuarioId}", usuarioId);
                return StatusCode(500, new { error = "Error interno del servidor" });
            }
        }

        /// <summary>
        /// Obtener resumen de pasos de los últimos 7 días
        /// </summary>
        [HttpGet("pasos-semana/{usuarioId}")]
        public async Task<ActionResult<object>> GetPasosSemana(int usuarioId)
        {
            try
            {
                var hoy = DateTime.Today;
                var haceUnaSemana = hoy.AddDays(-6); // Últimos 7 días incluyendo hoy

                // Obtener todos los registros de pasos de la semana
                var pasosSemana = await _context.DatosVitales
                    .Where(d => d.UsuarioId == usuarioId && 
                               d.TipoDato == "Pasos" && 
                               d.FechaMedicion >= haceUnaSemana && 
                               d.FechaMedicion < hoy.AddDays(1))
                    .ToListAsync();

                // Agrupar por día para evitar duplicados
                var pasosPorDia = pasosSemana
                    .GroupBy(d => d.FechaMedicion.Date)
                    .Select(g => new
                    {
                        fecha = g.Key,
                        fechaTexto = g.Key.ToString("dd/MM"),
                        diaSemana = g.Key.ToString("dddd", new System.Globalization.CultureInfo("es-ES")),
                        pasos = g.GroupBy(x => new { x.FechaMedicion.Hour, x.FechaMedicion.Minute })
                                 .Sum(x => (int)Math.Round(x.Sum(y => y.Valor))),
                        registros = g.Count()
                    })
                    .OrderBy(p => p.fecha)
                    .ToList();

                // Llenar días faltantes con 0 pasos
                var datosSemana = new List<object>();
                for (int i = 0; i < 7; i++)
                {
                    var fecha = haceUnaSemana.AddDays(i);
                    var datoDia = pasosPorDia.FirstOrDefault(p => p.fecha == fecha);
                    
                    datosSemana.Add(new
                    {
                        fecha = fecha.ToString("yyyy-MM-dd"),
                        fechaTexto = fecha.ToString("dd/MM"),
                        diaSemana = fecha.ToString("dddd", new System.Globalization.CultureInfo("es-ES")),
                        pasos = datoDia?.pasos ?? 0,
                        registros = datoDia?.registros ?? 0,
                        esHoy = fecha.Date == hoy.Date
                    });
                }

                var totalPasos = pasosPorDia.Sum(p => p.pasos);
                var promedioDiario = pasosPorDia.Any() ? totalPasos / 7 : 0;

                return Ok(new
                {
                    fechaInicio = haceUnaSemana.ToString("yyyy-MM-dd"),
                    fechaFin = hoy.ToString("yyyy-MM-dd"),
                    totalPasos = totalPasos,
                    promedioDiario = promedioDiario,
                    diasConDatos = pasosPorDia.Count,
                    datosGrafico = datosSemana
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error obteniendo pasos de la semana para usuario {UsuarioId}", usuarioId);
                return StatusCode(500, new { error = "Error interno del servidor" });
            }
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