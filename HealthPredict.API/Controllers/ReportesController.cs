using System;
using System.Threading.Tasks;
using HealthPredict.API.Services;
using Microsoft.AspNetCore.Mvc;
using DinkToPdf.Contracts;

namespace HealthPredict.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReportesController : ControllerBase
    {
        private readonly ReporteService _reporteService;
        private readonly IConverter _converter;
        private readonly bool _pdfServiceAvailable;

        public ReportesController(ReporteService reporteService, IServiceProvider serviceProvider)
        {
            _reporteService = reporteService;

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

        // GET: api/Reportes/DatosVitales/{usuarioId}
        [HttpGet("DatosVitales/{usuarioId}")]
        public async Task<IActionResult> GetReporteDatosVitales(int usuarioId, [FromQuery] string tipoDato = null)
        {
            try
            {
                // Verificar si el servicio de PDF está disponible
                if (!_pdfServiceAvailable)
                {
                    return StatusCode(503, "El servicio de generación de PDF no está disponible. Por favor, asegúrese de que la librería libwkhtmltox.dll esté instalada correctamente.");
                }

                var pdfBytes = await _reporteService.GenerarReporteDatosVitalesAsync(usuarioId, tipoDato);
                
                // Generar nombre de archivo para la descarga
                string fileName = $"HealthPredict_Reporte_{usuarioId}_{DateTime.Now:yyyyMMdd_HHmmss}.pdf";
                
                // Devolver el archivo PDF
                return File(pdfBytes, "application/pdf", fileName);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al generar el reporte: {ex.Message}");
            }
        }
    }
} 