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
    public class RecipeController : ControllerBase
    {
        private readonly AppDbContext _context;

        public RecipeController(AppDbContext context, Utilidades utilidades)
        {
            _context = context;
        }

        [HttpPost("Ingresar")]
        public async Task<IActionResult> Ingresar([FromBody] Recetas receta)
        {
            try
            {
                // Establecer la fecha de creación a la fecha actual
                receta.fecha_creacion = DateTime.Now;

                // Agregamos la receta a la base de datos
                _context.Recetas.Add(receta);
                await _context.SaveChangesAsync();

                // Devolver el ID de la receta recién guardada
                return Ok(new { message = "Receta ingresada con éxito.", id_receta = receta.id_receta });

            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Error al ingresar la receta: {ex.Message}");
            }
        }

        [HttpGet("Listar")]
        public async Task<ActionResult<IEnumerable<Recetas>>> Listar()
        {
            try
            {
                var recetas = await _context.Recetas.ToListAsync();
                return Ok(recetas);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Error al obtener las recetas: {ex.Message}");
            }


        }
    }
}