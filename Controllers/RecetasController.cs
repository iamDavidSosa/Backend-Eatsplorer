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
    public class RecetasController : ControllerBase
    {
        private readonly AppDbContext _context;

        public RecetasController(AppDbContext context)
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


        /*[HttpGet("Listar")]
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
        */

        [HttpGet("Listar")]
        public async Task<ActionResult<IEnumerable<object>>> Listar()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
            {
                return Unauthorized(new { isSuccess = false, message = "Usuario no autenticado." });
            }

            try
            {
                int idUsuario = int.Parse(userIdClaim.Value);

                var recetas = await _context.Recetas
                    .Select(r => new
                    {
                        r.id_receta,
                        r.titulo,
                        r.descripcion,
                        r.instrucciones,
                        r.foto_receta,
                        r.usuario_id,
                        r.fecha_creacion,
                        r.porciones,
                        r.likes,
                        Ingredientes = _context.Recetas_Ingredientes
                            .Where(ri => ri.id_receta == r.id_receta)
                            .Join(_context.Ingredientes,
                                  ri => ri.id_ingrediente,
                                  i => i.id_ingrediente,
                                  (ri, i) => new
                                  {
                                      i.id_ingrediente,
                                      i.nombre,
                                      i.medida,
                                      ri.cantidad
                                  }).ToList()
                    })
                    .ToListAsync();

                if (recetas == null || !recetas.Any())
                {
                    return NotFound(new { isSuccess = false, message = "No se encontraron recetas para este usuario." });
                }

                return Ok(new { isSuccess = true, recetas = recetas });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Error al obtener las recetas: {ex.Message}");
            }
        }

        [HttpGet("BuscarPorId/{id}")]
        public async Task<IActionResult> BuscarRecetaPorId(int id)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
            {
                return Unauthorized(new { isSuccess = false, message = "Usuario no autenticado." });
            }

            try
            {
                if (id <= 0)
                {
                    return BadRequest(new { isSuccess = false, message = "El ID de la receta es obligatorio y debe ser mayor a cero." });
                }

                int userId = int.Parse(userIdClaim.Value);

                var receta = await _context.Recetas
                    .Where(r => r.id_receta == id)
                    .Select(r => new
                    {
                        r.id_receta,
                        r.titulo,
                        r.descripcion,
                        r.instrucciones,
                        r.foto_receta,
                        r.usuario_id,
                        usuario_nombre = _context.Usuarios
                            .Where(u => u.id_usuario == r.usuario_id)
                            .Select(u => u.usuario)
                            .FirstOrDefault(),
                        r.fecha_creacion,
                        r.porciones,
                        r.likes,
                        isRecetaGuardada = _context.Recetas_Guardadas
                            .Any(rg => rg.id_receta == id && rg.id_usuario == userId), // Aquí se añade el booleano
                        Ingredientes = _context.Recetas_Ingredientes
                            .Where(ri => ri.id_receta == r.id_receta)
                            .Join(_context.Ingredientes,
                                  ri => ri.id_ingrediente,
                                  i => i.id_ingrediente,
                                  (ri, i) => new
                                  {
                                      i.id_ingrediente,
                                      i.nombre,
                                      ri.cantidad
                                  }).ToList()
                    })
                    .FirstOrDefaultAsync();

                if (receta == null)
                {
                    return NotFound(new { isSuccess = false, message = "Receta no encontrada." });
                }

                return Ok(new { isSuccess = true, data = receta });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { isSuccess = false, message = "Ocurrió un error al buscar la receta.", detalle = ex.Message });
            }
        }




        [HttpPost("BuscarPorNombre")]
        public async Task<IActionResult> BuscarRecetasPorNombre([FromBody] RecetasDTO request)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
            {
                return Unauthorized(new { isSuccess = false, message = "Usuario no autenticado." });
            }
            try
            {
                if (string.IsNullOrEmpty(request.titulo))
                {
                    return BadRequest(new { isSuccess = false, message = "El título de la receta es obligatorio." });
                }

                var recetas = await _context.Recetas
                    .Where(r => r.titulo != null && r.titulo.ToLower().Contains(request.titulo.ToLower()))
                    .Select(r => new
                    {
                        r.id_receta,
                        r.titulo,
                        r.descripcion,
                        r.instrucciones,
                        r.foto_receta,
                        r.usuario_id,
                        usuario_nombre = _context.Usuarios
                            .Where(u => u.id_usuario == r.usuario_id)
                            .Select(u => u.usuario)
                            .FirstOrDefault(),
                        r.fecha_creacion,
                        r.porciones,
                        r.likes,
                        Ingredientes = _context.Recetas_Ingredientes
                            .Where(ri => ri.id_receta == r.id_receta)
                            .Join(_context.Ingredientes,
                                  ri => ri.id_ingrediente,
                                  i => i.id_ingrediente,
                                  (ri, i) => new
                                  {
                                      i.id_ingrediente,
                                      i.nombre,
                                      ri.cantidad
                                  }).ToList()
                    })
                    .ToListAsync();

                return Ok(new { isSuccess = true, data = recetas });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { isSuccess = false, message = "Ocurrió un error al buscar las recetas.", detalle = ex.Message });
            }
        }


        [HttpPost("BuscarPorIngrediente")]
        public async Task<IActionResult> BuscarRecetas([FromBody] IngredientesRequest2 request)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
            {
                return Unauthorized(new { isSuccess = false, message = "Usuario no autenticado." });
            }
            try
            {
                var ingredientes = request.Id_Ingredientes;

                var recetasIds = await _context.Recetas_Ingredientes
                    .Where(ri => ingredientes.Contains(ri.id_ingrediente))
                    .GroupBy(ri => ri.id_receta)
                    .Where(g => g.Count() == ingredientes.Count)
                    .Select(g => g.Key)
                    .ToListAsync();

                var recetas = await _context.Recetas
                    .Where(r => recetasIds.Contains(r.id_receta))
                    .Select(r => new
                    {
                        r.id_receta,
                        r.titulo,
                        r.descripcion,
                        r.instrucciones,
                        r.foto_receta,
                        r.usuario_id,
                        usuario_nombre = _context.Usuarios
                            .Where(u => u.id_usuario == r.usuario_id)
                            .Select(u => u.usuario)
                            .FirstOrDefault(),
                        r.fecha_creacion,
                        r.porciones,
                        r.likes,
                        Ingredientes = _context.Recetas_Ingredientes
                            .Where(ri => ri.id_receta == r.id_receta)
                            .Join(_context.Ingredientes,
                                  ri => ri.id_ingrediente,
                                  i => i.id_ingrediente,
                                  (ri, i) => new
                                  {
                                      i.id_ingrediente,
                                      i.nombre,
                                      ri.cantidad
                                  }).ToList()
                    })
                    .ToListAsync();

                return Ok(new { isSuccess = true, Recetas = recetas });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { isSuccess = false, message = "Ocurrió un error al buscar las recetas.", detalle = ex.Message });
            }
        }


        [HttpPost("BuscarSinIngrediente")]
        public async Task<IActionResult> BuscarRecetasSinIngredientes([FromBody] IngredientesRequest2 request)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
            {
                return Unauthorized(new { isSuccess = false, message = "Usuario no autenticado." });
            }
            try
            {
                var ingredientesIds = request.Id_Ingredientes;

                var recetasSinIngredientes = await _context.Recetas
                    .Where(r => !_context.Recetas_Ingredientes
                        .Where(ri => ingredientesIds.Contains(ri.id_ingrediente))
                        .Select(ri => ri.id_receta)
                        .Contains(r.id_receta))
                    .Select(r => new
                    {
                        r.id_receta,
                        r.titulo,
                        r.descripcion,
                        r.instrucciones,
                        r.foto_receta,
                        r.usuario_id,
                        usuario_nombre = _context.Usuarios
                            .Where(u => u.id_usuario == r.usuario_id)
                            .Select(u => u.usuario)
                            .FirstOrDefault(),
                        r.fecha_creacion,
                        r.porciones,
                        r.likes,
                        Ingredientes = _context.Recetas_Ingredientes
                            .Where(ri => ri.id_receta == r.id_receta)
                            .Join(_context.Ingredientes,
                                  ri => ri.id_ingrediente,
                                  i => i.id_ingrediente,
                                  (ri, i) => new
                                  {
                                      i.id_ingrediente,
                                      i.nombre,
                                      ri.cantidad
                                  }).ToList()
                    })
                    .ToListAsync();

                return Ok(new { isSuccess = true, Recetas = recetasSinIngredientes });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { isSuccess = false, message = "Ocurrió un error al buscar las recetas.", detalle = ex.Message });
            }
        }


        [HttpPost("BuscarPorTag")]
        public async Task<IActionResult> BuscarRecetasPorTag([FromBody] TagsDTO2 request)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
            {
                return Unauthorized(new { isSuccess = false, message = "Usuario no autenticado." });
            }
            try
            {
                var tags = request.Id_Tags;

                var recetasIds = await _context.RecetasTags
                    .Where(rt => tags.Contains(rt.id_tag))
                    .GroupBy(rt => rt.id_receta)
                    .Where(g => g.Count() == tags.Count)
                    .Select(g => g.Key)
                    .ToListAsync();

                var recetas = await _context.Recetas
                    .Where(r => recetasIds.Contains(r.id_receta))
                    .Select(r => new
                    {
                        r.id_receta,
                        r.titulo,
                        r.descripcion,
                        r.instrucciones,
                        r.foto_receta,
                        r.usuario_id,
                        usuario_nombre = _context.Usuarios
                            .Where(u => u.id_usuario == r.usuario_id)
                            .Select(u => u.usuario)
                            .FirstOrDefault(),
                        r.fecha_creacion,
                        r.porciones,
                        r.likes,
                        Ingredientes = _context.Recetas_Ingredientes
                            .Where(ri => ri.id_receta == r.id_receta)
                            .Join(_context.Ingredientes,
                                  ri => ri.id_ingrediente,
                                  i => i.id_ingrediente,
                                  (ri, i) => new
                                  {
                                      i.id_ingrediente,
                                      i.nombre,
                                      ri.cantidad
                                  }).ToList()
                    })
                    .ToListAsync();

                return Ok(new { isSuccess = true, Recetas = recetas });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { isSuccess = false, message = "Ocurrió un error al buscar las recetas.", detalle = ex.Message });
            }
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

            // Obtención de los ingredientes según el tipo (gusta, no consume, alérgico)
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

            // Obtención de las recetas filtradas según las restricciones del usuario
            var recetasFiltradas = await _context.Recetas
                .Where(r =>
                    !_context.Recetas_Ingredientes
                        .Where(ri => ri.id_receta == r.id_receta)
                        .Any(ri => ingredientesAlergico.Contains(ri.id_ingrediente)))
                .ToListAsync();

            // Filtrado adicional basado en las preferencias del usuario
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
            })
            .Select(r => new
            {
                r.id_receta,
                r.titulo,
                r.descripcion,
                r.instrucciones,
                r.foto_receta,
                r.usuario_id,
                usuario_nombre = _context.Usuarios
                    .Where(u => u.id_usuario == r.usuario_id)
                    .Select(u => u.usuario)
                    .FirstOrDefault(),
                r.fecha_creacion,
                r.porciones,
                r.likes,
                Ingredientes = _context.Recetas_Ingredientes
                    .Where(ri => ri.id_receta == r.id_receta)
                    .Join(_context.Ingredientes,
                          ri => ri.id_ingrediente,
                          i => i.id_ingrediente,
                          (ri, i) => new
                          {
                              i.id_ingrediente,
                              i.nombre,
                              ri.cantidad
                          }).ToList()
            })
            .ToList();

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

            int idUsuario;
            if (!int.TryParse(userIdClaim.Value, out idUsuario))
            {
                return Unauthorized(new { isSuccess = false, message = "ID de usuario inválido." });
            }

            try
            {
                var recetasConDespensa = await _context.Recetas
                    .Where(r => _context.Recetas_Ingredientes
                        .Where(ri => ri.id_receta == r.id_receta)
                        .All(ri => _context.Despensa
                            .Where(d => d.id_usuario == idUsuario && d.id_ingrediente == ri.id_ingrediente)
                            .Any(d => d.cantidad >= ri.cantidad)))
                    .Select(r => new
                    {
                        r.id_receta,
                        r.titulo,
                        r.descripcion,
                        r.instrucciones,
                        r.foto_receta,
                        r.usuario_id,
                        usuario_nombre = _context.Usuarios
                            .Where(u => u.id_usuario == r.usuario_id)
                            .Select(u => u.usuario)
                            .FirstOrDefault(),
                        r.fecha_creacion,
                        r.porciones,
                        r.likes,
                        Ingredientes = _context.Recetas_Ingredientes
                            .Where(ri => ri.id_receta == r.id_receta)
                            .Join(_context.Ingredientes,
                                  ri => ri.id_ingrediente,
                                  i => i.id_ingrediente,
                                  (ri, i) => new
                                  {
                                      i.id_ingrediente,
                                      i.nombre,
                                      ri.cantidad
                                  }).ToList()
                    })
                    .ToListAsync();

                return Ok(new { isSuccess = true, recetasConDespensa });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { isSuccess = false, message = "Ocurrió un error al obtener las recetas.", detalle = ex.Message });
            }
        }



        /*  [HttpDelete("Eliminar")]
          public async Task<IActionResult> Eliminar([FromBody] RecetasEliminarDTO request)
          {
              if (request == null || request.id_receta <= 0)
              {
                  return BadRequest("Datos inválidos.");
              }

              try
              {
                  var receta = await _context.Recetas.FindAsync(request.id_receta);

                  if (receta == null)
                  {
                      return NotFound("Receta no encontrada.");
                  }

                  _context.Recetas.Remove(receta);
                  await _context.SaveChangesAsync();

                  return Ok("Receta eliminada exitosamente.");
              }
              catch (Exception ex)
              {
                  return StatusCode(StatusCodes.Status500InternalServerError, $"Error al eliminar: {ex.Message}");
              }
          }*/

        [HttpDelete("Eliminar")]
        public async Task<IActionResult> Eliminar([FromBody] RecetasEliminarDTO request)
        {
            if (request == null || request.id_receta <= 0)
            {
                return BadRequest("Datos inválidos.");
            }

            try
            {
                // Obtén el ID del usuario desde el token
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
                if (userIdClaim == null)
                {
                    return Unauthorized("Usuario no autenticado.");
                }

                int idUsuario = int.Parse(userIdClaim.Value);

                // Busca la receta que coincida con el ID y que el autor sea el usuario autenticado
                var receta = await _context.Recetas
                    .FirstOrDefaultAsync(r => r.id_receta == request.id_receta && r.usuario_id == idUsuario);

                if (receta == null)
                {
                    return NotFound("Receta no encontrada o no tienes permiso para eliminarla.");
                }

                // Elimina la receta
                _context.Recetas.Remove(receta);
                await _context.SaveChangesAsync();

                return Ok("Receta eliminada exitosamente.");
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Error al eliminar: {ex.Message}");
            }
        }


    /*    [HttpPut]
        public async Task<IActionResult> ActualizarReceta([FromBody] RecetasActualizarDTO request)
        {
            if (request == null || request.id_receta <= 0)
            {
                return BadRequest("Datos inválidos.");
            }

            try
            {
                var receta = await _context.Recetas.FindAsync(request.id_receta);

                if (receta == null)
                {
                    return NotFound("Receta no encontrada.");
                }

                // Actualiza los campos del registro
                receta.titulo = request.titulo;
                receta.descripcion = request.descripcion;
                receta.instrucciones = request.instrucciones;
                receta.foto_receta = request.foto_receta;
                receta.porciones = request.porciones;

                // Guarda los cambios en la base de datos
                await _context.SaveChangesAsync();

                return Ok("Receta actualizada exitosamente.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error: {ex.Message}");
            }
        }*/


    }
}