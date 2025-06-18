using HealthPredict.DAL;
using HealthPredict.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HealthPredict.BLL
{
    public class DatoVitalService
    {
        private readonly HealthPredictContext _context;

        public DatoVitalService(HealthPredictContext context)
        {
            _context = context;
        }

        public async Task<List<DatoVital>> GetAllDatosVitalesAsync()
        {
            return await _context.DatosVitales
                .Include(d => d.Usuario)
                .OrderByDescending(d => d.FechaRegistro)
                .ToListAsync();
        }

        public async Task<List<DatoVital>> GetDatosVitalesByUsuarioAsync(int usuarioId)
        {
            return await _context.DatosVitales
                .Where(d => d.UsuarioId == usuarioId)
                .OrderByDescending(d => d.FechaRegistro)
                .ToListAsync();
        }

        public async Task<List<DatoVital>> GetDatosVitalesByTipoAsync(int usuarioId, string tipoDato)
        {
            return await _context.DatosVitales
                .Where(d => d.UsuarioId == usuarioId && d.TipoDato == tipoDato)
                .OrderByDescending(d => d.FechaRegistro)
                .ToListAsync();
        }

        public async Task<List<DatoVital>> GetDatosVitalesByFechaAsync(int usuarioId, DateTime fechaInicio, DateTime fechaFin)
        {
            return await _context.DatosVitales
                .Where(d => d.UsuarioId == usuarioId && d.FechaRegistro >= fechaInicio && d.FechaRegistro <= fechaFin)
                .OrderByDescending(d => d.FechaRegistro)
                .ToListAsync();
        }

        public async Task<DatoVital> GetDatoVitalByIdAsync(int id)
        {
            return await _context.DatosVitales
                .Include(d => d.Usuario)
                .FirstOrDefaultAsync(d => d.Id == id);
        }

        public async Task<DatoVital> CreateDatoVitalAsync(DatoVital datoVital)
        {
            datoVital.FechaRegistro = DateTime.Now;
            
            _context.DatosVitales.Add(datoVital);
            await _context.SaveChangesAsync();
            
            return datoVital;
        }

        public async Task<List<DatoVital>> CreateDatosVitalesEnLoteAsync(List<DatoVital> datosVitales)
        {
            // Verificar duplicados basados en usuario, tipo, fecha y valor
            var datosParaInsertar = new List<DatoVital>();
            
            foreach (var dato in datosVitales)
            {
                // Verificar si ya existe un dato similar en la base de datos
                var existe = await _context.DatosVitales
                    .AnyAsync(d => d.UsuarioId == dato.UsuarioId &&
                                  d.TipoDato == dato.TipoDato &&
                                  d.FechaRegistro.Date == dato.FechaRegistro.Date &&
                                  d.FechaRegistro.Hour == dato.FechaRegistro.Hour &&
                                  d.FechaRegistro.Minute == dato.FechaRegistro.Minute &&
                                  Math.Abs(d.Valor - dato.Valor) < 0.01M);
                
                if (!existe)
                {
                    datosParaInsertar.Add(dato);
                }
            }

            if (datosParaInsertar.Any())
            {
                _context.DatosVitales.AddRange(datosParaInsertar);
                await _context.SaveChangesAsync();
            }
            
            return datosParaInsertar;
        }

        public async Task<DateTime?> GetUltimaFechaSincronizacionAsync(int usuarioId)
        {
            var ultimoRegistro = await _context.DatosVitales
                .Where(d => d.UsuarioId == usuarioId && d.DispositivoOrigen == "Apple Health")
                .OrderByDescending(d => d.FechaRegistro)
                .FirstOrDefaultAsync();
                
            return ultimoRegistro?.FechaRegistro;
        }

        public async Task<List<DatoVital>> GetDatosVitalesParaSincronizarAsync(int usuarioId, DateTime? ultimaFecha = null)
        {
            var query = _context.DatosVitales
                .Where(d => d.UsuarioId == usuarioId);
                
            if (ultimaFecha.HasValue)
            {
                query = query.Where(d => d.FechaRegistro > ultimaFecha.Value);
            }
            
            return await query
                .OrderBy(d => d.FechaRegistro)
                .ToListAsync();
        }

        public async Task<bool> UpdateDatoVitalAsync(DatoVital datoVital)
        {
            _context.Entry(datoVital).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
                return true;
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await DatoVitalExistsAsync(datoVital.Id))
                {
                    return false;
                }
                else
                {
                    throw;
                }
            }
        }

        public async Task<bool> DeleteDatoVitalAsync(int id)
        {
            var datoVital = await _context.DatosVitales.FindAsync(id);
            if (datoVital == null)
                return false;

            _context.DatosVitales.Remove(datoVital);
            await _context.SaveChangesAsync();
            
            return true;
        }

        public async Task<Dictionary<string, decimal>> GetEstadisticasAsync(int usuarioId, string tipoDato, DateTime fechaInicio, DateTime fechaFin)
        {
            var datos = await _context.DatosVitales
                .Where(d => d.UsuarioId == usuarioId && 
                       d.TipoDato == tipoDato && 
                       d.FechaRegistro >= fechaInicio && 
                       d.FechaRegistro <= fechaFin)
                .Select(d => d.Valor)
                .ToListAsync();

            if (!datos.Any())
                return new Dictionary<string, decimal>();

            return new Dictionary<string, decimal>
            {
                { "promedio", datos.Average() },
                { "maximo", datos.Max() },
                { "minimo", datos.Min() }
            };
        }

        public async Task<bool> VerificarValorFueraDeRango(DatoVital datoVital)
        {
            // Definición básica de rangos normales para algunos tipos de datos
            Dictionary<string, (decimal min, decimal max)> rangosNormales = new Dictionary<string, (decimal min, decimal max)>
            {
                { "frecuencia_cardiaca", (60, 100) },
                { "presion_sistolica", (90, 120) },
                { "presion_diastolica", (60, 80) },
                { "temperatura", (36.1M, 37.2M) },
                { "glucosa", (70, 140) }
            };

            // Verificar si el tipo de dato tiene rangos definidos
            if (rangosNormales.ContainsKey(datoVital.TipoDato))
            {
                var (min, max) = rangosNormales[datoVital.TipoDato];
                return datoVital.Valor < min || datoVital.Valor > max;
            }

            return false;
        }
        
        public async Task<bool> DatoVitalExistsAsync(int id)
        {
            return await _context.DatosVitales.AnyAsync(e => e.Id == id);
        }
        
        public async Task<decimal> GetPromedioByTipoAsync(int usuarioId, string tipoDato, DateTime fechaInicio, DateTime fechaFin)
        {
            var datos = await _context.DatosVitales
                .Where(d => d.UsuarioId == usuarioId && d.TipoDato == tipoDato && d.FechaRegistro >= fechaInicio && d.FechaRegistro <= fechaFin)
                .ToListAsync();

            if (datos == null || !datos.Any())
                return 0;

            return datos.Average(d => d.Valor);
        }

        public async Task<decimal> GetMaximoByTipoAsync(int usuarioId, string tipoDato, DateTime fechaInicio, DateTime fechaFin)
        {
            var datos = await _context.DatosVitales
                .Where(d => d.UsuarioId == usuarioId && d.TipoDato == tipoDato && d.FechaRegistro >= fechaInicio && d.FechaRegistro <= fechaFin)
                .ToListAsync();

            if (datos == null || !datos.Any())
                return 0;

            return datos.Max(d => d.Valor);
        }

        public async Task<decimal> GetMinimoByTipoAsync(int usuarioId, string tipoDato, DateTime fechaInicio, DateTime fechaFin)
        {
            var datos = await _context.DatosVitales
                .Where(d => d.UsuarioId == usuarioId && d.TipoDato == tipoDato && d.FechaRegistro >= fechaInicio && d.FechaRegistro <= fechaFin)
                .ToListAsync();

            if (datos == null || !datos.Any())
                return 0;

            return datos.Min(d => d.Valor);
        }
    }
} 