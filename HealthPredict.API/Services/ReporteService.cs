using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using HealthPredict.DAL;
using HealthPredict.Models;
using Microsoft.EntityFrameworkCore;
using DinkToPdf;
using DinkToPdf.Contracts;
using System.Linq;
using System.Globalization;

namespace HealthPredict.API.Services
{
    public class ReporteService
    {
        private readonly HealthPredictContext _context;
        private readonly IConverter _converter;
        private readonly bool _pdfServiceAvailable;

        public ReporteService(HealthPredictContext context, IServiceProvider serviceProvider)
        {
            _context = context;
            
            // Intentar obtener el servicio IConverter (podría no estar disponible)
            try
            {
                _converter = (IConverter)serviceProvider.GetService(typeof(IConverter));
                _pdfServiceAvailable = _converter != null;
            }
            catch
            {
                _pdfServiceAvailable = false;
            }
        }

        public async Task<byte[]> GenerarReporteDatosVitalesAsync(int usuarioId, string tipoDato = null)
        {
            if (!_pdfServiceAvailable)
            {
                throw new InvalidOperationException("El servicio de generación de PDF no está disponible. Por favor, asegúrese de que la librería libwkhtmltox.dll esté instalada correctamente.");
            }

            // Obtener el usuario
            var usuario = await _context.Usuarios
                .FirstOrDefaultAsync(u => u.Id == usuarioId);

            if (usuario == null)
            {
                throw new Exception("Usuario no encontrado");
            }

            // Obtener datos vitales
            var query = _context.DatosVitales
                .Where(d => d.UsuarioId == usuarioId);

            if (!string.IsNullOrEmpty(tipoDato))
            {
                query = query.Where(d => d.TipoDato == tipoDato);
            }

            var datosVitales = await query.OrderByDescending(d => d.FechaRegistro).ToListAsync();
            
            if (!datosVitales.Any())
            {
                throw new Exception("No hay datos vitales para generar el reporte");
            }

            // Agrupar datos por tipo
            var datosPorTipo = datosVitales
                .GroupBy(d => d.TipoDato)
                .ToDictionary(g => g.Key, g => g.ToList());

            // Generar HTML para el reporte
            string html = GenerarHtmlReporte(usuario, datosPorTipo);

            // Configurar documento PDF
            var doc = new HtmlToPdfDocument()
            {
                GlobalSettings = {
                    PaperSize = PaperKind.A4,
                    Orientation = Orientation.Portrait,
                    ColorMode = ColorMode.Color,
                    Margins = new MarginSettings { Top = 20, Bottom = 20, Left = 20, Right = 20 }
                },
                Objects = {
                    new ObjectSettings {
                        HtmlContent = html,
                        WebSettings = { DefaultEncoding = "utf-8" }
                    }
                }
            };

            // Convertir HTML a PDF
            return _converter.Convert(doc);
        }

