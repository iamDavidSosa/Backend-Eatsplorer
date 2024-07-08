using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PROYECTO_PRUEBA.Custom;
using PROYECTO_PRUEBA.Models;
using PROYECTO_PRUEBA.Models.DTOs;
using Microsoft.AspNetCore.Authorization;
using PROYECTO_PRUEBA.Context;

namespace PROYECTO_PRUEBA.Controllers
{
    [Route("api/[controller]")]
    [AllowAnonymous]
    [ApiController]
    public class AccesoController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly Utilidades _utilidades;

        public AccesoController(AppDbContext context, Utilidades utilidades)
        {
            _context = context;
            _utilidades = utilidades;
        }

        // POST: api/Acceso/register
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] UsuarioDTO usuarioDTO)
        {
            if (_context.Usuarios.Any(u => u.correo == usuarioDTO.correo))
            {
                return BadRequest(new { isSuccess = false });
            }

            var usuario = new Usuario
            {
                correo = usuarioDTO.correo,
                usuario = usuarioDTO.usuario,
                clave = _utilidades.EncriptarSHA256(usuarioDTO.clave),
                id_rol = 2,
                fecha_creacion = DateTime.Now
            };

            await _context.Usuarios.AddAsync(usuario);
            await _context.SaveChangesAsync();

            if(usuario.id_usuario != 0)
            {
                return await LoginAfterRegistration(usuarioDTO.correo, usuarioDTO.clave);
            }
            else
            {
                return BadRequest(new {isSuccess = false});
            }
        }

        private async Task<IActionResult> LoginAfterRegistration(string correo, string clave)
        {
            var usuarioEncontrado = await _context.Usuarios
                .Where(u => u.correo == correo && u.clave == _utilidades.EncriptarSHA256(clave))
                .FirstOrDefaultAsync();

            if (usuarioEncontrado == null)
            {
                return Unauthorized(new { isSuccess = false, token = "", message = "Usuario no autorizado." });
            }
            else
            {
                // Generar token y devolverlo junto con el éxito
                return Ok(new { isSuccess = true, token = _utilidades.GenerarToken(usuarioEncontrado), id_usuario = usuarioEncontrado.id_usuario, usuario = usuarioEncontrado.usuario, correo = usuarioEncontrado.correo });
            }
        }

        // POST: api/Acceso/Login
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDTO loginDIO)
        {
            var usuarioEncontrado = await _context.Usuarios
                .Where(u => u.correo == loginDIO.correo && u.clave == _utilidades.EncriptarSHA256(loginDIO.clave)).FirstOrDefaultAsync();

            if (usuarioEncontrado == null) { return Unauthorized(new { isSuccess = false, token = "" }); }
            else return Ok(new { isSuccess = true, token = _utilidades.GenerarToken(usuarioEncontrado), id_usuario = usuarioEncontrado.id_usuario, usuario = usuarioEncontrado.usuario, correo = usuarioEncontrado.correo });

        }

        // POST: api/Acceso/LoginDirecto
        [HttpPost("loginDirecto")]
        public async Task<IActionResult> LoginDirecto([FromBody] LoginDirectoDTO loginDirectoDTO)
        {
            var usuarioEncontrado = await _context.Usuarios
                .Where(u => u.correo == loginDirectoDTO.correo).FirstOrDefaultAsync();

            if (usuarioEncontrado == null) { return Unauthorized(new { isSuccess = false, token = "" }); }
            else return Ok(new { isSuccess = true, token = _utilidades.GenerarToken(usuarioEncontrado), id_usuario = usuarioEncontrado.id_usuario, usuario = usuarioEncontrado.usuario, correo = usuarioEncontrado.correo });
        }
    }
}
