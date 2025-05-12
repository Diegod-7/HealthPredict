using HealthPredict.BLL;
using HealthPredict.Models;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HealthPredict.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AlertasController : ControllerBase
    {
        private readonly AlertaService _alertaService;

        public AlertasController(AlertaService alertaService)
        {
            _alertaService = alertaService;
        }

        // GET: api/Alertas/Usuario/5
        [HttpGet("Usuario/{usuarioId}")]
        public async Task<ActionResult<IEnumerable<Alerta>>> GetAlertasByUsuario(int usuarioId)
        {
            var alertas = await _alertaService.GetAlertasByUsuarioAsync(usuarioId);
            return Ok(alertas);
        }

        // GET: api/Alertas/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Alerta>> GetAlerta(int id)
        {
            var alerta = await _alertaService.GetAlertaByIdAsync(id);

            if (alerta == null)
            {
                return NotFound();
            }

            return Ok(alerta);
        }

        // POST: api/Alertas
        [HttpPost]
        public async Task<ActionResult<Alerta>> CreateAlerta(Alerta alerta)
        {
            var createdAlerta = await _alertaService.CreateAlertaAsync(alerta);
            return CreatedAtAction(nameof(GetAlerta), new { id = createdAlerta.Id }, createdAlerta);
        }

        // PUT: api/Alertas/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAlerta(int id, Alerta alerta)
        {
            if (id != alerta.Id)
            {
                return BadRequest();
            }

            await _alertaService.UpdateAlertaAsync(alerta);
            return NoContent();
        }

        // DELETE: api/Alertas/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAlerta(int id)
        {
            var result = await _alertaService.DeleteAlertaAsync(id);
            if (!result)
            {
                return NotFound();
            }

            return NoContent();
        }

        // POST: api/Alertas/5/MarcarLeida
        [HttpPost("{id}/MarcarLeida")]
        public async Task<ActionResult<Alerta>> MarcarAlertaComoLeida(int id)
        {
            var alerta = await _alertaService.MarcarComoLeidaAsync(id);
            if (alerta == null)
            {
                return NotFound();
            }

            return Ok(alerta);
        }

        // POST: api/Alertas/5/Resolver
        [HttpPost("{id}/Resolver")]
        public async Task<ActionResult<Alerta>> ResolverAlerta(int id, [FromBody] NotasResolucionModel model)
        {
            var alerta = await _alertaService.ResolverAlertaAsync(id, model.NotasResolucion);
            if (alerta == null)
            {
                return NotFound();
            }

            return Ok(alerta);
        }
    }

    public class NotasResolucionModel
    {
        public string NotasResolucion { get; set; }
    }
} 