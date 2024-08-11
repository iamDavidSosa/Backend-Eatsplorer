using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PROYECTO_PRUEBA.Context;
using PROYECTO_PRUEBA.Custom;
using PROYECTO_PRUEBA.Models;
using Microsoft.EntityFrameworkCore;
using PROYECTO_PRUEBA.Models.DTOs;
using System.Globalization;
using System.Text;
using System.Security.Claims;

namespace PROYECTO_PRUEBA.Controllers
{

    [Route("api/[controller]")]
    [Authorize]
    [ApiController]
    public class RecipeController : ControllerBase
    {
        private readonly AppDbContext _context;

        public RecipeController(AppDbContext context)
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

        [HttpPost("BuscarPorIngrediente")]
        public async Task<IActionResult> BuscarRecetas([FromBody] IngredientesRequest request)
        {
            try
            {
                // Obtiene los IDs de los ingredientes basados en los nombres proporcionados
                var ingredientes = await _context.Ingredientes
                    .Where(i => request.NombresIngredientes.Contains(i.nombre))
                    .Select(i => i.id_ingrediente)
                    .ToListAsync();

                // Consulta para obtener los IDs de las recetas que contienen todos los ingredientes especificados
                var recetasIds = await _context.Recetas_Ingredientes
                    .Where(ri => ingredientes.Contains(ri.id_ingrediente))
                    .GroupBy(ri => ri.id_receta)
                    .Where(g => g.Count() == ingredientes.Count)
                    .Select(g => g.Key)
                    .ToListAsync();

                // Consulta para obtener las recetas basadas en los IDs obtenidos
                var recetas = await _context.Recetas
                    .Where(r => recetasIds.Contains(r.id_receta))
                    .ToListAsync();

                return Ok(recetas);
            }
            catch (Exception ex)
            {
                // Manejo de la excepción y retorno de un mensaje de error
                return StatusCode(500, new { mensaje = "Ocurrió un error al buscar las recetas.", detalle = ex.Message });
            }
        }


        [HttpPost("BuscarSinIngrediente")]
        public async Task<IActionResult> BuscarRecetasSinIngredientes([FromBody] IngredientesRequest request)
        {
            try
            {
                // Obtiene los IDs de los ingredientes basados en los nombres proporcionados
                var ingredientesIds = await _context.Ingredientes
                    .Where(i => request.NombresIngredientes.Contains(i.nombre))
                    .Select(i => i.id_ingrediente)
                    .ToListAsync();

                // Consulta para obtener las recetas que NO contienen ninguno de los ingredientes especificados
                var recetasSinIngredientes = await _context.Recetas
                    .Where(r => !_context.Recetas_Ingredientes
                        .Where(ri => ingredientesIds.Contains(ri.id_ingrediente))
                        .Select(ri => ri.id_receta)
                        .Contains(r.id_receta))
                    .ToListAsync();

                return Ok(recetasSinIngredientes);
            }
            catch (Exception ex)
            {
                // Manejo de la excepción y retorno de un mensaje de error
                return StatusCode(500, new { mensaje = "Ocurrió un error al buscar las recetas.", detalle = ex.Message });
            }
        }


        [HttpPost("BuscarPorTag")]
        public async Task<IActionResult> BuscarRecetasPorTag([FromBody] TagsDTO request)
        {
            try
            {
                // Obtiene los IDs de los tags basados en los nombres proporcionados
                var tags = await _context.Tags
                    .Where(t => request.NombresTags.Contains(t.nombre))
                    .Select(t => t.id_tag)
                    .ToListAsync();

                // Consulta para obtener los IDs de las recetas que contienen todos los tags especificados
                var recetasIds = await _context.RecetasTags
                    .Where(rt => tags.Contains(rt.id_tag))
                    .GroupBy(rt => rt.id_receta)
                    .Where(g => g.Count() == tags.Count)
                    .Select(g => g.Key)
                    .ToListAsync();

                // Consulta para obtener las recetas basadas en los IDs obtenidos
                var recetas = await _context.Recetas
                    .Where(r => recetasIds.Contains(r.id_receta))
                    .ToListAsync();

                return Ok(recetas);
            }
            catch (Exception ex)
            {
                // Manejo de la excepción y retorno de un mensaje de error
                return StatusCode(500, new { mensaje = "Ocurrió un error al buscar las recetas.", detalle = ex.Message });
            }
        }



