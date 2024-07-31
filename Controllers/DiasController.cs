using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PROYECTO_PRUEBA.Context;
using PROYECTO_PRUEBA.Custom;
using PROYECTO_PRUEBA.Models;
using Microsoft.EntityFrameworkCore;

namespace PROYECTO_PRUEBA.Controllers
{

    [Route("api/[controller]")]
    [Authorize]
    [ApiController]
    public class DiasController : ControllerBase
    {
        private readonly AppDbContext _context;

        public DiasController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost("Ingresar")]
        public async Task<IActionResult> Ingresar([FromBody] Dias day)
        {
            try
            {

                // Agregamos el dia a la base de datos
                _context.Dias.Add(day);
                await _context.SaveChangesAsync();

                return Ok(new { message = "Dias ingresado con éxito." });

            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Error al ingresar el numero de dias: {ex.Message}");
            }
        }

        [HttpGet("Listar")]
        public async Task<ActionResult<IEnumerable<Dias>>> Listar()
        {
            try
            {
                var dias = await _context.Dias.ToListAsync();
                return Ok(dias);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Error al obtener los dias: {ex.Message}");
            }


        }
    }
}