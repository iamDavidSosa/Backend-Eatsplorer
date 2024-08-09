using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PROYECTO_PRUEBA.Context;
using PROYECTO_PRUEBA.Custom;
using PROYECTO_PRUEBA.Models;
using PROYECTO_PRUEBA.Models.DTOs;

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
                    return Ok();
                }

                // Agregamos el ingrediente a la base de datos
                _context.Ingredientes.Add(ingrediente);
                await _context.SaveChangesAsync();

                return Ok(new { message = "Ingrediente ingresado con éxito.", id_ingrediente = ingrediente.id_ingrediente });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Error al ingresar el ingrediente: {ex.Message}");
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
        [HttpPost("BuscarPorNombre")]
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


    }
}
