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

                
                return Ok(new { message = "Ingreso exitoso" });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Error al ingresar la receta: {ex.Message}");
            }
        }


        /*  [HttpGet("Listar")]
          public async Task<ActionResult<IEnumerable<Recetas_Guardadas>>> Listar([FromBody] UsuarioIdDTO dto)
          {
              try
              {
                  // Filtra las recetas guardadas por el id_usuario
                  var recetas_guardadas = await _context.Recetas_Guardadas
                      .Where(r => r.id_usuario == dto.id_usuario)
                      .ToListAsync();

                  return Ok(recetas_guardadas);
              }
              catch (Exception ex)
              {
                  return StatusCode(StatusCodes.Status500InternalServerError, $"Error: {ex.Message}");
              }
          }*/


        [HttpPost("Listar")]
        public async Task<ActionResult<IEnumerable<RecetaGuardadasDTO>>> Listar([FromBody] UsuarioIdDTO dto)
        {
            try
            {
                var recetasGuardadas = await _context.Recetas_Guardadas
                    .Where(r => r.id_usuario == dto.id_usuario)
                    .Select(r => r.id_receta)
                    .ToListAsync();

                var recetas = await _context.Recetas
                    .Where(r => recetasGuardadas.Contains(r.id_receta))
                    .ToListAsync();

                var recetasConIngredientes = await _context.Recetas_Ingredientes
                    .Where(ri => recetasGuardadas.Contains(ri.id_receta))
                    .Join(_context.Ingredientes,
                          ri => ri.id_ingrediente,
                          i => i.id_ingrediente,
                          (ri, i) => new { ri.id_receta, i.nombre })
                    .GroupBy(x => x.id_receta)
                    .Select(g => new
                    {
                        RecetaId = g.Key,
                        Ingredientes = g.Select(x => x.nombre).ToList()
                    })
                    .ToListAsync();

                var resultado = recetas.Select(r => new RecetaGuardadasDTO
                {
                    id_receta = r.id_receta,
                    titulo = r.titulo,
                    descripcion = r.descripcion,
                    instrucciones = r.instrucciones,
                    foto_receta = r.foto_receta,
                    porciones = r.porciones,
                    Ingredientes = recetasConIngredientes
                        .FirstOrDefault(rc => rc.RecetaId == r.id_receta)?.Ingredientes ?? new List<string>()
                }).ToList();

                return Ok(resultado);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Error: {ex.Message}");
            }
        }


        /* [HttpGet("ListarPorDias")]
         public async Task<ActionResult<IEnumerable<Recetas_Guardadas>>> ListarPorDias([FromBody] UsuarioIdDTO dto)
         {
             try
             {
                 // Obtener el número de días desde la base de datos
                 var diasConfig = await _context.Dias.FirstOrDefaultAsync(); // Obtiene el primer registro de la tabla

                 if (diasConfig == null)
                 {
                     return NotFound("No se encontró la configuración de días.");
                 }

                 // Calcular la fecha de hoy y la fecha correspondiente al número de días antes de hoy
                 DateTime fechaFin = DateTime.Now;
                 DateTime fechaInicio = fechaFin.AddDays(-diasConfig.dias);

                 // Filtrar las recetas guardadas por el intervalo de tiempo calculado y el ID del usuario
                 var recetas_guardadas = await _context.Recetas_Guardadas
                     .Where(r => r.id_usuario == dto.id_usuario && r.fecha_acceso >= fechaInicio && r.fecha_acceso <= fechaFin)
                     .ToListAsync();

                 return Ok(recetas_guardadas);
             }
             catch (Exception ex)
             {
                 // Retornar un error 500 si ocurre una excepción
                 return StatusCode(StatusCodes.Status500InternalServerError, $"Error: {ex.Message}");
             }
         }*/


        [HttpPost("ListarPorDias")]
        public async Task<ActionResult<IEnumerable<RecetaGuardadasDTO>>> ListarPorDias([FromBody] UsuarioIdDTO dto)
        {
            try
            {
                // Obtener el número de días desde la base de datos
                var diasConfig = await _context.Dias.FirstOrDefaultAsync();

                if (diasConfig == null)
                {
                    return NotFound("No se encontró la configuración de días.");
                }

                // Calcular la fecha de hoy y la fecha correspondiente al número de días antes de hoy
                DateTime fechaFin = DateTime.Now;
                DateTime fechaInicio = fechaFin.AddDays(-diasConfig.dias);

                // Filtrar las recetas guardadas por el intervalo de tiempo calculado y el ID del usuario
                var recetasGuardadasIds = await _context.Recetas_Guardadas
                    .Where(r => r.id_usuario == dto.id_usuario && r.fecha_acceso >= fechaInicio && r.fecha_acceso <= fechaFin)
                    .Select(rg => rg.id_receta)
                    .ToListAsync();

                // Si no hay recetas guardadas, retorna una lista vacía
                if (!recetasGuardadasIds.Any())
                {
                    return Ok(new List<RecetaGuardadasDTO>());
                }

                // Obtener las recetas y sus ingredientes
                var recetas = await _context.Recetas
                    .Where(r => recetasGuardadasIds.Contains(r.id_receta))
                    .ToListAsync();

                var recetasIngredientes = await _context.Recetas_Ingredientes
                    .Where(ri => recetasGuardadasIds.Contains(ri.id_receta))
                    .Join(_context.Ingredientes,
                        ri => ri.id_ingrediente,
                        i => i.id_ingrediente,
                        (ri, i) => new { ri.id_receta, i.nombre })
                    .ToListAsync();

                // Agrupar los ingredientes por receta
                var recetasConIngredientes = recetas
                    .Select(r => new RecetaGuardadasDTO
                    {
                        id_receta = r.id_receta,
                        titulo = r.titulo,
                        descripcion = r.descripcion,
                        instrucciones = r.instrucciones,
                        foto_receta = r.foto_receta,
                        porciones = r.porciones,
                        Ingredientes = recetasIngredientes
                            .Where(ri => ri.id_receta == r.id_receta)
                            .Select(ri => ri.nombre)
                            .Distinct()
                            .ToList()
                    })
                    .ToList();

                return Ok(recetasConIngredientes);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Error: {ex.Message}");
            }
        }


        [HttpPut("ActualizarFechaAcceso")]
        public async Task<IActionResult> ActualizarFechaAcceso([FromBody] Recetas_GuardadasDTO request)
        {
            if (request == null || request.id_receta <= 0 || request.id_usuario <= 0)
            {
                return BadRequest("Datos inválidos.");
            }

            try
            {
                // Encuentra el registro que quieres actualizar
                var recetaGuardada = await _context.Recetas_Guardadas
                    .FirstOrDefaultAsync(rt => rt.id_receta == request.id_receta && rt.id_usuario == request.id_usuario);

                if (recetaGuardada == null)
                {
                    return NotFound("Registro no encontrado.");
                }

                // Actualiza la fecha de acceso a la fecha y hora actual
                recetaGuardada.fecha_acceso = DateTime.Now;

                // Guarda los cambios en la base de datos
                await _context.SaveChangesAsync();

                return Ok("Fecha de acceso actualizada exitosamente.");
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Error al actualizar la fecha de acceso: {ex.Message}");
            }
        }



        [HttpDelete("Eliminar")]
        public async Task<IActionResult> Eliminar([FromBody] Recetas_GuardadasDTO request)
        {
            if (request == null || request.id_receta <= 0 || request.id_usuario <= 0)
            {
                return BadRequest("Datos inválidos.");
            }

            try
            {
                // Encuentra el registro que quieres eliminar
                var recetaGuardada = await _context.Recetas_Guardadas
                    .FirstOrDefaultAsync(rt => rt.id_receta == request.id_receta && rt.id_usuario == request.id_usuario);

                if (recetaGuardada == null)
                {
                    return NotFound("Registro no encontrado.");
                }

                // Elimina el registro
                _context.Recetas_Guardadas.Remove(recetaGuardada);
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
