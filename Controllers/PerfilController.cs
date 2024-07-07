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

        // GET: api/Perfil
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
    }
}
