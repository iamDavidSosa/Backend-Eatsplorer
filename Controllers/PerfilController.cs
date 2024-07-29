using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PROYECTO_PRUEBA.Custom;
using PROYECTO_PRUEBA.Models;
using PROYECTO_PRUEBA.Models.DTOs;
using Microsoft.AspNetCore.Authorization;
using PROYECTO_PRUEBA.Context;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.Security.Claims;

namespace PROYECTO_PRUEBA.Controllers
{
    [Route("api/[controller]")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [ApiController]
    public class PerfilController : ControllerBase
    {
        private readonly AppDbContext _context;
        public PerfilController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/Perfil
        [HttpGet("perfil")]
        public async Task<IActionResult> GetPerfil()
        {
            // Obtener el ID del usuario autenticado
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
            {
                return Unauthorized(new { isSuccess = false, mesasge = "Usuario no autenticado." });
            }

            int idUsuario = int.Parse(userIdClaim.Value);

            var usuario = await _context.Usuarios.FindAsync(idUsuario);

            var gustos = await _context.Detalles_Usuario
                        .Where(du => du.id_usuario == idUsuario && du.tipo == 1)
                        .Join(_context.Ingredientes, 
                         du => du.id_ingrediente, 
                         i => i.id_ingrediente, 
                        (du, i) => new { du.id_ingrediente, i.nombre })
                         .ToListAsync();

            var noConsume = await _context.Detalles_Usuario
                        .Where(du => du.id_usuario == idUsuario && du.tipo == 2)
                        .Join(_context.Ingredientes, 
                         du => du.id_ingrediente, 
                         i => i.id_ingrediente, 
                         (du, i) => new { du.id_ingrediente, i.nombre })
                        .ToListAsync();

            var alergico = await _context.Detalles_Usuario
                        .Where(du => du.id_usuario == idUsuario && du.tipo == 3)
                        .Join(_context.Ingredientes, 
                         du => du.id_ingrediente, 
                         i => i.id_ingrediente, 
                         (du, i) => new { du.id_ingrediente, i.nombre })
                         .ToListAsync();

            if (usuario == null)
            {
                return NotFound(new { isSuccess = false, mesasge = "Usuario no encontrado" });
            }

            var usuarioEditar = new
            {
                usuario = usuario.usuario,
                correo = usuario.correo,
                descripcion = usuario.descripcion,
                url_foto_perfil = usuario.url_foto_perfil,
                url_foto_portada = usuario.url_foto_portada,   
                gustos = gustos,
                noConsume = noConsume,
                alergico = alergico
            };

            return Ok(new { isSuccess = true, usuarioEditar });
        }

        // GET: api/Perfil/1
        [HttpGet("{id}")]
        public async Task<IActionResult> GetPerfil(int id)
        {
            var Usuario = await _context.Usuarios.FindAsync(id);

            var gustos = await _context.Detalles_Usuario
                        .Where(du => du.id_usuario == id && du.tipo == 1)
                        .Join(_context.Ingredientes,
                         du => du.id_ingrediente,
                         i => i.id_ingrediente,
                        (du, i) => new { du.id_ingrediente, i.nombre })
                         .ToListAsync();

            var noConsume = await _context.Detalles_Usuario
                        .Where(du => du.id_usuario == id && du.tipo == 2)
                        .Join(_context.Ingredientes,
                         du => du.id_ingrediente,
                         i => i.id_ingrediente,
                         (du, i) => new { du.id_ingrediente, i.nombre })
                        .ToListAsync();

            var alergico = await _context.Detalles_Usuario
                        .Where(du => du.id_usuario == id && du.tipo == 3)
                        .Join(_context.Ingredientes,
                         du => du.id_ingrediente,
                         i => i.id_ingrediente,
                         (du, i) => new { du.id_ingrediente, i.nombre })
                         .ToListAsync();
            if (Usuario == null)
            {
                return NotFound(new { isSuccess = false, mesasge = "Usuario no encontrado" });
            }

            var usuario = new
            {
                usuario = Usuario.usuario,
                correo = Usuario.correo,
                descripcion = Usuario.descripcion,
                url_foto_perfil = Usuario.url_foto_perfil,
                url_foto_portada = Usuario.url_foto_portada,
                gustos = gustos,
                noConsume = noConsume,
                alergico = alergico
            };
            return Ok(new { isSuccess = true, usuario });
        }
        // PUT: api/Perfil/actualizarPerfil
        [HttpPut("actualizarPerfil")]
        public async Task<IActionResult> ActualizarPerfil([FromBody] ActualizacionUsuarioDTO actualizacionUsuarioDTO )
        {
            // Validaciones de los datos del cuerpo de la solicitud
            if (actualizacionUsuarioDTO == null)
            {
                return BadRequest(new { isSuccess = false, message = "Datos de la solicitud no proporcionados." });
            }
            if (string.IsNullOrEmpty(actualizacionUsuarioDTO.Usuario))
            {
                return BadRequest(new { isSuccess = false, message = "El campo 'usuario' es obligatorio." });
            }
            if (string.IsNullOrEmpty(actualizacionUsuarioDTO.Correo))
            {
                return BadRequest(new { isSuccess = false, message = "El campo 'correo' es obligatorio." });
            }

            // Inicializar listas vacías si son nulas
            actualizacionUsuarioDTO.Gustos = actualizacionUsuarioDTO.Gustos ?? new List<int>();
            actualizacionUsuarioDTO.NoConsume = actualizacionUsuarioDTO.NoConsume ?? new List<int>();
            actualizacionUsuarioDTO.Alergico = actualizacionUsuarioDTO.Alergico ?? new List<int>();

            // Obtener el ID del usuario autenticado
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
            {
                return Unauthorized(new { isSuccess = false, message = "Usuario no autenticado." });
            }

            int idUsuario = int.Parse(userIdClaim.Value);

            // Consulta para obtener el usuario por su ID
            var usuario = await _context.Usuarios.FindAsync(idUsuario);
            if (usuario == null)
            {
                return NotFound(new { isSuccess = false, message = "Usuario no encontrado." });
            }

            // Actualizar los datos del usuario
            usuario.usuario = actualizacionUsuarioDTO.Usuario;
            usuario.correo = actualizacionUsuarioDTO.Correo;
            usuario.descripcion = actualizacionUsuarioDTO.Descripcion;
            usuario.url_foto_perfil = actualizacionUsuarioDTO.UrlFotoPerfil;
            usuario.url_foto_portada = actualizacionUsuarioDTO.UrlFotoPortada;

            // Eliminar los registros existentes en la tabla Detalles_Usuario para este usuario
            var detallesUsuarioExistentes = _context.Detalles_Usuario.Where(du => du.id_usuario == idUsuario);
            _context.Detalles_Usuario.RemoveRange(detallesUsuarioExistentes);

            // Insertar nuevos registros en la tabla Detalles_Usuario
            var detallesUsuarioNuevos = new List<Detalles_Usuario>();

            foreach (var gusto in actualizacionUsuarioDTO.Gustos)
            {
                detallesUsuarioNuevos.Add(new Detalles_Usuario { id_usuario = idUsuario, id_ingrediente = gusto, tipo = 1 });
            }
            foreach (var noConsume in actualizacionUsuarioDTO.NoConsume)
            {
                detallesUsuarioNuevos.Add(new Detalles_Usuario { id_usuario = idUsuario, id_ingrediente = noConsume, tipo = 2 });
            }
            foreach (var alergico in actualizacionUsuarioDTO.Alergico)
            {
                detallesUsuarioNuevos.Add(new Detalles_Usuario { id_usuario = idUsuario, id_ingrediente = alergico, tipo = 3 });
            }

            _context.Detalles_Usuario.AddRange(detallesUsuarioNuevos);

            // Guardar los cambios en la base de datos
            await _context.SaveChangesAsync();

            return Ok(new { isSuccess = true, message = "Perfil actualizado con éxito." });
        }



    }
}
