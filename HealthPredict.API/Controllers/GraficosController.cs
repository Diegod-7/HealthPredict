using Microsoft.AspNetCore.Mvc;
using HealthPredict.DAL;
using HealthPredict.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HealthPredict.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GraficosController : ControllerBase
    {
        private readonly HealthPredictContext _context;

        public GraficosController(HealthPredictContext context)
        {
            _context = context;
        }

        // GET: api/Graficos/DatosVitales/{usuarioId}/{tipoDato}
        [HttpGet("DatosVitales/{usuarioId}/{tipoDato}")]
        public async Task<ActionResult<IEnumerable<object>>> GetDatosVitalesPorTipo(int usuarioId, string tipoDato)
        {
            var usuario = await _context.Usuarios.FindAsync(usuarioId);
            if (usuario == null)
            {
                return NotFound("Usuario no encontrado");
            }

            var datos = await _context.DatosVitales
                .Where(d => d.UsuarioId == usuarioId && d.TipoDato == tipoDato)
                .OrderBy(d => d.FechaRegistro)
                .Select(d => new
                {
                    fecha = d.FechaRegistro,
                    valor = d.Valor,
                    unidad = d.Unidad
                })
                .ToListAsync();

            if (!datos.Any())
            {
                return NotFound($"No se encontraron datos del tipo {tipoDato} para el usuario");
            }

            return Ok(datos);
        }

        // GET: api/Graficos/TiposDeDatos/{usuarioId}
        [HttpGet("TiposDeDatos/{usuarioId}")]
        public async Task<ActionResult<IEnumerable<string>>> GetTiposDeDatos(int usuarioId)
        {
            var usuario = await _context.Usuarios.FindAsync(usuarioId);
            if (usuario == null)
            {
                return NotFound("Usuario no encontrado");
            }

            var tiposDeDatos = await _context.DatosVitales
                .Where(d => d.UsuarioId == usuarioId)
                .Select(d => d.TipoDato)
                .Distinct()
                .ToListAsync();

            if (!tiposDeDatos.Any())
            {
                return NotFound("No se encontraron datos vitales para el usuario");
            }

            return Ok(tiposDeDatos);
        }

        // GET: api/Graficos/ResumenDatosVitales/{usuarioId}
        [HttpGet("ResumenDatosVitales/{usuarioId}")]
        public async Task<ActionResult<object>> GetResumenDatosVitales(int usuarioId)
        {
            var usuario = await _context.Usuarios.FindAsync(usuarioId);
            if (usuario == null)
            {
                return NotFound("Usuario no encontrado");
            }

            var tiposDeDatos = await _context.DatosVitales
                .Where(d => d.UsuarioId == usuarioId)
                .Select(d => d.TipoDato)
                .Distinct()
                .ToListAsync();

            var resultado = new Dictionary<string, object>();

            foreach (var tipo in tiposDeDatos)
            {
                var ultimoDato = await _context.DatosVitales
                    .Where(d => d.UsuarioId == usuarioId && d.TipoDato == tipo)
                    .OrderByDescending(d => d.FechaRegistro)
                    .FirstOrDefaultAsync();

                var promedio = await _context.DatosVitales
                    .Where(d => d.UsuarioId == usuarioId && d.TipoDato == tipo)
                    .AverageAsync(d => d.Valor);

                var minimo = await _context.DatosVitales
                    .Where(d => d.UsuarioId == usuarioId && d.TipoDato == tipo)
                    .MinAsync(d => d.Valor);

                var maximo = await _context.DatosVitales
                    .Where(d => d.UsuarioId == usuarioId && d.TipoDato == tipo)
                    .MaxAsync(d => d.Valor);

                resultado[tipo] = new
                {
                    ultimoValor = ultimoDato?.Valor,
                    unidad = ultimoDato?.Unidad,
                    fecha = ultimoDato?.FechaRegistro,
                    promedio = promedio,
                    minimo = minimo,
                    maximo = maximo
                };
            }

            return Ok(resultado);
        }

        // GET: api/Graficos/ComparativaMensual/{usuarioId}/{tipoDato}
        [HttpGet("ComparativaMensual/{usuarioId}/{tipoDato}")]
        public async Task<ActionResult<object>> GetComparativaMensual(int usuarioId, string tipoDato)
        {
            var usuario = await _context.Usuarios.FindAsync(usuarioId);
            if (usuario == null)
            {
                return NotFound("Usuario no encontrado");
            }

                            var fechaActual = DateTime.UtcNow;
            var inicioMesActual = new DateTime(fechaActual.Year, fechaActual.Month, 1);
            var inicioMesAnterior = inicioMesActual.AddMonths(-1);

            var datosMesActual = await _context.DatosVitales
                .Where(d => d.UsuarioId == usuarioId && 
                       d.TipoDato == tipoDato && 
                       d.FechaRegistro >= inicioMesActual)
                .ToListAsync();

            var datosMesAnterior = await _context.DatosVitales
                .Where(d => d.UsuarioId == usuarioId && 
                       d.TipoDato == tipoDato && 
                       d.FechaRegistro >= inicioMesAnterior && 
                       d.FechaRegistro < inicioMesActual)
                .ToListAsync();

            var promedioMesActual = datosMesActual.Any() ? datosMesActual.Average(d => d.Valor) : 0;
            var promedioMesAnterior = datosMesAnterior.Any() ? datosMesAnterior.Average(d => d.Valor) : 0;

            var resultado = new
            {
                mesActual = new
                {
                    mes = fechaActual.ToString("MMMM yyyy"),
                    promedio = promedioMesActual,
                    cantidadMediciones = datosMesActual.Count
                },
                mesAnterior = new
                {
                    mes = inicioMesAnterior.ToString("MMMM yyyy"),
                    promedio = promedioMesAnterior,
                    cantidadMediciones = datosMesAnterior.Count
                },
                variacion = datosMesAnterior.Any() 
                    ? ((promedioMesActual - promedioMesAnterior) / promedioMesAnterior) * 100 
                    : 0,
                unidad = datosMesActual.FirstOrDefault()?.Unidad ?? ""
            };

            return Ok(resultado);
        }

        // GET: api/Graficos/estadisticas-generales
        [HttpGet("estadisticas-generales")]
        public async Task<ActionResult<object>> GetEstadisticasGenerales()
        {
            try
            {
                var totalUsuarios = await _context.Usuarios.CountAsync();
                var totalDatosVitales = await _context.DatosVitales.CountAsync();
                var totalAlertas = await _context.Alertas.CountAsync();
                var alertasActivas = await _context.Alertas.CountAsync(a => !a.Resuelta);
                
                // Estadísticas por departamento
                var estatsPorDepartamento = await _context.Usuarios
                    .GroupBy(u => u.Departamento)
                    .Select(g => new
                    {
                        departamento = g.Key,
                        totalUsuarios = g.Count(),
                        usuariosActivos = g.Count(u => u.EsActivo)
                    })
                    .ToListAsync();

                // Tipos de datos más registrados
                var tiposDatosMasRegistrados = await _context.DatosVitales
                    .GroupBy(d => d.TipoDato)
                    .Select(g => new
                    {
                        tipoDato = g.Key,
                        cantidad = g.Count(),
                        ultimaActualizacion = g.Max(d => d.FechaRegistro)
                    })
                    .OrderByDescending(x => x.cantidad)
                    .Take(10)
                    .ToListAsync();

                // Alertas por severidad
                var alertasPorSeveridad = await _context.Alertas
                    .GroupBy(a => a.Severidad)
                    .Select(g => new
                    {
                        severidad = g.Key,
                        cantidad = g.Count(),
                        resueltas = g.Count(a => a.Resuelta)
                    })
                    .ToListAsync();

                // Tendencia de datos vitales en los últimos 30 días
                var hace30Dias = DateTime.UtcNow.AddDays(-30);
                var tendenciaUltimos30Dias = await _context.DatosVitales
                    .Where(d => d.FechaRegistro >= hace30Dias)
                    .GroupBy(d => d.FechaRegistro.Date)
                    .Select(g => new
                    {
                        fecha = g.Key,
                        cantidad = g.Count()
                    })
                    .OrderBy(x => x.fecha)
                    .ToListAsync();

                var resultado = new
                {
                    resumenGeneral = new
                    {
                        totalUsuarios,
                        totalDatosVitales,
                        totalAlertas,
                        alertasActivas,
                        porcentajeAlertasResueltas = totalAlertas > 0 ? 
                            Math.Round(((double)(totalAlertas - alertasActivas) / totalAlertas) * 100, 1) : 0
                    },
                    estatsPorDepartamento,
                    tiposDatosMasRegistrados,
                    alertasPorSeveridad,
                    tendenciaUltimos30Dias,
                    fechaActualizacion = DateTime.Now
                };

                return Ok(resultado);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "Error al obtener estadísticas generales", detalle = ex.Message });
            }
        }

        // GET: api/Graficos/dashboard-supervisor/{jefeId}
        [HttpGet("dashboard-supervisor/{jefeId}")]
        public async Task<ActionResult<object>> GetDashboardSupervisor(int jefeId)
        {
            try
            {
                var jefe = await _context.Usuarios.FindAsync(jefeId);
                if (jefe == null || jefe.Rol != "Jefe")
                {
                    return NotFound("Supervisor no encontrado");
                }

                // Obtener subordinados
                var subordinados = await _context.Usuarios
                    .Where(u => u.JefeId == jefeId && u.EsActivo)
                    .ToListAsync();

                var subordinadosIds = subordinados.Select(s => s.Id).ToList();

                // Alertas activas de subordinados
                var alertasActivas = await _context.Alertas
                    .Where(a => subordinadosIds.Contains(a.UsuarioId) && !a.Resuelta)
                    .Include(a => a.Usuario)
                    .OrderByDescending(a => a.FechaCreacion)
                    .Take(10)
                    .Select(a => new
                    {
                        id = a.Id,
                        usuario = a.Usuario.Nombre + " " + a.Usuario.Apellido,
                        tipoAlerta = a.TipoAlerta,
                        severidad = a.Severidad,
                        descripcion = a.Descripcion,
                        fechaCreacion = a.FechaCreacion,
                        leida = a.Leida
                    })
                    .ToListAsync();

                // Resumen de salud por subordinado
                var resumenSubordinados = new List<object>();
                foreach (var subordinado in subordinados)
                {
                    var ultimosDatos = await _context.DatosVitales
                        .Where(d => d.UsuarioId == subordinado.Id)
                        .Where(d => d.FechaRegistro >= DateTime.UtcNow.AddDays(-7))
                        .GroupBy(d => d.TipoDato)
                        .Select(g => new
                        {
                            tipoDato = g.Key,
                            ultimoValor = g.OrderByDescending(d => d.FechaRegistro).First().Valor,
                            promedio = g.Average(d => d.Valor)
                        })
                        .ToListAsync();

                    var alertasRecientes = await _context.Alertas
                        .Where(a => a.UsuarioId == subordinado.Id && !a.Resuelta)
                        .CountAsync();

                    resumenSubordinados.Add(new
                    {
                        id = subordinado.Id,
                        nombre = subordinado.Nombre + " " + subordinado.Apellido,
                        cargo = subordinado.Cargo,
                        departamento = subordinado.Departamento,
                        alertasActivas = alertasRecientes,
                        ultimosDatos,
                        ultimoAcceso = subordinado.UltimoAcceso
                    });
                }

                var resultado = new
                {
                    supervisor = new
                    {
                        id = jefe.Id,
                        nombre = jefe.Nombre + " " + jefe.Apellido,
                        departamento = jefe.Departamento
                    },
                    totalSubordinados = subordinados.Count,
                    alertasActivas = alertasActivas.Count,
                    alertasCriticas = alertasActivas.Count(a => a.severidad == "Crítica"),
                    alertasRecientes = alertasActivas,
                    resumenSubordinados,
                    fechaActualizacion = DateTime.Now
                };

                return Ok(resultado);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "Error al obtener dashboard supervisor", detalle = ex.Message });
            }
        }
    }
} 