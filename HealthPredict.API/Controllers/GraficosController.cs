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

            var fechaActual = DateTime.Now;
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
    }
} 