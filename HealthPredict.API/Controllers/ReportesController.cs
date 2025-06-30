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

        // GET: api/Reportes
        [HttpGet]
        public IActionResult GetTiposReportes()
        {
            var tiposReportes = new
            {
                disponibles = new[]
                {
                    new { tipo = "datos-vitales", descripcion = "Reporte de datos vitales por usuario", endpoint = "/api/Reportes/DatosVitales/{usuarioId}" },
                    new { tipo = "alertas", descripcion = "Reporte de alertas por usuario", endpoint = "/api/Reportes/Alertas/{usuarioId}" },
                    new { tipo = "departamento", descripcion = "Reporte consolidado por departamento", endpoint = "/api/Reportes/Departamento/{departamento}" },
                    new { tipo = "supervisor", descripcion = "Reporte para supervisores", endpoint = "/api/Reportes/Supervisor/{jefeId}" },
                    new { tipo = "resumen-ejecutivo", descripcion = "Resumen ejecutivo general", endpoint = "/api/Reportes/ResumenEjecutivo" }
                },
                formatos = new[] { "PDF", "JSON", "CSV" },
                servicioDisponible = _pdfServiceAvailable
            };

            return Ok(tiposReportes);
        }

        // GET: api/Reportes/DatosVitales/{usuarioId}
        [HttpGet("DatosVitales/{usuarioId}")]
        public async Task<IActionResult> GetReporteDatosVitales(int usuarioId, [FromQuery] string tipoDato = null, [FromQuery] string formato = "pdf")
        {
            try
            {
                if (formato.ToLower() == "json")
                {
                    var datosJson = await _reporteService.GetDatosVitalesJsonAsync(usuarioId, tipoDato);
                    return Ok(datosJson);
                }

                // Verificar si el servicio de PDF está disponible
                if (!_pdfServiceAvailable && formato.ToLower() == "pdf")
                {
                    return StatusCode(503, new { 
                        error = "El servicio de generación de PDF no está disponible",
                        alternativa = "Puede solicitar el reporte en formato JSON agregando ?formato=json"
                    });
                }

                var pdfBytes = await _reporteService.GenerarReporteDatosVitalesAsync(usuarioId, tipoDato);
                
                // Generar nombre de archivo para la descarga
                string fileName = $"HealthPredict_DatosVitales_{usuarioId}_{DateTime.Now:yyyyMMdd_HHmmss}.pdf";
                
                // Devolver el archivo PDF
                return File(pdfBytes, "application/pdf", fileName);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "Error al generar el reporte", detalle = ex.Message });
            }
        }

        // GET: api/Reportes/Alertas/{usuarioId}
        [HttpGet("Alertas/{usuarioId}")]
        public async Task<IActionResult> GetReporteAlertas(int usuarioId, [FromQuery] string formato = "json")
        {
            try
            {
                var reporteAlertas = await _reporteService.GetReporteAlertasAsync(usuarioId);
                
                if (formato.ToLower() == "json")
                {
                    return Ok(reporteAlertas);
                }

                // Para PDF, necesitaríamos implementar la generación
                return Ok(new { 
                    mensaje = "Reporte de alertas disponible en formato JSON",
                    datos = reporteAlertas,
                    nota = "Formato PDF en desarrollo"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "Error al generar reporte de alertas", detalle = ex.Message });
            }
        }

        // GET: api/Reportes/Supervisor/{jefeId}
        [HttpGet("Supervisor/{jefeId}")]
        public async Task<IActionResult> GetReporteSupervisor(int jefeId, [FromQuery] string formato = "json")
        {
            try
            {
                var reporteSupervisor = await _reporteService.GetReporteSupervisorAsync(jefeId);
                
                if (formato.ToLower() == "json")
                {
                    return Ok(reporteSupervisor);
                }

                return Ok(new { 
                    mensaje = "Reporte de supervisor disponible en formato JSON",
                    datos = reporteSupervisor,
                    nota = "Formato PDF en desarrollo"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "Error al generar reporte de supervisor", detalle = ex.Message });
            }
        }

        // GET: api/Reportes/Departamento/{departamento}
        [HttpGet("Departamento/{departamento}")]
        public async Task<IActionResult> GetReporteDepartamento(string departamento, [FromQuery] string formato = "json")
        {
            try
            {
                var reporteDepartamento = await _reporteService.GetReporteDepartamentoAsync(departamento);
                
                if (formato.ToLower() == "json")
                {
                    return Ok(reporteDepartamento);
                }

                return Ok(new { 
                    mensaje = "Reporte de departamento disponible en formato JSON",
                    datos = reporteDepartamento,
                    nota = "Formato PDF en desarrollo"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "Error al generar reporte de departamento", detalle = ex.Message });
            }
        }

        // GET: api/Reportes/ResumenEjecutivo
        [HttpGet("ResumenEjecutivo")]
        public async Task<IActionResult> GetResumenEjecutivo([FromQuery] string formato = "json")
        {
            try
            {
                var resumenEjecutivo = await _reporteService.GetResumenEjecutivoAsync();
                
                if (formato.ToLower() == "json")
                {
                    return Ok(resumenEjecutivo);
                }

                return Ok(new { 
                    mensaje = "Resumen ejecutivo disponible en formato JSON",
                    datos = resumenEjecutivo,
                    nota = "Formato PDF en desarrollo"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "Error al generar resumen ejecutivo", detalle = ex.Message });
            }
        }
    }
} 