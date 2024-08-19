using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PROYECTO_PRUEBA.Context;
using PROYECTO_PRUEBA.Models.DTOs;
using System.Security.Claims;

namespace PROYECTO_PRUEBA.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class Control_DespensaController : ControllerBase
    {

        private readonly AppDbContext _context;

        public Control_DespensaController(AppDbContext context)
        {
            _context = context;
        }


        /*   [HttpPost("ValidarIngredientes")]
           public async Task<IActionResult> ValidarIngredientes([FromBody] RecetasEliminarDTO dto)
           {
               var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
               if (userIdClaim == null)
               {
                   return Unauthorized(new { isSuccess = false, message = "Usuario no autenticado." });
               }

               try
               {
                   int idUsuario = int.Parse(userIdClaim.Value);

                   // Obtén los ingredientes requeridos para la receta
                   var ingredientesReceta = await _context.Recetas_Ingredientes
                       .Where(ri => ri.id_receta == dto.id_receta)
                       .ToListAsync();

                   // Verifica si el usuario tiene los ingredientes necesarios en su despensa
                   foreach (var ingredienteReceta in ingredientesReceta)
                   {
                       var ingredienteDespensa = await _context.Despensa
                           .Where(d => d.id_usuario == idUsuario && d.id_ingrediente == ingredienteReceta.id_ingrediente)
                           .FirstOrDefaultAsync();

                       if (ingredienteDespensa == null || ingredienteDespensa.cantidad < ingredienteReceta.cantidad)
                       {
                           return Ok(new
                           {
                               isSuccess = false,
                               message = "No tienes suficientes ingredientes en tu despensa.",
                               idIngredienteFaltante = ingredienteReceta.id_ingrediente
                           });
                       }
                   }

                   return Ok(new { isSuccess = true, message = "Tienes todos los ingredientes necesarios en las cantidades adecuadas." });
               }
               catch (Exception ex)
               {
                   return StatusCode(500, new { isSuccess = false, message = "Ocurrió un error al validar los ingredientes.", detalle = ex.Message });
               }
           }*/

        [HttpPost("ValidarIngredientes")]
        public async Task<IActionResult> ValidarIngredientes([FromBody] RecetasEliminarDTO dto)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
            {
                return Unauthorized(new { isSuccess = false, message = "Usuario no autenticado." });
            }

            try
            {
                int idUsuario = int.Parse(userIdClaim.Value);

                // Obtén los ingredientes requeridos para la receta
                var ingredientesReceta = await _context.Recetas_Ingredientes
                    .Where(ri => ri.id_receta == dto.id_receta)
                    .ToListAsync();

                // Verifica si el usuario tiene los ingredientes necesarios en su despensa
                foreach (var ingredienteReceta in ingredientesReceta)
                {
                    var ingredienteDespensa = await _context.Despensa
                        .Where(d => d.id_usuario == idUsuario && d.id_ingrediente == ingredienteReceta.id_ingrediente)
                        .FirstOrDefaultAsync();

                    if (ingredienteDespensa == null || ingredienteDespensa.cantidad < ingredienteReceta.cantidad)
                    {
                        return Ok(new
                        {
                            isSuccess = false,
                            message = "No tienes suficientes ingredientes en tu despensa.",
                            idIngredienteFaltante = ingredienteReceta.id_ingrediente
                        });
                    }
                }

                // Si tiene todos los ingredientes, resta las cantidades usadas de la despensa
                foreach (var ingredienteReceta in ingredientesReceta)
                {
                    var ingredienteDespensa = await _context.Despensa
                        .Where(d => d.id_usuario == idUsuario && d.id_ingrediente == ingredienteReceta.id_ingrediente)
                        .FirstOrDefaultAsync();

                    if (ingredienteDespensa != null)
                    {
                        ingredienteDespensa.cantidad -= ingredienteReceta.cantidad;

                        if (ingredienteDespensa.cantidad < 0)
                        {
                            ingredienteDespensa.cantidad = 0;
                        }

                        _context.Despensa.Update(ingredienteDespensa);
                    }
                }

                await _context.SaveChangesAsync();

                return Ok(new { isSuccess = true, message = "Ingredientes validados y cantidades actualizadas en la despensa." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { isSuccess = false, message = "Ocurrió un error al validar y actualizar los ingredientes.", detalle = ex.Message });
            }
        }


    }
}
