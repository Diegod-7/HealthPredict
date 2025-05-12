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

        public async Task<List<Usuario>> GetUsuariosAsync()
        {
            return await _context.Usuarios.ToListAsync();
        }

        public async Task<Usuario> GetUsuarioByIdAsync(int id)
        {
            return await _context.Usuarios.FindAsync(id);
        }

        public async Task<Usuario> GetUsuarioByEmailAsync(string email)
        {
            return await _context.Usuarios.FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task<Usuario> CreateUsuarioAsync(Usuario usuario)
        {
            usuario.FechaRegistro = DateTime.Now;
            usuario.UltimoAcceso = DateTime.Now;
            
            _context.Usuarios.Add(usuario);
            await _context.SaveChangesAsync();
            
            return usuario;
        }

        public async Task<bool> UpdateUsuarioAsync(Usuario usuario)
        {
            _context.Entry(usuario).State = EntityState.Modified;
            
            try
            {
                await _context.SaveChangesAsync();
                return true;
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await UsuarioExistsAsync(usuario.Id))
                {
                    return false;
                }
                else
                {
                    throw;
                }
            }
        }

        public async Task<bool> DeleteUsuarioAsync(int id)
        {
            var usuario = await _context.Usuarios.FindAsync(id);
            if (usuario == null)
                return false;

            _context.Usuarios.Remove(usuario);
            await _context.SaveChangesAsync();
            
            return true;
        }

        public async Task<Usuario> AuthenticateAsync(string email, string password)
        {
            var usuario = await _context.Usuarios
                .FirstOrDefaultAsync(u => u.Email == email && u.Password == password);

            if (usuario != null)
            {
                usuario.UltimoAcceso = DateTime.Now;
                await _context.SaveChangesAsync();
            }

            return usuario;
        }
        
        public async Task<bool> UsuarioExistsAsync(int id)
        {
            return await _context.Usuarios.AnyAsync(e => e.Id == id);
        }

        public async Task<bool> EmailExistsAsync(string email)
        {
            return await _context.Usuarios.AnyAsync(e => e.Email == email);
        }
    }
} 