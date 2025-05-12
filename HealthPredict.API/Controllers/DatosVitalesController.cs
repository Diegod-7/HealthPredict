using HealthPredict.BLL;
using HealthPredict.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HealthPredict.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DatosVitalesController : ControllerBase
    {
        private readonly DatoVitalService _datoVitalService;
        private readonly AlertaService _alertaService;

        public DatosVitalesController(DatoVitalService datoVitalService, AlertaService alertaService)
        {
            _datoVitalService = datoVitalService;
            _alertaService = alertaService;
        }

        // GET: api/DatosVitales/Usuario/5
        [HttpGet("Usuario/{usuarioId}")]
        public async Task<ActionResult<IEnumerable<DatoVital>>> GetDatosVitalesByUsuario(int usuarioId)
        {
            var datosVitales = await _datoVitalService.GetDatosVitalesByUsuarioAsync(usuarioId);
            return Ok(datosVitales);
        }

        // GET: api/DatosVitales/5
        [HttpGet("{id}")]
        public async Task<ActionResult<DatoVital>> GetDatoVital(int id)
        {
            var datoVital = await _datoVitalService.GetDatoVitalByIdAsync(id);

            if (datoVital == null)
            {
                return NotFound();
            }

            return Ok(datoVital);
        }

        // POST: api/DatosVitales
        [HttpPost]
        public async Task<ActionResult<DatoVital>> CreateDatoVital(DatoVital datoVital)
        {
            var createdDatoVital = await _datoVitalService.CreateDatoVitalAsync(datoVital);
            
            // Verificar si el valor está fuera de rango
            bool fueraDeRango = await _datoVitalService.VerificarValorFueraDeRango(createdDatoVital);
            if (fueraDeRango)
            {
                // Generar alerta automática
                await _alertaService.GenerarAlertaPorDatoVitalAsync(
                    createdDatoVital, 
                    "valor_anormal", 
                    "media");
            }
            
            return CreatedAtAction(nameof(GetDatoVital), new { id = createdDatoVital.Id }, createdDatoVital);
        }

        // PUT: api/DatosVitales/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateDatoVital(int id, DatoVital datoVital)
        {
            if (id != datoVital.Id)
            {
                return BadRequest();
            }

            await _datoVitalService.UpdateDatoVitalAsync(datoVital);
            return NoContent();
        }

        // DELETE: api/DatosVitales/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDatoVital(int id)
        {
            var result = await _datoVitalService.DeleteDatoVitalAsync(id);
            if (!result)
            {
                return NotFound();
            }

            return NoContent();
        }

        // GET: api/DatosVitales/Estadisticas
        [HttpGet("Estadisticas")]
        public async Task<ActionResult<Dictionary<string, decimal>>> GetEstadisticas(
            [FromQuery] int usuarioId, 
            [FromQuery] string tipoDato, 
            [FromQuery] DateTime fechaInicio, 
            [FromQuery] DateTime fechaFin)
        {
            var estadisticas = await _datoVitalService.GetEstadisticasAsync(usuarioId, tipoDato, fechaInicio, fechaFin);
            return Ok(estadisticas);
        }
    }
} 