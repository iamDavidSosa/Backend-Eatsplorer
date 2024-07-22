using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PROYECTO_PRUEBA.Context;
using PROYECTO_PRUEBA.Custom;
using PROYECTO_PRUEBA.Models;

namespace PROYECTO_PRUEBA.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class Recetas_GuardadasController : ControllerBase
    {
        private readonly AppDbContext _context;

        public Recetas_GuardadasController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost("Ingresar")]
        public async Task<IActionResult> Ingresar([FromBody] Recetas_Guardadas receta_guardada)
        {
            if (receta_guardada == null)
            {
                return BadRequest("Datos de la receta guardada no válidos.");
            }

            // Verificar que la receta y el usuario existan antes de agregar la receta guardada
            var recetaExiste = await _context.Recetas.AnyAsync(r => r.id_receta == receta_guardada.id_receta);
            var usuarioExiste = await _context.Usuarios.AnyAsync(u => u.id_usuario == receta_guardada.id_usuario);

            if (!recetaExiste)
            {
                return NotFound("La receta especificada no existe.");
            }

            if (!usuarioExiste)
            {
                return NotFound("El usuario especificado no existe.");
            }

            try
            {
                // Establecer la fecha de creación a la fecha actual
                receta_guardada.fecha_acceso = DateTime.Now;

                // Agregamos la receta guardada a la base de datos
                _context.Recetas_Guardadas.Add(receta_guardada);
                await _context.SaveChangesAsync();

                // Devolver el ID de la receta recién guardada
                return Ok(new { message = "Ingreso exitoso" });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Error al ingresar la receta: {ex.Message}");
            }
        }


        [HttpGet("Listar")]
        public async Task<ActionResult<IEnumerable<Recetas_Guardadas>>> Listar()
        {
            try
            {
                var recetas_guardadas = await _context.Recetas_Guardadas.ToListAsync();
                return Ok(recetas_guardadas);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Error: {ex.Message}");
            }


        }

    }
}
