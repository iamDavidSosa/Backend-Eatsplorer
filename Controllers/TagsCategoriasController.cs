using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PROYECTO_PRUEBA.Context;
using PROYECTO_PRUEBA.Models;
using PROYECTO_PRUEBA.Models.DTOs;

namespace PROYECTO_PRUEBA.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class TagsCategoriasController : ControllerBase
    {
        private readonly AppDbContext _context;
        public TagsCategoriasController(AppDbContext context)
        {
            _context = context;
        }

        // POST: api/RecetasTags/Ingresar
        [HttpPost("Ingresar")]
        public async Task<IActionResult> Ingresar([FromBody] TagsCategorias tagsCategorias)
        {
            if (tagsCategorias == null || tagsCategorias.id_categoria <= 0 || tagsCategorias.id_tag <= 0)
            {
                return BadRequest("Invalid input.");
            }

            try
            {
                // Verificar si la relación ya existe
                var existingRelacion = await _context.TagsCategorias
                    .FirstOrDefaultAsync(rt => rt.id_categoria == tagsCategorias.id_categoria &&
                                               rt.id_tag == tagsCategorias.id_tag);

                if (existingRelacion != null)
                {
                    return Ok(); // No realizar ninguna acción si la relación ya existe
                }

                // Agregamos la relación a la base de datos
                _context.TagsCategorias.Add(tagsCategorias);
                await _context.SaveChangesAsync();

                return Ok(new { message = "Relación ingresada con éxito." });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Error al ingresar la relación: {ex.Message}");
            }
        }


        [HttpPost("tagsByCategoria")]
        public async Task<IActionResult> GetTagsByCategoria([FromBody] CategoriaDTO request)
        {
            if (request == null || string.IsNullOrEmpty(request.NombreCategoria))
            {
                return BadRequest("Invalid category name.");
            }

            var categoria = await _context.Categoria
                .Where(c => c.nombre == request.NombreCategoria)
                .Select(c => c.id_categoria)
                .FirstOrDefaultAsync();

            if (categoria == 0)
            {
                return NotFound("Category not found.");
            }

            var tags = await _context.TagsCategorias
                .Where(tc => tc.id_categoria == categoria)
                .Select(tc => tc.id_tag)
                .Distinct()
                .Join(_context.Tags,
                    tagId => tagId,
                    tag => tag.id_tag,
                    (tagId, tag) => tag)
                .ToListAsync();

            return Ok(tags);
        }

        [HttpDelete("Eliminar")]
        public async Task<IActionResult> EliminarTagCategoria([FromBody] TagsCatgoriasDTO request)
        {
            if (request == null || request.id_tag <= 0 || request.id_categoria <= 0)
            {
                return BadRequest("Datos inválidos.");
            }

            try
            {
                // Encuentra el registro que quieres eliminar
                var tagCategoria = await _context.TagsCategorias
                    .FirstOrDefaultAsync(tc => tc.id_tag == request.id_tag && tc.id_categoria == request.id_categoria);

                if (tagCategoria == null)
                {
                    return NotFound("Registro no encontrado.");
                }

                // Elimina el registro
                _context.TagsCategorias.Remove(tagCategoria);
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
