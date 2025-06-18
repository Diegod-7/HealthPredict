using HealthPredict.DAL;
using HealthPredict.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HealthPredict.BLL
{
    public class AlertaService
    {
        private readonly HealthPredictContext _context;

        public AlertaService(HealthPredictContext context)
        {
            _context = context;
        }

        public async Task<List<Alerta>> GetAllAlertasAsync()
        {
            return await _context.Alertas
                .Include(a => a.Usuario)
                .OrderByDescending(a => a.FechaCreacion)
                .ToListAsync();
        }

        public async Task<List<Alerta>> GetAlertasByUsuarioAsync(int usuarioId)
        {
            return await _context.Alertas
                .Where(a => a.UsuarioId == usuarioId)
                .OrderByDescending(a => a.FechaCreacion)
                .ToListAsync();
        }

        public async Task<List<Alerta>> GetAlertasNoLeidasByUsuarioAsync(int usuarioId)
        {
            return await _context.Alertas
                .Where(a => a.UsuarioId == usuarioId && !a.Leida)
                .OrderByDescending(a => a.FechaCreacion)
                .ToListAsync();
        }

        public async Task<List<Alerta>> GetAlertasBySeveridadAsync(int usuarioId, string severidad)
        {
            return await _context.Alertas
                .Where(a => a.UsuarioId == usuarioId && a.Severidad == severidad)
                .OrderByDescending(a => a.FechaCreacion)
                .ToListAsync();
        }

        public async Task<Alerta> GetAlertaByIdAsync(int id)
        {
            return await _context.Alertas
                .Include(a => a.Usuario)
                .FirstOrDefaultAsync(a => a.Id == id);
        }

        public async Task<Alerta> CreateAlertaAsync(Alerta alerta)
        {
            alerta.FechaCreacion = DateTime.Now;
            alerta.Leida = false;
            alerta.Resuelta = false;
            
            _context.Alertas.Add(alerta);
            await _context.SaveChangesAsync();
            
            return alerta;
        }

        public async Task<bool> UpdateAlertaAsync(Alerta alerta)
        {
            _context.Entry(alerta).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
                return true;
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await AlertaExistsAsync(alerta.Id))
                {
                    return false;
                }
                else
                {
                    throw;
                }
            }
        }

        public async Task<bool> DeleteAlertaAsync(int id)
        {
            var alerta = await _context.Alertas.FindAsync(id);
            if (alerta == null)
                return false;

            _context.Alertas.Remove(alerta);
            await _context.SaveChangesAsync();
            
            return true;
        }

        public async Task<Alerta> MarcarComoLeidaAsync(int id)
        {
            var alerta = await _context.Alertas.FindAsync(id);
            if (alerta == null)
                return null;

            alerta.Leida = true;
            alerta.FechaLectura = DateTime.Now;
            
            await _context.SaveChangesAsync();
            
            return alerta;
        }

        public async Task<Alerta> ResolverAlertaAsync(int id, string notasResolucion)
        {
            var alerta = await _context.Alertas.FindAsync(id);
            if (alerta == null)
                return null;

            alerta.Resuelta = true;
            alerta.FechaResolucion = DateTime.Now;
            alerta.NotasResolucion = notasResolucion;
            
            await _context.SaveChangesAsync();
            
            return alerta;
        }

        public async Task<Alerta> GenerarAlertaPorDatoVitalAsync(DatoVital datoVital, string tipoAlerta, string severidad)
        {
            var alerta = new Alerta
            {
                UsuarioId = datoVital.UsuarioId,
                TipoAlerta = tipoAlerta,
                Descripcion = $"Valor anormal de {datoVital.TipoDato}: {datoVital.Valor} {datoVital.Unidad}",
                Severidad = severidad
            };

            return await CreateAlertaAsync(alerta);
        }
        
        public async Task<bool> AlertaExistsAsync(int id)
        {
            return await _context.Alertas.AnyAsync(e => e.Id == id);
        }
    }
} 