        private string GenerarHtmlReporte(Usuario usuario, Dictionary<string, List<DatoVital>> datosPorTipo)
        {
            var html = $@"
            <!DOCTYPE html>
            <html>
            <head>
                <meta charset='utf-8'>
                <title>Reporte de Datos Vitales - HealthPredict</title>
                <style>
                    body {{ font-family: Arial, sans-serif; margin: 0; padding: 20px; color: #333; }}
                    .header {{ text-align: center; margin-bottom: 30px; }}
                    .logo {{ font-size: 24px; font-weight: bold; color: #4e73df; margin-bottom: 10px; }}
                    .title {{ font-size: 20px; margin-bottom: 5px; }}
                    .subtitle {{ font-size: 16px; color: #666; margin-bottom: 20px; }}
                    .user-info {{ margin-bottom: 30px; border: 1px solid #ddd; padding: 15px; border-radius: 5px; }}
                    .user-info h3 {{ margin-top: 0; color: #4e73df; }}
                    .section {{ margin-bottom: 30px; }}
                    .section h3 {{ color: #4e73df; border-bottom: 1px solid #eee; padding-bottom: 5px; }}
                    table {{ width: 100%; border-collapse: collapse; margin-bottom: 20px; }}
                    th, td {{ border: 1px solid #ddd; padding: 8px; text-align: left; }}
                    th {{ background-color: #f8f9fc; }}
                    .stat-box {{ display: inline-block; width: 22%; margin-right: 2%; padding: 10px; border: 1px solid #ddd; border-radius: 5px; margin-bottom: 15px; }}
                    .stat-label {{ font-size: 12px; color: #666; }}
                    .stat-value {{ font-size: 18px; font-weight: bold; color: #4e73df; }}
                    .footer {{ text-align: center; margin-top: 40px; font-size: 12px; color: #666; }}
                    .page-break {{ page-break-after: always; }}
                </style>
            </head>
            <body>
                <div class='header'>
                    <div class='logo'>HealthPredict</div>
                    <div class='title'>Reporte de Datos Vitales</div>
                    <div class='subtitle'>Generado el {DateTime.Now.ToString("dd/MM/yyyy HH:mm")}</div>
                </div>

                <div class='user-info'>
                    <h3>Información del Paciente</h3>
                    <table>
                        <tr>
                            <th>Nombre Completo</th>
                            <td>{usuario.Nombre} {usuario.Apellido}</td>
                            <th>Fecha de Nacimiento</th>
                            <td>{usuario.FechaNacimiento.ToString("dd/MM/yyyy")}</td>
                        </tr>
                        <tr>
                            <th>Género</th>
                            <td>{usuario.Genero}</td>
                            <th>Edad</th>
                            <td>{CalcularEdad(usuario.FechaNacimiento)} años</td>
                        </tr>
                        <tr>
                            <th>Altura</th>
                            <td>{usuario.Altura} m</td>
                            <th>Peso</th>
                            <td>{usuario.Peso} kg</td>
                        </tr>
                    </table>
                </div>";

            // Agregar secciones para cada tipo de dato vital
            foreach (var tipo in datosPorTipo.Keys)
            {
                var datos = datosPorTipo[tipo];
                var ultimoDato = datos.OrderByDescending(d => d.FechaRegistro).First();
                var promedio = datos.Average(d => d.Valor);
                var minimo = datos.Min(d => d.Valor);
                var maximo = datos.Max(d => d.Valor);

                html += $@"
                <div class='section'>
                    <h3>{tipo}</h3>
                    
                    <div>
                        <div class='stat-box'>
                            <div class='stat-label'>Último Valor</div>
                            <div class='stat-value'>{ultimoDato.Valor.ToString("0.0")} {ultimoDato.Unidad}</div>
                            <div class='stat-label'>{ultimoDato.FechaRegistro.ToString("dd/MM/yyyy HH:mm")}</div>
                        </div>
                        <div class='stat-box'>
                            <div class='stat-label'>Promedio</div>
                            <div class='stat-value'>{promedio.ToString("0.0")} {ultimoDato.Unidad}</div>
                        </div>
                        <div class='stat-box'>
                            <div class='stat-label'>Mínimo</div>
                            <div class='stat-value'>{minimo.ToString("0.0")} {ultimoDato.Unidad}</div>
                        </div>
                        <div class='stat-box'>
                            <div class='stat-label'>Máximo</div>
                            <div class='stat-value'>{maximo.ToString("0.0")} {ultimoDato.Unidad}</div>
                        </div>
                    </div>
                    
                    <h4>Últimas 10 Mediciones</h4>
                    <table>
                        <tr>
                            <th>Fecha</th>
                            <th>Valor</th>
                            <th>Dispositivo</th>
                            <th>Notas</th>
                        </tr>";

                // Agregar filas para las últimas 10 mediciones
                foreach (var dato in datos.OrderByDescending(d => d.FechaRegistro).Take(10))
                {
                    html += $@"
                        <tr>
                            <td>{dato.FechaRegistro.ToString("dd/MM/yyyy HH:mm")}</td>
                            <td>{dato.Valor.ToString("0.0")} {dato.Unidad}</td>
                            <td>{dato.DispositivoOrigen ?? "No especificado"}</td>
                            <td>{dato.Notas ?? ""}</td>
                        </tr>";
                }

                html += @"
                    </table>
                </div>";
            }

            // Agregar pie de página
            html += $@"
                <div class='footer'>
                    <p>Este reporte fue generado automáticamente por HealthPredict. La información contenida es confidencial y está destinada únicamente para uso médico.</p>
                    <p>© {DateTime.Now.Year} HealthPredict - Todos los derechos reservados</p>
                </div>
            </body>
            </html>";

            return html;
        }

        private int CalcularEdad(DateTime fechaNacimiento)
        {
            var hoy = DateTime.Today;
            var edad = hoy.Year - fechaNacimiento.Year;
            if (fechaNacimiento.Date > hoy.AddYears(-edad)) edad--;
            return edad;
        }
    }
} 