using HealthPredict.BLL;
using HealthPredict.Models;
using HealthPredict.DAL;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace HealthPredict.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsuariosController : ControllerBase
    {
        private readonly UsuarioService _usuarioService;
        private readonly HealthPredictContext _context;

        public UsuariosController(UsuarioService usuarioService, HealthPredictContext context)
        {
            _usuarioService = usuarioService;
            _context = context;
        }

        // GET: api/Usuarios
        [HttpGet]
        public async Task<ActionResult<IEnumerable<object>>> GetUsuarios()
        {
            try
            {
                var usuarios = await _usuarioService.GetUsuariosAsync();
                var usuariosResponse = usuarios.Select(u => new
                {
                    id = u.Id,
                    nombre = u.Nombre,
                    apellido = u.Apellido,
                    email = u.Email,
                    nombreCompleto = u.NombreCompleto,
                    rol = u.Rol,
                    departamento = u.Departamento,
                    cargo = u.Cargo,
                    jefeId = u.JefeId,
                    esJefe = u.EsJefe,
                    esTrabajador = u.EsTrabajador,
                    fechaRegistro = u.FechaRegistro,
                    ultimoAcceso = u.UltimoAcceso
                });
                return Ok(usuariosResponse);
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
                // Validaciones básicas
                if (string.IsNullOrWhiteSpace(usuario.Nombre))
                    return BadRequest("El nombre es requerido");
                
                if (string.IsNullOrWhiteSpace(usuario.Apellido))
                    return BadRequest("El apellido es requerido");
                
                if (string.IsNullOrWhiteSpace(usuario.Email))
                    return BadRequest("El email es requerido");
                
                if (string.IsNullOrWhiteSpace(usuario.Password))
                    return BadRequest("La contraseña es requerida");

                if (await _usuarioService.EmailExistsAsync(usuario.Email))
                {
                    return Conflict($"El email {usuario.Email} ya está registrado");
                }

                // Asignar valores por defecto si no están presentes
                if (string.IsNullOrWhiteSpace(usuario.Genero))
                    usuario.Genero = "Masculino";
                
                if (usuario.Altura <= 0)
                    usuario.Altura = 175;
                
                if (usuario.Peso <= 0)
                    usuario.Peso = 70;
                
                if (usuario.FechaNacimiento == default(DateTime))
                    usuario.FechaNacimiento = new DateTime(1990, 1, 1);
                
                if (string.IsNullOrWhiteSpace(usuario.Rol))
                    usuario.Rol = "Trabajador";
                
                // Las fechas de registro y último acceso se asignan automáticamente en el servicio
                usuario.FechaRegistro = DateTime.UtcNow;
                usuario.UltimoAcceso = DateTime.UtcNow;
                usuario.EsActivo = true;

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
        public async Task<ActionResult<object>> Authenticate([FromBody] LoginModel model)
        {
            try
            {
                var usuario = await _usuarioService.AuthenticateAsync(model.Email, model.Password);

                if (usuario == null)
                {
                    return Unauthorized(new { success = false, message = "Credenciales inválidas" });
                }

                // Devolver un objeto simple sin referencias circulares
                var usuarioResponse = new
                {
                    id = usuario.Id,
                    nombre = usuario.Nombre,
                    apellido = usuario.Apellido,
                    email = usuario.Email,
                    nombreCompleto = usuario.NombreCompleto,
                    rol = usuario.Rol,
                    departamento = usuario.Departamento,
                    cargo = usuario.Cargo,
                    jefeId = usuario.JefeId,
                    esJefe = usuario.EsJefe,
                    esTrabajador = usuario.EsTrabajador,
                    fechaRegistro = usuario.FechaRegistro,
                    ultimoAcceso = usuario.UltimoAcceso
                };

                return Ok(new { success = true, usuario = usuarioResponse });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = $"Error interno del servidor: {ex.Message}" });
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

        /// <summary>
        /// Reinicializa los usuarios con los datos correctos del sistema
        /// </summary>
        /// <returns>Resultado de la operación</returns>
        [HttpPost("reinicializar-usuarios")]
        public async Task<IActionResult> ReinicializarUsuarios()
        {
            try
            {
                // Eliminar todos los usuarios existentes
                var usuariosExistentes = await _context.Usuarios.ToListAsync();
                _context.Usuarios.RemoveRange(usuariosExistentes);
                await _context.SaveChangesAsync();

                // Crear los nuevos usuarios correctos
                var usuarios = new List<Usuario>
                {
                    // Jefe
                    new Usuario
                    {
                        Nombre = "Carlos",
                        Apellido = "Rodriguez",
                        Email = "carlos.rodriguez@healthpredict.com",
                        Password = "admin123",
                        FechaNacimiento = new DateTime(1985, 3, 15),
                        Genero = "Masculino",
                        Altura = 178,
                        Peso = 80.0m,
                        FechaRegistro = DateTime.UtcNow,
                        UltimoAcceso = DateTime.UtcNow,
                        EsProfesionalMedico = false,
                        Rol = "Jefe",
                        Departamento = "Administración",
                        Cargo = "Gerente General",
                        JefeId = null,
                        EsActivo = true
                    },
                    
                    // Trabajadores
                    new Usuario
                    {
                        Nombre = "Diego",
                        Apellido = "Diaz",
                        Email = "diego.diaz@healthpredict.com",
                        Password = "diego123",
                        FechaNacimiento = new DateTime(1992, 8, 22),
                        Genero = "Masculino",
                        Altura = 175,
                        Peso = 75.0m,
                        FechaRegistro = DateTime.UtcNow,
                        UltimoAcceso = DateTime.UtcNow,
                        EsProfesionalMedico = false,
                        Rol = "Trabajador",
                        Departamento = "Desarrollo",
                        Cargo = "Desarrollador Full Stack",
                        JefeId = null, // Se actualizará después
                        EsActivo = true
                    },
                    
                    new Usuario
                    {
                        Nombre = "Iahn",
                        Apellido = "Vera",
                        Email = "iahn.vera@healthpredict.com",
                        Password = "iahn123",
                        FechaNacimiento = new DateTime(1994, 11, 10),
                        Genero = "Masculino",
                        Altura = 172,
                        Peso = 70.0m,
                        FechaRegistro = DateTime.UtcNow,
                        UltimoAcceso = DateTime.UtcNow,
                        EsProfesionalMedico = false,
                        Rol = "Trabajador",
                        Departamento = "Desarrollo",
                        Cargo = "Desarrollador Frontend",
                        JefeId = null, // Se actualizará después
                        EsActivo = true
                    },
                    
                    new Usuario
                    {
                        Nombre = "Matias",
                        Apellido = "Maripangue",
                        Email = "matias.maripangue@healthpredict.com",
                        Password = "matias123",
                        FechaNacimiento = new DateTime(1993, 6, 5),
                        Genero = "Masculino",
                        Altura = 180,
                        Peso = 82.0m,
                        FechaRegistro = DateTime.UtcNow,
                        UltimoAcceso = DateTime.UtcNow,
                        EsProfesionalMedico = false,
                        Rol = "Trabajador",
                        Departamento = "Desarrollo",
                        Cargo = "Desarrollador Backend",
                        JefeId = null, // Se actualizará después
                        EsActivo = true
                    }
                };

                // Agregar usuarios
                await _context.Usuarios.AddRangeAsync(usuarios);
                await _context.SaveChangesAsync();

                // Actualizar JefeId de los trabajadores para que apunten al jefe
                var jefe = await _context.Usuarios.FirstOrDefaultAsync(u => u.Email == "carlos.rodriguez@healthpredict.com");
                if (jefe != null)
                {
                    var trabajadores = await _context.Usuarios
                        .Where(u => u.Rol == "Trabajador")
                        .ToListAsync();
                    
                    foreach (var trabajador in trabajadores)
                    {
                        trabajador.JefeId = jefe.Id;
                    }
                    
                    await _context.SaveChangesAsync();
                }

                return Ok(new { 
                    mensaje = "Usuarios reinicializados exitosamente",
                    usuarios = usuarios.Select(u => new { u.Nombre, u.Apellido, u.Email, u.Rol }).ToList()
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al reinicializar usuarios: {ex.Message}");
            }
        }

        /// <summary>
        /// Crea un nuevo usuario
        /// </summary>
        /// <param name="nuevoUsuario">Datos del nuevo usuario</param>
        /// <returns>Usuario creado</returns>
        [HttpPost("crear")]
        public async Task<IActionResult> CrearUsuario([FromBody] CrearUsuarioRequest nuevoUsuario)
        {
            try
            {
                // Verificar si el email ya existe
                var usuarioExistente = await _context.Usuarios
                    .FirstOrDefaultAsync(u => u.Email == nuevoUsuario.Email);
                
                if (usuarioExistente != null)
                {
                    return BadRequest($"Ya existe un usuario con el email {nuevoUsuario.Email}");
                }

                var usuario = new Usuario
                {
                    Nombre = nuevoUsuario.Nombre,
                    Apellido = nuevoUsuario.Apellido,
                    Email = nuevoUsuario.Email,
                    Password = nuevoUsuario.Password,
                    FechaNacimiento = nuevoUsuario.FechaNacimiento ?? new DateTime(1990, 1, 1),
                    Genero = nuevoUsuario.Genero ?? "Masculino",
                    Altura = nuevoUsuario.Altura ?? 175,
                    Peso = nuevoUsuario.Peso ?? 70.0m,
                    FechaRegistro = DateTime.UtcNow,
                    UltimoAcceso = DateTime.UtcNow,
                    EsProfesionalMedico = false,
                    Rol = nuevoUsuario.Rol ?? "Trabajador",
                    Departamento = nuevoUsuario.Departamento ?? "Desarrollo",
                    Cargo = nuevoUsuario.Cargo ?? "Desarrollador",
                    JefeId = nuevoUsuario.JefeId,
                    EsActivo = true
                };

                await _context.Usuarios.AddAsync(usuario);
                await _context.SaveChangesAsync();

                return Ok(new { 
                    mensaje = "Usuario creado exitosamente",
                    usuario = new { 
                        usuario.Id, 
                        usuario.Nombre, 
                        usuario.Apellido, 
                        usuario.Email, 
                        usuario.Rol 
                    }
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al crear usuario: {ex.Message}");
            }
        }

        /// <summary>
        /// Fuerza la inicialización de usuarios usando el DbInitializer
        /// </summary>
        /// <returns>Resultado de la operación</returns>
        [HttpPost("forzar-inicializacion")]
        public async Task<IActionResult> ForzarInicializacion()
        {
            try
            {
                await DbInitializer.InitializeAsync(_context);
                
                var usuarios = await _context.Usuarios.ToListAsync();
                
                return Ok(new { 
                    mensaje = "Inicialización forzada completada",
                    totalUsuarios = usuarios.Count,
                    usuarios = usuarios.Select(u => new { u.Id, u.Nombre, u.Apellido, u.Email, u.Rol }).ToList()
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al forzar inicialización: {ex.Message}");
            }
        }

        /// <summary>
        /// Diagnóstica el estado de la base de datos y la conexión
        /// </summary>
        /// <returns>Información de diagnóstico</returns>
        [HttpGet("diagnostico-bd")]
        public async Task<IActionResult> DiagnosticoBD()
        {
            var diagnostico = new
            {
                timestamp = DateTime.UtcNow,
                connectionString = Environment.GetEnvironmentVariable("DATABASE_URL") != null ? "✅ Configurado" : "❌ No encontrado",
                environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "No configurado"
            };

            try
            {
                // Probar conexión
                var puedeConectar = await _context.Database.CanConnectAsync();
                
                // Contar usuarios
                var totalUsuarios = await _context.Usuarios.CountAsync();
                
                // Verificar tablas
                var tablas = await _context.Database.SqlQueryRaw<string>(
                    "SELECT table_name FROM information_schema.tables WHERE table_schema = 'public'"
                ).ToListAsync();

                return Ok(new
                {
                    diagnostico,
                    conexion = new
                    {
                        puedeConectar,
                        mensaje = puedeConectar ? "✅ Conexión exitosa" : "❌ No se puede conectar"
                    },
                    baseDatos = new
                    {
                        totalUsuarios,
                        tablas = tablas.ToArray(),
                        totalTablas = tablas.Count
                    },
                    estado = puedeConectar && totalUsuarios > 0 ? "✅ Base de datos operativa" : "⚠️ Requiere inicialización"
                });
            }
            catch (Exception ex)
            {
                return Ok(new
                {
                    diagnostico,
                    error = new
                    {
                        tipo = ex.GetType().Name,
                        mensaje = ex.Message,
                        stackTrace = ex.StackTrace?.Split('\n').Take(5).ToArray(),
                        excepcionInterna = ex.InnerException?.Message
                    },
                    estado = "❌ Error de conexión"
                });
            }
        }

        public class CrearUsuarioRequest
        {
            public string Nombre { get; set; } = "";
            public string Apellido { get; set; } = "";
            public string Email { get; set; } = "";
            public string Password { get; set; } = "";
            public DateTime? FechaNacimiento { get; set; }
            public string? Genero { get; set; }
            public int? Altura { get; set; }
            public decimal? Peso { get; set; }
            public string? Rol { get; set; }
            public string? Departamento { get; set; }
            public string? Cargo { get; set; }
            public int? JefeId { get; set; }
        }
    }

    public class LoginModel
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }
} 