using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PROYECTO_PRUEBA.Context;
using PROYECTO_PRUEBA.Models;
using Microsoft.EntityFrameworkCore;

namespace PROYECTO_PRUEBA.Controllers
{
    [Route("api/[controller]")]
    [Authorize]
    [ApiController]
    public class RecetasIngredientesController : ControllerBase
    {
        private readonly AppDbContext _context;

        public RecetasIngredientesController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost("Ingresar")]
        public async Task<IActionResult> Ingresar([FromBody] Recetas_Ingredientes recetaIngrediente)
        {
            try
            {
                // Verificar si la relación ya existe
                var existingRelacion = await _context.Recetas_Ingredientes
                    .FirstOrDefaultAsync(ri => ri.id_receta == recetaIngrediente.id_receta &&
                                               ri.id_ingrediente == recetaIngrediente.id_ingrediente);

                if (existingRelacion != null)
                {
                    return Ok(); // No realizar ninguna acción si la relación ya existe
                }

                // Agregamos la relación a la base de datos
                _context.Recetas_Ingredientes.Add(recetaIngrediente);
                await _context.SaveChangesAsync();

                return Ok(new { message = "Relación ingresada con éxito." });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Error al ingresar la relación: {ex.Message}");
            }
        }
    }
}