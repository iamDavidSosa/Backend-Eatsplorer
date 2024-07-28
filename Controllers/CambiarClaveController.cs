using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using PROYECTO_PRUEBA.Context;
using PROYECTO_PRUEBA.Custom;
using PROYECTO_PRUEBA.Models;
using PROYECTO_PRUEBA.Models.DTOs;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace PROYECTO_PRUEBA.Controllers
{
    /* [Route("api/[controller]")]
     [ApiController]
     public class CambiarClaveController : ControllerBase
     {
         private readonly AppDbContext _context;
         private readonly Utilidades _utilidades;

         public CambiarClaveController(AppDbContext context, Utilidades utilidades)
         {
             _context = context;
             _utilidades = utilidades;
         }

         // CODIGO PARA CAMBIAR CLAVE
         [HttpPost("CambiarClave")]
         public async Task<IActionResult> CambiarClave(int idUsuario, string claveActual, string nuevaClave)
         {
             // Consulta para obtener el usuario por su ID
             var usuario = await _context.Usuarios.FindAsync(idUsuario);

             // Verifica si el usuario existe
             if (usuario == null)
             {
                 return NotFound("Usuario no encontrado.");
             }

             // Verifica si la clave actual proporcionada coincide con la clave almacenada (encriptada)
             if (usuario.clave != _utilidades.EncriptarSHA256(claveActual))
             {
                 return Unauthorized("La clave actual es incorrecta.");
             }

             // Actualiza la clave del usuario con la nueva clave (encriptada)
             usuario.clave = _utilidades.EncriptarSHA256(nuevaClave);

             // Guarda los cambios en la base de datos
             await _context.SaveChangesAsync();

             // Devuelve un mensaje de éxito
             return Ok("La contraseña ha sido cambiada con éxito.");
         }
     }*/

    [Route("api/[controller]")]
    [Authorize]
    [ApiController]
    public class CambiarClaveController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly Utilidades _utilidades;

        public CambiarClaveController(AppDbContext context, Utilidades utilidades, IConfiguration configuration)
        {
            _context = context;
            _utilidades = utilidades;
        }
        // POST: api/CambiarClave/cambiarClave
        [HttpPost("cambiarClave")]
        public async Task<IActionResult> CambiarClave([FromBody] CambiarClaveDTO cambiarClaveDTO)
        {
            // Obtener el ID del usuario autenticado
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
            {
                return Unauthorized(new { isSuccess = false, mesasge = "Usuario no autenticado." });
            }

            int idUsuario = int.Parse(userIdClaim.Value);

            // Consulta para obtener el usuario por su ID
            var usuario = await _context.Usuarios.FindAsync(idUsuario);

            // Verifica si el usuario existe
            if (usuario == null)
            {
                return NotFound(new { isSuccess = false, mesasge = "Usuario no encontrado." });
            }

            // Verifica si la clave actual proporcionada coincide con la clave almacenada (encriptada)
            if (usuario.clave != _utilidades.EncriptarSHA256(cambiarClaveDTO.ClaveActual))
            {
                return BadRequest(new { isSuccess = false, mesasge = "La clave actual es incorrecta." });
            }

            // Actualiza la clave del usuario con la nueva clave (encriptada)
            usuario.clave = _utilidades.EncriptarSHA256(cambiarClaveDTO.NuevaClave);

            // Guarda los cambios en la base de datos
            await _context.SaveChangesAsync();

            // Devuelve un mensaje de éxito
            return Ok(new { isSuccess = true, message = "La contraseña ha sido cambiada con éxito." });
        }


    }


}
