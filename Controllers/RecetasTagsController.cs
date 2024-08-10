﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PROYECTO_PRUEBA.Context;
using PROYECTO_PRUEBA.Models;

namespace PROYECTO_PRUEBA.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class RecetasTagsController : ControllerBase
    {
        private readonly AppDbContext _context;
        public RecetasTagsController(AppDbContext context)
        {
            _context = context;
        }

        // POST: api/RecetasTags/Ingresar
        [HttpPost("Ingresar")]
        public async Task<IActionResult> Ingresar([FromBody] RecetasTags recetaTag)
        {
            if (recetaTag == null || recetaTag.id_receta <= 0 || recetaTag.id_tag <= 0)
            {
                return BadRequest("Invalid input.");
            }

            try
            {
                // Verificar si la relación ya existe
                var existingRelacion = await _context.RecetasTags
                    .FirstOrDefaultAsync(rt => rt.id_receta == recetaTag.id_receta &&
                                               rt.id_tag == recetaTag.id_tag);

                if (existingRelacion != null)
                {
                    return Ok(); // No realizar ninguna acción si la relación ya existe
                }

                // Agregamos la relación a la base de datos
                _context.RecetasTags.Add(recetaTag);
                await _context.SaveChangesAsync();

                return Ok(new { message = "Relación ingresada con éxito." });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Error al ingresar la relación: {ex.Message}");
            }
        }



    }
}
