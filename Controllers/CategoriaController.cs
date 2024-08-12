using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PROYECTO_PRUEBA.Context;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using PROYECTO_PRUEBA.Models;
using PROYECTO_PRUEBA.Models.DTOs;

namespace PROYECTO_PRUEBA.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class CategoriaController : ControllerBase
    {
        private readonly AppDbContext _context;

        public CategoriaController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost("Ingresar")]
        public async Task<IActionResult> Ingresar([FromBody] Categoria categoria)
        {
            try
            {

                // Agregamos a la base de datos
                _context.Categoria.Add(categoria);
                await _context.SaveChangesAsync();

                return Ok(new { message = "Categoria ingresada con éxito."});
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Error al ingresar la categoria: {ex.Message}");
            }

        }


        [HttpGet("Listar")]
        public async Task<ActionResult<IEnumerable<Categoria>>> Listar()
        {
            try
            {
                var categoria = await _context.Categoria.ToListAsync();
                return Ok(categoria);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Error al obtener las categorias: {ex.Message}");
            }
        }


        [HttpDelete("Eliminar")]
        public async Task<IActionResult> Eliminar([FromBody] CategoriaEliminarDTO request)
        {
            if (request == null || request.id_categoria <= 0)
            {
                return BadRequest("Datos inválidos.");
            }

            try
            {
                var categoria = await _context.Categoria.FindAsync(request.id_categoria);

                if (categoria == null)
                {
                    return NotFound("Categoría no encontrada.");
                }

                _context.Categoria.Remove(categoria);
                await _context.SaveChangesAsync();

                return Ok("Categoría eliminada exitosamente.");
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Error al eliminar la categoría: {ex.Message}");
            }
        }

    }
}
