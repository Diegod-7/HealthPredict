using System;
using System.Threading.Tasks;
using HealthPredict.DAL;
using Microsoft.EntityFrameworkCore;

namespace HealthPredict.API.Services
{
    public class ReporteService
    {
        private readonly HealthPredictContext _context;

        public ReporteService(HealthPredictContext context, IServiceProvider serviceProvider)
        {
            _context = context;
        }

        public async Task<byte[]> GenerarReporteDatosVitalesAsync(int usuarioId, string tipoDato = null)
        {
            // Implementación básica - retorna un PDF vacío por ahora
            var htmlContent = "<html><body><h1>Reporte de Datos Vitales</h1><p>Funcionalidad en desarrollo</p></body></html>";
            return System.Text.Encoding.UTF8.GetBytes(htmlContent);
        }

        public async Task<object> GetDatosVitalesJsonAsync(int usuarioId, string tipoDato = null)
        {
            var usuario = await _context.Usuarios.FindAsync(usuarioId);
            if (usuario == null)
            {
                throw new ArgumentException("Usuario no encontrado");
            }

            var query = _context.DatosVitales.Where(d => d.UsuarioId == usuarioId);
            
            if (!string.IsNullOrEmpty(tipoDato))
            {
                query = query.Where(d => d.TipoDato == tipoDato);
            }

            var datos = await query
                .OrderByDescending(d => d.FechaRegistro)
                .Take(100)
                .Select(d => new
                {
                    id = d.Id,
                    tipoDato = d.TipoDato,
                    valor = d.Valor,
                    unidad = d.Unidad,
                    fechaRegistro = d.FechaRegistro,
                    dispositivoOrigen = d.DispositivoOrigen,
                    notas = d.Notas
                })
                .ToListAsync();

            return new
            {
                usuario = new
                {
                    id = usuario.Id,
                    nombre = usuario.Nombre + " " + usuario.Apellido,
                    cargo = usuario.Cargo,
                    departamento = usuario.Departamento
                },
                filtros = new
                {
                    tipoDato = tipoDato ?? "Todos",
                    totalRegistros = datos.Count
                },
                datos,
                fechaGeneracion = DateTime.Now
            };
        }

        public async Task<object> GetReporteAlertasAsync(int usuarioId)
        {
            var usuario = await _context.Usuarios.FindAsync(usuarioId);
            if (usuario == null)
            {
                throw new ArgumentException("Usuario no encontrado");
            }

            var alertas = await _context.Alertas
                .Where(a => a.UsuarioId == usuarioId)
                .OrderByDescending(a => a.FechaCreacion)
                .Select(a => new
                {
                    id = a.Id,
                    tipoAlerta = a.TipoAlerta,
                    severidad = a.Severidad,
                    descripcion = a.Descripcion,
                    fechaCreacion = a.FechaCreacion,
                    leida = a.Leida,
                    resuelta = a.Resuelta
                })
                .ToListAsync();

            return new
            {
                usuario = new
                {
                    id = usuario.Id,
                    nombre = usuario.Nombre + " " + usuario.Apellido,
                    cargo = usuario.Cargo,
                    departamento = usuario.Departamento
                },
                totalAlertas = alertas.Count,
                alertasActivas = alertas.Count(a => !a.resuelta),
                alertas,
                fechaGeneracion = DateTime.Now
            };
        }

        public async Task<object> GetReporteSupervisorAsync(int jefeId)
        {
            var jefe = await _context.Usuarios.FindAsync(jefeId);
            if (jefe == null || jefe.Rol != "Jefe")
            {
                throw new ArgumentException("Supervisor no encontrado");
            }

            var subordinados = await _context.Usuarios
                .Where(u => u.JefeId == jefeId && u.EsActivo)
                .ToListAsync();

            return new
            {
                supervisor = new
                {
                    id = jefe.Id,
                    nombre = jefe.Nombre + " " + jefe.Apellido,
                    departamento = jefe.Departamento
                },
                totalSubordinados = subordinados.Count,
                subordinados = subordinados.Select(s => new
                {
                    id = s.Id,
                    nombre = s.Nombre + " " + s.Apellido,
                    cargo = s.Cargo
                }),
                fechaGeneracion = DateTime.Now
            };
        }

        public async Task<object> GetReporteDepartamentoAsync(string departamento)
        {
            var usuariosDepartamento = await _context.Usuarios
                .Where(u => u.Departamento == departamento && u.EsActivo)
                .ToListAsync();

            return new
            {
                departamento,
                totalEmpleados = usuariosDepartamento.Count,
                empleados = usuariosDepartamento.Select(u => new
                {
                    id = u.Id,
                    nombre = u.Nombre + " " + u.Apellido,
                    cargo = u.Cargo
                }),
                fechaGeneracion = DateTime.Now
            };
        }

        public async Task<object> GetResumenEjecutivoAsync()
        {
            var totalUsuarios = await _context.Usuarios.CountAsync();
            var totalAlertas = await _context.Alertas.CountAsync();
            var totalDatosVitales = await _context.DatosVitales.CountAsync();

            return new
            {
                resumenGeneral = new
                {
                    totalUsuarios,
                    totalAlertas,
                    totalDatosVitales
                },
                fechaGeneracion = DateTime.Now
            };
        }
    }
} 