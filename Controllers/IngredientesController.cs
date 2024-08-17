using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PROYECTO_PRUEBA.Context;
using PROYECTO_PRUEBA.Custom;
using PROYECTO_PRUEBA.Models;
using PROYECTO_PRUEBA.Models.DTOs;
using System.Globalization;
using System.Text;

namespace PROYECTO_PRUEBA.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class IngredientesController : ControllerBase
    {
        private readonly AppDbContext _context;

        public IngredientesController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost("Ingresar")]
        public async Task<IActionResult> Ingresar([FromBody] Ingredientes ingrediente)
        {
            try
            {
                // Verificar si el ingrediente ya existe (case-insensitive)
                var existingIngrediente = await _context.Ingredientes
                    .FirstOrDefaultAsync(i => EF.Functions.Collate(i.nombre, "SQL_Latin1_General_CP1_CI_AS") == ingrediente.nombre);

                if (existingIngrediente != null)
                {
                    // Si el ingrediente ya existe, devolver los datos del ingrediente existente
                    return Ok(new
                    {
                        isSuccess = false,
                        message = "El ingrediente ya existe.",
                        ingrediente = existingIngrediente
                    });
                }

                // Agregar el ingrediente a la base de datos
                _context.Ingredientes.Add(ingrediente);
                await _context.SaveChangesAsync();

                // Devolver el ID del ingrediente recién guardado
                return Ok(new
                {
                    isSuccess = true,
                    message = "Ingrediente ingresado con éxito.",
                    id_ingrediente = ingrediente.id_ingrediente
                });
            }
            catch (Exception ex)
            {
                // Manejar el error y devolver el mensaje de error
                return StatusCode(StatusCodes.Status500InternalServerError, new
                {
                    isSuccess = false,
                    message = $"Error al ingresar el ingrediente: {ex.Message}"
                });
            }
        }


        [HttpGet("Listar")]
        public async Task<ActionResult<IEnumerable<Ingredientes>>> Listar()
        {
            try
            {
                var ingredientes = await _context.Ingredientes.ToListAsync();
                return Ok(ingredientes);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Error al obtener los ingredientes: {ex.Message}");
            }
        }

        // POST: api/Ingredientes/BuscarPorNombre
        [HttpPost("ListarPorNombre")]
        public async Task<ActionResult<IEnumerable<Ingredientes>>> BuscarPorNombre([FromBody] IngredienteDTO request)
        {
            if (request == null || string.IsNullOrWhiteSpace(request.Nombre))
            {
                return BadRequest("Invalid name.");
            }

            try
            {
                var ingredientes = await _context.Ingredientes
                    .Where(i => EF.Functions.Like(i.nombre, $"%{request.Nombre}%"))
                    .ToListAsync();

                return Ok(ingredientes);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Error al buscar los ingredientes: {ex.Message}");
            }
        }

        [HttpPost("Buscar")]
        public async Task<ActionResult<IEnumerable<Ingredientes>>> Buscar([FromBody] IngredienteDTO request)
        {
            if (request == null || string.IsNullOrEmpty(request.Nombre))
            {
                return BadRequest("El parámetro de búsqueda no puede estar vacío.");
            }

            // Obtén todos los ingredientes
            var ingredientes = await _context.Ingredientes.ToListAsync();

            // Normaliza la búsqueda para ignorar mayúsculas, minúsculas y acentos
            var normalizedQuery = request.Nombre.Normalize(NormalizationForm.FormD);
            var accentsRemovedQuery = new string(normalizedQuery
                .Where(c => CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark)
                .ToArray());

            // Filtra los ingredientes en memoria
            var resultados = ingredientes
                .Where(i => RemoveAccents(i.nombre).ToLower().Contains(accentsRemovedQuery.ToLower()))
                .ToList();

            return Ok(resultados);
        }

        private string RemoveAccents(string text)
        {
            return text.Normalize(NormalizationForm.FormD)
                .Where(c => CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark)
                .Aggregate(string.Empty, (current, c) => current + c);
        }



        [HttpDelete("Eliminar")]
        public async Task<IActionResult> Eliminar([FromBody] IngredienteEliminarDTO request)
        {
            if (request == null || request.id_ingrediente <= 0)
            {
                return BadRequest("Datos inválidos.");
            }

            try
            {
                var ingrediente = await _context.Ingredientes.FindAsync(request.id_ingrediente);

                if (ingrediente == null)
                {
                    return NotFound("Ingrediente no encontrado.");
                }

                _context.Ingredientes.Remove(ingrediente);
                await _context.SaveChangesAsync();

                return Ok("Ingrediente eliminado exitosamente.");
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Error al eliminar el ingrediente: {ex.Message}");
            }
        }

    }
}
