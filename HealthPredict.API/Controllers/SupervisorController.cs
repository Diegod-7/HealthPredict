using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HealthPredict.DAL;
using HealthPredict.Models;

namespace HealthPredict.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SupervisorController : ControllerBase
    {
        private readonly HealthPredictContext _context;

        public SupervisorController(HealthPredictContext context)
        {
            _context = context;
        }

        // GET: api/Supervisor/dashboard/{jefeId}
        [HttpGet("dashboard/{jefeId}")]
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
                        usuarioId = a.UsuarioId,
                        usuario = a.Usuario.Nombre + " " + a.Usuario.Apellido,
                        tipoAlerta = a.TipoAlerta,
                        severidad = a.Severidad,
                        descripcion = a.Descripcion,
                        fechaCreacion = a.FechaCreacion,
                        leida = a.Leida
                    })
                    .ToListAsync();

                // Métricas por departamento
                var metricasDepartamento = await GetMetricasDepartamento(jefe.Departamento);

                // Tendencias de salud del equipo
                var tendenciasSalud = await GetTendenciasSaludEquipo(subordinadosIds);

                // Trabajadores en riesgo
                var trabajadoresEnRiesgo = await GetTrabajadoresEnRiesgo(subordinadosIds);

                // Resumen de bienestar por subordinado
                var resumenSubordinados = new List<object>();
                foreach (var subordinado in subordinados)
                {
                    var alertasSubordinado = await _context.Alertas
                        .Where(a => a.UsuarioId == subordinado.Id && !a.Resuelta)
                        .CountAsync();

                    var ultimosDatos = await _context.DatosVitales
                        .Where(d => d.UsuarioId == subordinado.Id)
                        .Where(d => d.FechaRegistro >= DateTime.UtcNow.AddDays(-7))
                        .GroupBy(d => d.TipoDato)
                        .Select(g => new
                        {
                            tipoDato = g.Key,
                            ultimoValor = g.OrderByDescending(d => d.FechaRegistro).First().Valor,
                            promedio = Math.Round(g.Average(d => d.Valor), 1),
                            unidad = g.First().Unidad
                        })
                        .ToListAsync();

                    // Calcular score de bienestar
                    var scoreBienestar = CalcularScoreBienestar(ultimosDatos.Cast<object>().ToList(), alertasSubordinado);

                    resumenSubordinados.Add(new
                    {
                        id = subordinado.Id,
                        nombre = subordinado.Nombre + " " + subordinado.Apellido,
                        cargo = subordinado.Cargo,
                        departamento = subordinado.Departamento,
                        email = subordinado.Email,
                        alertasActivas = alertasSubordinado,
                        ultimosDatos = ultimosDatos.Cast<object>().ToList(),
                        ultimoAcceso = subordinado.UltimoAcceso,
                        scoreBienestar,
                        nivelRiesgo = GetNivelRiesgo(scoreBienestar, alertasSubordinado)
                    });
                }

                var promedioScoreBienestar = resumenSubordinados.Any() ? 
                    resumenSubordinados.Average(s => (int)s.GetType().GetProperty("scoreBienestar").GetValue(s)) : 0;

                var resultado = new
                {
                    supervisor = new
                    {
                        id = jefe.Id,
                        nombre = jefe.Nombre + " " + jefe.Apellido,
                        departamento = jefe.Departamento,
                        cargo = jefe.Cargo
                    },
                    resumenGeneral = new
                    {
                        totalSubordinados = subordinados.Count,
                        alertasActivas = alertasActivas.Count,
                        alertasCriticas = alertasActivas.Count(a => a.severidad == "Crítica"),
                        alertasAltas = alertasActivas.Count(a => a.severidad == "Alta"),
                        trabajadoresEnRiesgo = trabajadoresEnRiesgo.Count,
                        promedioScoreBienestar = Math.Round(promedioScoreBienestar, 1)
                    },
                    alertasRecientes = alertasActivas,
                    resumenSubordinados,
                    metricasDepartamento,
                    tendenciasSalud,
                    trabajadoresEnRiesgo,
                    fechaActualizacion = DateTime.Now
                };

                return Ok(resultado);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "Error al obtener dashboard supervisor", detalle = ex.Message });
            }
        }

        // GET: api/Supervisor/metricas-departamento/{departamento}
        [HttpGet("metricas-departamento/{departamento}")]
        public async Task<ActionResult<object>> GetMetricasDepartamento(string departamento)
        {
            try
            {
                var usuariosDepartamento = await _context.Usuarios
                    .Where(u => u.Departamento == departamento && u.EsActivo)
                    .ToListAsync();

                var usuariosIds = usuariosDepartamento.Select(u => u.Id).ToList();

                // Alertas por severidad
                var alertasPorSeveridad = await _context.Alertas
                    .Where(a => usuariosIds.Contains(a.UsuarioId) && !a.Resuelta)
                    .GroupBy(a => a.Severidad)
                    .Select(g => new
                    {
                        severidad = g.Key,
                        cantidad = g.Count()
                    })
                    .ToListAsync();

                // Datos vitales promedio del departamento
                var hace30Dias = DateTime.UtcNow.AddDays(-30);
                var promediosDepartamento = await _context.DatosVitales
                    .Where(d => usuariosIds.Contains(d.UsuarioId) && d.FechaRegistro >= hace30Dias)
                    .GroupBy(d => d.TipoDato)
                    .Select(g => new
                    {
                        tipoDato = g.Key,
                        promedio = Math.Round(g.Average(d => d.Valor), 1),
                        cantidad = g.Count(),
                        unidad = g.First().Unidad
                    })
                    .ToListAsync();

                var resultado = new
                {
                    departamento,
                    totalUsuarios = usuariosDepartamento.Count,
                    alertasPorSeveridad,
                    promediosDepartamento,
                    fechaActualizacion = DateTime.Now
                };

                return Ok(resultado);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "Error al obtener métricas del departamento", detalle = ex.Message });
            }
        }

        // GET: api/Supervisor/trabajadores-riesgo/{jefeId}
        [HttpGet("trabajadores-riesgo/{jefeId}")]
        public async Task<ActionResult<object>> GetTrabajadoresEnRiesgo(int jefeId)
        {
            try
            {
                var subordinados = await _context.Usuarios
                    .Where(u => u.JefeId == jefeId && u.EsActivo)
                    .Select(u => u.Id)
                    .ToListAsync();

                var trabajadoresRiesgo = await GetTrabajadoresEnRiesgo(subordinados);

                return Ok(new
                {
                    trabajadoresEnRiesgo = trabajadoresRiesgo,
                    fechaActualizacion = DateTime.Now
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "Error al obtener trabajadores en riesgo", detalle = ex.Message });
            }
        }

        // GET: api/Supervisor/tendencias-salud/{jefeId}
        [HttpGet("tendencias-salud/{jefeId}")]
        public async Task<ActionResult<object>> GetTendenciasSalud(int jefeId)
        {
            try
            {
                var subordinados = await _context.Usuarios
                    .Where(u => u.JefeId == jefeId && u.EsActivo)
                    .Select(u => u.Id)
                    .ToListAsync();

                var tendencias = await GetTendenciasSaludEquipo(subordinados);

                return Ok(new
                {
                    tendenciasSalud = tendencias,
                    fechaActualizacion = DateTime.Now
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "Error al obtener tendencias de salud", detalle = ex.Message });
            }
        }

        // Métodos privados auxiliares
        private async Task<List<object>> GetTrabajadoresEnRiesgo(List<int> usuariosIds)
        {
            var trabajadoresRiesgo = new List<object>();

            // Usuarios con alertas críticas
            var usuariosConAlertasCriticas = await _context.Alertas
                .Where(a => usuariosIds.Contains(a.UsuarioId) && !a.Resuelta && a.Severidad == "Crítica")
                .Include(a => a.Usuario)
                .GroupBy(a => a.UsuarioId)
                .Select(g => new
                {
                    usuarioId = g.Key,
                    usuario = g.First().Usuario.Nombre + " " + g.First().Usuario.Apellido,
                    cargo = g.First().Usuario.Cargo,
                    alertasCriticas = g.Count(),
                    ultimaAlerta = g.OrderByDescending(a => a.FechaCreacion).First().Descripcion,
                    fechaUltimaAlerta = g.OrderByDescending(a => a.FechaCreacion).First().FechaCreacion,
                    nivelRiesgo = "Crítico"
                })
                .ToListAsync();

            trabajadoresRiesgo.AddRange(usuariosConAlertasCriticas);

            // Usuarios con múltiples alertas altas
            var usuariosConMultiplesAlertas = await _context.Alertas
                .Where(a => usuariosIds.Contains(a.UsuarioId) && !a.Resuelta && a.Severidad == "Alta")
                .Include(a => a.Usuario)
                .GroupBy(a => a.UsuarioId)
                .Where(g => g.Count() >= 2)
                .Select(g => new
                {
                    usuarioId = g.Key,
                    usuario = g.First().Usuario.Nombre + " " + g.First().Usuario.Apellido,
                    cargo = g.First().Usuario.Cargo,
                    alertasAltas = g.Count(),
                    ultimaAlerta = g.OrderByDescending(a => a.FechaCreacion).First().Descripcion,
                    fechaUltimaAlerta = g.OrderByDescending(a => a.FechaCreacion).First().FechaCreacion,
                    nivelRiesgo = "Alto"
                })
                .ToListAsync();

            // Evitar duplicados
            var usuariosYaIncluidos = usuariosConAlertasCriticas.Select(u => u.usuarioId).ToList();
            var nuevosUsuarios = usuariosConMultiplesAlertas.Where(u => !usuariosYaIncluidos.Contains(u.usuarioId));
            trabajadoresRiesgo.AddRange(nuevosUsuarios);

            return trabajadoresRiesgo.Cast<object>().ToList();
        }

        private async Task<object> GetTendenciasSaludEquipo(List<int> usuariosIds)
        {
            var hace30Dias = DateTime.UtcNow.AddDays(-30);
            var hace7Dias = DateTime.UtcNow.AddDays(-7);

            // Tendencia de alertas
            var alertasUltimos30Dias = await _context.Alertas
                .Where(a => usuariosIds.Contains(a.UsuarioId) && a.FechaCreacion >= hace30Dias)
                .GroupBy(a => a.FechaCreacion.Date)
                .Select(g => new
                {
                    fecha = g.Key,
                    cantidad = g.Count(),
                    criticas = g.Count(a => a.Severidad == "Crítica")
                })
                .OrderBy(x => x.fecha)
                .ToListAsync();

            return new
            {
                alertasDiarias = alertasUltimos30Dias,
                resumen = new
                {
                    alertasUltimos7Dias = alertasUltimos30Dias.Where(a => a.fecha >= hace7Dias).Sum(a => a.cantidad),
                    alertasCriticasUltimos7Dias = alertasUltimos30Dias.Where(a => a.fecha >= hace7Dias).Sum(a => a.criticas)
                }
            };
        }

        private int CalcularScoreBienestar(List<object> ultimosDatos, int alertasActivas)
        {
            int score = 100;

            // Penalizar por alertas
            score -= alertasActivas * 10;

            // Evaluar datos vitales específicos
            foreach (var dato in ultimosDatos)
            {
                var tipoDato = dato.GetType().GetProperty("tipoDato")?.GetValue(dato)?.ToString();
                var ultimoValor = Convert.ToDecimal(dato.GetType().GetProperty("ultimoValor")?.GetValue(dato) ?? 0);

                switch (tipoDato?.ToLower())
                {
                    case "nivel de estrés":
                        if (ultimoValor >= 8) score -= 15;
                        else if (ultimoValor >= 6) score -= 10;
                        else if (ultimoValor >= 4) score -= 5;
                        break;
                    case "horas de sueño":
                        if (ultimoValor < 5) score -= 15;
                        else if (ultimoValor < 6) score -= 10;
                        else if (ultimoValor < 7) score -= 5;
                        break;
                    case "presión arterial sistólica":
                        if (ultimoValor >= 140) score -= 15;
                        else if (ultimoValor >= 130) score -= 10;
                        break;
                }
            }

            return Math.Max(0, Math.Min(100, score));
        }

        private string GetNivelRiesgo(int scoreBienestar, int alertasActivas)
        {
            if (alertasActivas > 0 && scoreBienestar < 50) return "Crítico";
            if (alertasActivas > 1 || scoreBienestar < 70) return "Alto";
            if (scoreBienestar < 85) return "Moderado";
            return "Bajo";
        }
    }
} 