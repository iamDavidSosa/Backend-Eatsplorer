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
    public class MedidaController : ControllerBase
    {
        private readonly AppDbContext _context;

        public MedidaController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost("Ingresar")]
        public async Task<IActionResult> Ingresar([FromBody] Medida medida)
        {
            try
            {
                // Verificar e ya existe (case-insensitive)
                var existingMedida = await _context.Medida
                    .FirstOrDefaultAsync(i => EF.Functions.Collate(i.medida, "SQL_Latin1_General_CP1_CI_AS") == medida.medida);

                if (existingMedida != null)
                {
                    return Ok();
                }

                // Agregamos el ingrediente a la base de datos
                _context.Medida.Add(medida);
                await _context.SaveChangesAsync();

                return Ok(new { message = "Medida ingresada con éxito." });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Error al ingresar la medida: {ex.Message}");
            }

        }

        [HttpGet("Listar")]
        public async Task<ActionResult<IEnumerable<Medida>>> Listar()
        {
            try
            {
                var medida = await _context.Medida.ToListAsync();
                return Ok(medida);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Error al obtener las medidas: {ex.Message}");
            }
        }
    }
}
