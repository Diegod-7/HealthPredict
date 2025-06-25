using HealthPredict.BLL;
using HealthPredict.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HealthPredict.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsuariosController : ControllerBase
    {
        private readonly UsuarioService _usuarioService;

        public UsuariosController(UsuarioService usuarioService)
        {
            _usuarioService = usuarioService;
        }

        // GET: api/Usuarios
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Usuario>>> GetUsuarios()
        {
            try
            {
                var usuarios = await _usuarioService.GetUsuariosAsync();
                return Ok(usuarios);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error interno del servidor: {ex.Message}");
            }
        }

        // GET: api/Usuarios/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Usuario>> GetUsuario(int id)
        {
            try
            {
                var usuario = await _usuarioService.GetUsuarioByIdAsync(id);

                if (usuario == null)
                {
                    return NotFound($"Usuario con ID {id} no encontrado");
                }

                return Ok(usuario);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error interno del servidor: {ex.Message}");
            }
        }

        // GET: api/Usuarios/email/user@example.com
        [HttpGet("email/{email}")]
        public async Task<ActionResult<Usuario>> GetUsuarioByEmail(string email)
        {
            try
            {
                var usuario = await _usuarioService.GetUsuarioByEmailAsync(email);

                if (usuario == null)
                {
                    return NotFound($"Usuario con email {email} no encontrado");
                }

                return Ok(usuario);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error interno del servidor: {ex.Message}");
            }
        }

        // POST: api/Usuarios
        [HttpPost]
        public async Task<ActionResult<Usuario>> CreateUsuario(Usuario usuario)
        {
            try
            {
                if (await _usuarioService.EmailExistsAsync(usuario.Email))
                {
                    return Conflict($"El email {usuario.Email} ya está registrado");
                }

                var createdUsuario = await _usuarioService.CreateUsuarioAsync(usuario);
                return CreatedAtAction(nameof(GetUsuario), new { id = createdUsuario.Id }, createdUsuario);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error interno del servidor: {ex.Message}");
            }
        }

        // PUT: api/Usuarios/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUsuario(int id, Usuario usuario)
        {
            if (id != usuario.Id)
            {
                return BadRequest("El ID de la ruta no coincide con el ID del usuario");
            }

            try
            {
                var success = await _usuarioService.UpdateUsuarioAsync(usuario);
                if (!success)
                {
                    return NotFound($"Usuario con ID {id} no encontrado");
                }

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error interno del servidor: {ex.Message}");
            }
        }

        // DELETE: api/Usuarios/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUsuario(int id)
        {
            try
            {
                var result = await _usuarioService.DeleteUsuarioAsync(id);
                if (!result)
                {
                    return NotFound($"Usuario con ID {id} no encontrado");
                }

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error interno del servidor: {ex.Message}");
            }
        }

        // POST: api/Usuarios/authenticate
        [HttpPost("authenticate")]
        public async Task<ActionResult<Usuario>> Authenticate([FromBody] LoginModel model)
        {
            try
            {
                var usuario = await _usuarioService.AuthenticateAsync(model.Email, model.Password);

                if (usuario == null)
                {
                    return Unauthorized();
                }

                return Ok(usuario);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error interno del servidor: {ex.Message}");
            }
        }

        // GET: api/Usuarios/Jefes
        [HttpGet("Jefes")]
        public async Task<ActionResult<IEnumerable<Usuario>>> GetJefes()
        {
            try
            {
                var jefes = await _usuarioService.GetJefesAsync();
                return Ok(jefes);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error interno del servidor: {ex.Message}");
            }
        }

        // GET: api/Usuarios/Trabajadores
        [HttpGet("Trabajadores")]
        public async Task<ActionResult<IEnumerable<Usuario>>> GetTrabajadores()
        {
            try
            {
                var trabajadores = await _usuarioService.GetTrabajadoresAsync();
                return Ok(trabajadores);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error interno del servidor: {ex.Message}");
            }
        }

        // GET: api/Usuarios/Jefe/5/Subordinados
        [HttpGet("Jefe/{jefeId}/Subordinados")]
        public async Task<ActionResult<IEnumerable<Usuario>>> GetSubordinadosByJefe(int jefeId)
        {
            try
            {
                // Verificar que el usuario sea jefe
                if (!await _usuarioService.EsJefeAsync(jefeId))
                {
                    return BadRequest("El usuario especificado no es un jefe");
                }

                var subordinados = await _usuarioService.GetSubordinadosByJefeAsync(jefeId);
                return Ok(subordinados);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error interno del servidor: {ex.Message}");
            }
        }

        // GET: api/Usuarios/Dashboard/Jefe/5
        [HttpGet("Dashboard/Jefe/{jefeId}")]
        public async Task<ActionResult<object>> GetDashboardJefe(int jefeId)
        {
            try
            {
                // Verificar que el usuario sea jefe
                if (!await _usuarioService.EsJefeAsync(jefeId))
                {
                    return BadRequest("El usuario especificado no es un jefe");
                }

                var estadisticas = await _usuarioService.GetEstadisticasGeneralesJefeAsync(jefeId);
                return Ok(estadisticas);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error interno del servidor: {ex.Message}");
            }
        }

        // GET: api/Usuarios/Departamento/Desarrollo
        [HttpGet("Departamento/{departamento}")]
        public async Task<ActionResult<IEnumerable<Usuario>>> GetUsuariosByDepartamento(string departamento)
        {
            try
            {
                var usuarios = await _usuarioService.GetUsuariosByDepartamentoAsync(departamento);
                return Ok(usuarios);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error interno del servidor: {ex.Message}");
            }
        }

        // GET: api/Usuarios/VerificarAcceso/5/7
        [HttpGet("VerificarAcceso/{usuarioSolicitante}/{usuarioObjetivo}")]
        public async Task<ActionResult<bool>> VerificarAccesoADatos(int usuarioSolicitante, int usuarioObjetivo)
        {
            try
            {
                var puedeAcceder = await _usuarioService.PuedeAccederADatosAsync(usuarioSolicitante, usuarioObjetivo);
                return Ok(puedeAcceder);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error interno del servidor: {ex.Message}");
            }
        }

        // ✅ ENDPOINT TEMPORAL PARA INICIALIZAR DATOS
        [HttpPost("inicializar-datos")]
        public async Task<ActionResult> InicializarDatos()
        {
            try
            {
                // Verificar si ya existen usuarios
                var usuariosExistentes = await _usuarioService.GetUsuariosAsync();
                if (usuariosExistentes.Any())
                {
                    return Ok(new { mensaje = "Los usuarios ya están inicializados", total = usuariosExistentes.Count() });
                }

                // Crear usuarios de prueba
                var jefe = new Usuario
                {
                    Nombre = "Carlos",
                    Apellido = "Rodriguez",
                    Email = "jefe@healthpredict.com",
                    Password = "admin123",
                    FechaNacimiento = new DateTime(1980, 5, 15),
                    Genero = "Masculino",
                    Altura = 175,
                    Peso = 80,
                    Rol = "Jefe",
                    Departamento = "Administración",
                    Cargo = "Gerente General",
                    JefeId = null,
                    EsActivo = true,
                    FechaRegistro = DateTime.Now,
                    UltimoAcceso = DateTime.Now,
                    EsProfesionalMedico = false
                };

                var diego = new Usuario
                {
                    Nombre = "Diego",
                    Apellido = "Diaz",
                    Email = "diego.diaz@healthpredict.com",
                    Password = "diego123",
                    FechaNacimiento = new DateTime(1995, 3, 10),
                    Genero = "Masculino",
                    Altura = 180,
                    Peso = 75,
                    Rol = "Trabajador",
                    Departamento = "Desarrollo",
                    Cargo = "Desarrollador Senior",
                    JefeId = 1, // Se asignará después
                    EsActivo = true,
                    FechaRegistro = DateTime.Now,
                    UltimoAcceso = DateTime.Now,
                    EsProfesionalMedico = false
                };

                var matias = new Usuario
                {
                    Nombre = "Matias",
                    Apellido = "Maripangue",
                    Email = "matias.maripangue@healthpredict.com",
                    Password = "matias123",
                    FechaNacimiento = new DateTime(1992, 8, 22),
                    Genero = "Masculino",
                    Altura = 170,
                    Peso = 68,
                    Rol = "Trabajador",
                    Departamento = "Desarrollo",
                    Cargo = "Desarrollador",
                    JefeId = 1, // Se asignará después
                    EsActivo = true,
                    FechaRegistro = DateTime.Now,
                    UltimoAcceso = DateTime.Now,
                    EsProfesionalMedico = false
                };

                var iahn = new Usuario
                {
                    Nombre = "Iahn",
                    Apellido = "Vera",
                    Email = "iahn.vera@healthpredict.com",
                    Password = "iahn123",
                    FechaNacimiento = new DateTime(1993, 12, 5),
                    Genero = "Masculino",
                    Altura = 178,
                    Peso = 72,
                    Rol = "Trabajador",
                    Departamento = "Desarrollo",
                    Cargo = "Desarrollador",
                    JefeId = 1, // Se asignará después
                    EsActivo = true,
                    FechaRegistro = DateTime.Now,
                    UltimoAcceso = DateTime.Now,
                    EsProfesionalMedico = false
                };

                // Crear el jefe primero
                var jefeCreado = await _usuarioService.CreateUsuarioAsync(jefe);
                
                // Actualizar el JefeId para los trabajadores
                diego.JefeId = jefeCreado.Id;
                matias.JefeId = jefeCreado.Id;
                iahn.JefeId = jefeCreado.Id;

                // Crear los trabajadores
                await _usuarioService.CreateUsuarioAsync(diego);
                await _usuarioService.CreateUsuarioAsync(matias);
                await _usuarioService.CreateUsuarioAsync(iahn);

                return Ok(new { 
                    mensaje = "Usuarios inicializados correctamente",
                    usuarios = new[] {
                        new { email = "jefe@healthpredict.com", password = "admin123", rol = "Jefe" },
                        new { email = "diego.diaz@healthpredict.com", password = "diego123", rol = "Trabajador" },
                        new { email = "matias.maripangue@healthpredict.com", password = "matias123", rol = "Trabajador" },
                        new { email = "iahn.vera@healthpredict.com", password = "iahn123", rol = "Trabajador" }
                    }
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al inicializar datos: {ex.Message}");
            }
        }

        /// <summary>
        /// Actualiza el rol de un usuario
        /// </summary>
        /// <param name="usuarioId">ID del usuario</param>
        /// <param name="nuevoRol">Nuevo rol del usuario</param>
        /// <returns>Usuario actualizado</returns>
        [HttpPut("{usuarioId}/rol")]
        public async Task<IActionResult> ActualizarRol(int usuarioId, [FromBody] string nuevoRol)
        {
            try
            {
                var usuario = await _usuarioService.GetUsuarioByIdAsync(usuarioId);
                if (usuario == null)
                {
                    return NotFound("Usuario no encontrado");
                }

                usuario.Rol = nuevoRol;
                
                // Si es jefe, limpiar jefeId
                if (nuevoRol == "Jefe")
                {
                    usuario.JefeId = null;
                }

                await _usuarioService.UpdateUsuarioAsync(usuario);
                
                return Ok(new { mensaje = $"Rol actualizado a {nuevoRol} exitosamente", usuario });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al actualizar rol: {ex.Message}");
            }
        }
    }

    public class LoginModel
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }
} 