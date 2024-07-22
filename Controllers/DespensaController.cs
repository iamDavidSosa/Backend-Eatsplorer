using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PROYECTO_PRUEBA.Context;
using PROYECTO_PRUEBA.Models;

namespace PROYECTO_PRUEBA.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class DespensaController : ControllerBase
    {
        private readonly AppDbContext _context;

        public DespensaController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost("Ingresar")]
        public async Task<IActionResult> Ingresar([FromBody] Despensa despensa)
        {
            try
            {
                // Verificar si ya existe (case-insensitive)
                /*var existingDespensa = await _context.Despensa
                    .FirstOrDefaultAsync(i => EF.Functions.Collate(i.nombre, "SQL_Latin1_General_CP1_CI_AS") == ingrediente.nombre);

                if (existingDespensa != null)
                {
                    return Ok();
                }*/

                // Agregamos a la base de datos
                _context.Despensa.Add(despensa);
                await _context.SaveChangesAsync();

                return Ok(new { message = "Ingreso éxitoso." });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Error al ingresar: {ex.Message}");
            }

        }
    }
}
