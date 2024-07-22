using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PROYECTO_PRUEBA.Custom;
using PROYECTO_PRUEBA.Models;
using PROYECTO_PRUEBA.Models.DTOs;
using Microsoft.AspNetCore.Authorization;
using PROYECTO_PRUEBA.Context;
using Microsoft.AspNetCore.Authentication.JwtBearer;

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

        // POST: api/Perfil
        [HttpPost("perfil")]
        public async Task<IActionResult> GetPerfil([FromBody] PerfilDTO perfilDTO)
        {
            var usuario = await _context.Usuarios.FindAsync(perfilDTO.id_usuario);

            if (usuario == null)
            {
                return NotFound("Usuario no encontrado");
            }

            return Ok(usuario);
        }

        // GET: api/Perfil/1
        [HttpGet("{id}")]
        public async Task<IActionResult> GetPerfil(int id)
        {
            var usuario = await _context.Usuarios.FindAsync(id);

            if (usuario == null)
            {
                return NotFound("Usuario no encontrado");
            }

            return Ok(usuario);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUsuario(int id, [FromBody] UsuarioUpdateDTO usuarioUpdateDTO)
        {
            // Buscar el usuario en la base de datos
            var usuario = await _context.Usuarios.FindAsync(id);

            if (usuario == null)
            {
                return NotFound("Usuario no encontrado");
            }

            // Actualizar los campos del usuario con los datos del DTO
            if (usuarioUpdateDTO.Correo != null)
            {
                usuario.correo = usuarioUpdateDTO.Correo;
            }
            if (usuarioUpdateDTO.Usuario != null)
            {
                usuario.usuario = usuarioUpdateDTO.Usuario;
            }
            if (usuarioUpdateDTO.Clave != null)
            {
                usuario.clave = usuarioUpdateDTO.Clave;
            }
            if (usuarioUpdateDTO.UrlFotoPerfil != null)
            {
                usuario.url_foto_perfil = usuarioUpdateDTO.UrlFotoPerfil;
            }
            if (usuarioUpdateDTO.Descripcion != null)
            {
                usuario.descripcion = usuarioUpdateDTO.Descripcion;
            }
            if (usuarioUpdateDTO.UrlFotoPortada != null)
            {
                usuario.url_foto_portada = usuarioUpdateDTO.UrlFotoPortada;
            }
            if (usuarioUpdateDTO.CantRecetasGuardadas.HasValue)
            {
                usuario.cant_recetas_guardadas = usuarioUpdateDTO.CantRecetasGuardadas.Value;
            }
            if (usuarioUpdateDTO.GoogleId != null)
            {
                usuario.google_id = usuarioUpdateDTO.GoogleId;
            }
            if (usuarioUpdateDTO.GithubId != null)
            {
                usuario.github_id = usuarioUpdateDTO.GithubId;
            }

            // Guardar los cambios en la base de datos
            _context.Usuarios.Update(usuario);
            await _context.SaveChangesAsync();

            return Ok(usuario);
        }


    }
}