        [HttpPost("Buscar")]
        public async Task<ActionResult<IEnumerable<Recetas>>> Buscar([FromBody] RecetasDTO request)
        {
            if (request == null || string.IsNullOrEmpty(request.titulo))
            {
                return BadRequest("El parámetro de búsqueda no puede estar vacío.");
            }

            // Obtén todas las recetas
            var recetas = await _context.Recetas.ToListAsync();

            // Normaliza la búsqueda para ignorar mayúsculas, minúsculas y acentos
            var normalizedQuery = request.titulo.Normalize(NormalizationForm.FormD);
            var accentsRemovedQuery = new string(normalizedQuery
                .Where(c => CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark)
                .ToArray());

            // Filtra las recetas en memoria
            var resultados = recetas
                .Where(r => RemoveAccents(r.titulo).ToLower().Contains(accentsRemovedQuery.ToLower()))
                .ToList();

            return Ok(resultados);
        }

        private string RemoveAccents(string text)
        {
            return text.Normalize(NormalizationForm.FormD)
                .Where(c => CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark)
                .Aggregate(string.Empty, (current, c) => current + c);
        }

        [HttpGet("recetasSugeridas")]
        public async Task<IActionResult> ObtenerRecetasSugeridas()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
            {
                return Unauthorized(new { isSuccess = false, message = "Usuario no autenticado." });
            }

            int idUsuario = int.Parse(userIdClaim.Value);

            var ingredientesGusta = await _context.Detalles_Usuario
                .Where(du => du.id_usuario == idUsuario && du.tipo == 1)
                .Select(du => du.id_ingrediente)
                .ToListAsync();

            var ingredientesNoConsume = await _context.Detalles_Usuario
                .Where(du => du.id_usuario == idUsuario && du.tipo == 2)
                .Select(du => du.id_ingrediente)
                .ToListAsync();

            var ingredientesAlergico = await _context.Detalles_Usuario
                .Where(du => du.id_usuario == idUsuario && du.tipo == 3)
                .Select(du => du.id_ingrediente)
                .ToListAsync();

            var recetas = await _context.Recetas.ToListAsync();

            var recetasFiltradas = recetas.Where(r =>
                !_context.Recetas_Ingredientes
                .Where(ri => ri.id_receta == r.id_receta)
                .Any(ri => ingredientesAlergico.Contains(ri.id_ingrediente))
            ).ToList();

            var recetasSugeridas = recetasFiltradas.Where(r =>
            {
                var ingredientesReceta = _context.Recetas_Ingredientes
                    .Where(ri => ri.id_receta == r.id_receta)
                    .Select(ri => ri.id_ingrediente)
                    .ToList();

                var contieneGustos = ingredientesGusta.Any(i => ingredientesReceta.Contains(i));
                var contieneNoConsume = ingredientesNoConsume.Any(i => ingredientesReceta.Contains(i));
                var noSoloNoConsume = !contieneNoConsume || ingredientesReceta.Count(i => ingredientesNoConsume.Contains(i)) < ingredientesReceta.Count;

                return contieneGustos && noSoloNoConsume;
            }).ToList();

            return Ok(new { isSuccess = true, recetasSugeridas });
        }

        [HttpGet("recetasDespensa")]
        public async Task<IActionResult> ObtenerRecetasConDespensa()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
            {
                return Unauthorized(new { isSuccess = false, message = "Usuario no autenticado." });
            }

            int idUsuario = int.Parse(userIdClaim.Value);

            var recetas = await _context.Recetas.ToListAsync();

            var recetasConDespensa = recetas.Where(r =>
            {
                var ingredientesReceta = _context.Recetas_Ingredientes
                    .Where(ri => ri.id_receta == r.id_receta)
                    .ToList();

                bool tieneTodosLosIngredientes = ingredientesReceta.All(ir =>
                {
                    var ingredienteDespensa = _context.Despensa
                        .FirstOrDefault(d => d.id_usuario == idUsuario && d.id_ingrediente == ir.id_ingrediente);

                    return ingredienteDespensa != null;//&& ingredienteDespensa.cantidad >= ir.cantidad;
                });

                return tieneTodosLosIngredientes;
            }).ToList();

            return Ok(new { isSuccess = true, recetasConDespensa });
        }


    }
}