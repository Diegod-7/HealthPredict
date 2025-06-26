using HealthPredict.DAL;
using HealthPredict.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HealthPredict.BLL
{
    public class UsuarioService
    {
        private readonly HealthPredictContext _context;

        public UsuarioService(HealthPredictContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Usuario>> GetUsuariosAsync()
        {
            return await _context.Usuarios
                .Include(u => u.Jefe)
                .Include(u => u.Subordinados)
                .ToListAsync();
        }

        public async Task<Usuario?> GetUsuarioByIdAsync(int id)
        {
            return await _context.Usuarios
                .Include(u => u.Jefe)
                .Include(u => u.Subordinados)
                .FirstOrDefaultAsync(u => u.Id == id);
        }

        public async Task<Usuario?> GetUsuarioByEmailAsync(string email)
        {
            return await _context.Usuarios
                .Include(u => u.Jefe)
                .Include(u => u.Subordinados)
                .FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task<Usuario> CreateUsuarioAsync(Usuario usuario)
        {
            // Solo asignar fechas si no están ya asignadas
            if (usuario.FechaRegistro == default(DateTime))
                usuario.FechaRegistro = DateTime.UtcNow;
            
            if (usuario.UltimoAcceso == default(DateTime))
                usuario.UltimoAcceso = DateTime.UtcNow;
            
            _context.Usuarios.Add(usuario);
            await _context.SaveChangesAsync();
            
            return usuario;
        }

        public async Task<bool> UpdateUsuarioAsync(Usuario usuario)
        {
            var existingUsuario = await _context.Usuarios.FindAsync(usuario.Id);
            if (existingUsuario == null) return false;

            _context.Entry(existingUsuario).CurrentValues.SetValues(usuario);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteUsuarioAsync(int id)
        {
            var usuario = await _context.Usuarios.FindAsync(id);
            if (usuario == null) return false;

            _context.Usuarios.Remove(usuario);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<Usuario?> AuthenticateAsync(string email, string password)
        {
            return await _context.Usuarios
                .Include(u => u.Jefe)
                .Include(u => u.Subordinados)
                .FirstOrDefaultAsync(u => u.Email == email && u.Password == password && u.EsActivo);
        }
        
        public async Task<bool> UsuarioExistsAsync(int id)
        {
            return await _context.Usuarios.AnyAsync(e => e.Id == id);
        }

        public async Task<bool> EmailExistsAsync(string email)
        {
            return await _context.Usuarios.AnyAsync(u => u.Email == email);
        }

        /// <summary>
        /// Obtiene todos los jefes del sistema
        /// </summary>
        public async Task<IEnumerable<Usuario>> GetJefesAsync()
        {
            return await _context.Usuarios
                .Include(u => u.Subordinados)
                .Where(u => u.Rol == "Jefe" && u.EsActivo)
                .ToListAsync();
        }

        /// <summary>
        /// Obtiene todos los trabajadores del sistema
        /// </summary>
        public async Task<IEnumerable<Usuario>> GetTrabajadoresAsync()
        {
            return await _context.Usuarios
                .Include(u => u.Jefe)
                .Where(u => u.Rol == "Trabajador" && u.EsActivo)
                .ToListAsync();
        }

        /// <summary>
        /// Obtiene los subordinados de un jefe específico
        /// </summary>
        public async Task<IEnumerable<Usuario>> GetSubordinadosByJefeAsync(int jefeId)
        {
            return await _context.Usuarios
                .Include(u => u.Jefe)
                .Where(u => u.JefeId == jefeId && u.EsActivo)
                .ToListAsync();
        }

        /// <summary>
        /// Verifica si un usuario es jefe
        /// </summary>
        public async Task<bool> EsJefeAsync(int usuarioId)
        {
            var usuario = await _context.Usuarios.FindAsync(usuarioId);
            return usuario?.Rol == "Jefe";
        }

        /// <summary>
        /// Verifica si un usuario puede acceder a los datos de otro usuario
        /// </summary>
        public async Task<bool> PuedeAccederADatosAsync(int usuarioSolicitante, int usuarioObjetivo)
        {
            // Un usuario siempre puede acceder a sus propios datos
            if (usuarioSolicitante == usuarioObjetivo)
                return true;

            // Verificar si el usuario solicitante es jefe
            var solicitante = await _context.Usuarios.FindAsync(usuarioSolicitante);
            if (solicitante?.Rol == "Jefe")
            {
                // Un jefe puede acceder a los datos de todos sus subordinados
                var esSubordinado = await _context.Usuarios
                    .AnyAsync(u => u.Id == usuarioObjetivo && u.JefeId == usuarioSolicitante);
                return esSubordinado;
            }

            return false; // Los trabajadores solo pueden ver sus propios datos
        }

        /// <summary>
        /// Obtiene estadísticas generales para el dashboard del jefe
        /// </summary>
        public async Task<object> GetEstadisticasGeneralesJefeAsync(int jefeId)
        {
            var subordinados = await GetSubordinadosByJefeAsync(jefeId);
            var subordinadoIds = subordinados.Select(s => s.Id).ToList();

            var totalAlertas = await _context.Alertas
                .Where(a => subordinadoIds.Contains(a.UsuarioId))
                .CountAsync();

            var alertasNoLeidas = await _context.Alertas
                .Where(a => subordinadoIds.Contains(a.UsuarioId) && !a.Leida)
                .CountAsync();

            var totalDatosVitales = await _context.DatosVitales
                .Where(d => subordinadoIds.Contains(d.UsuarioId))
                .CountAsync();

            return new
            {
                TotalSubordinados = subordinados.Count(),
                TotalAlertas = totalAlertas,
                AlertasNoLeidas = alertasNoLeidas,
                TotalDatosVitales = totalDatosVitales,
                Subordinados = subordinados.Select(s => new
                {
                    s.Id,
                    s.NombreCompleto,
                    s.Email,
                    s.Cargo,
                    s.Departamento,
                    UltimoAcceso = s.UltimoAcceso
                })
            };
        }

        /// <summary>
        /// Obtiene usuarios por departamento
        /// </summary>
        public async Task<IEnumerable<Usuario>> GetUsuariosByDepartamentoAsync(string departamento)
        {
            return await _context.Usuarios
                .Include(u => u.Jefe)
                .Where(u => u.Departamento == departamento && u.EsActivo)
                .ToListAsync();
        }
    }
} 