using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PROYECTO_PRUEBA.Context;
using PROYECTO_PRUEBA.Models;
using Microsoft.EntityFrameworkCore;
using PROYECTO_PRUEBA.Models.DTOs;

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

        [HttpDelete("Eliminar")]
        public async Task<IActionResult> EliminarRecetaIngrediente([FromBody] Recetas_IngredientesDTO request)
        {
            if (request == null || request.id_receta <= 0 || request.id_ingrediente <= 0)
            {
                return BadRequest("Datos inválidos.");
            }

            try
            {
                // Encuentra el registro que quieres eliminar
                var recetaIngrediente = await _context.Recetas_Ingredientes
                    .FirstOrDefaultAsync(ri => ri.id_receta == request.id_receta && ri.id_ingrediente == request.id_ingrediente);

                if (recetaIngrediente == null)
                {
                    return NotFound("Registro no encontrado.");
                }

                // Elimina el registro
                _context.Recetas_Ingredientes.Remove(recetaIngrediente);
                await _context.SaveChangesAsync();

                return Ok("Registro eliminado exitosamente.");
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Error al eliminar el registro: {ex.Message}");
            }
        }

    }
}