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
    public class TagsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public TagsController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost("Ingresar")]
        public async Task<IActionResult> Ingresar([FromBody] Tags etiqueta)
        {
            try
            {

                // Agregamos la tag a la base de datos
                _context.Tags.Add(etiqueta);
                await _context.SaveChangesAsync();

                return Ok(new { message = "Tag ingresada con éxito." });

            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Error al ingresar la tag: {ex.Message}");
            }
        }

        [HttpGet("Listar")]
        public async Task<ActionResult<IEnumerable<Tags>>> Listar()
        {
            try
            {
                var etiqueta = await _context.Tags.ToListAsync();
                return Ok(etiqueta);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Error al obtener las etiquetas: {ex.Message}");
            }
        }

        [HttpPost("AsignarCategoria")]
        public async Task<IActionResult> AsignarCategoria([FromBody] TagsCategorias tagsCategorias)
        {
            try
            {
                var tag = await _context.Tags.FindAsync(tagsCategorias.id_tag);
                var categoria = await _context.Categoria.FindAsync(tagsCategorias.id_categoria);

                if (tag == null || categoria == null)
                {
                    return BadRequest("Tag o Categoría no encontrada.");
                }

                _context.TagsCategorias.Add(tagsCategorias);
                await _context.SaveChangesAsync();

                return Ok(new { message = "Categoría asignada a la tag con éxito." });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Error al asignar la categoría: {ex.Message}");
            }
        }
    }
}