﻿using Microsoft.AspNetCore.Authorization;
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

        [HttpGet("ListarPorDias")]
        public async Task<ActionResult<IEnumerable<Recetas_Guardadas>>> ListarPorDias()
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

                // Filtrar las recetas guardadas por el intervalo de tiempo calculado
                var recetas_guardadas = await _context.Recetas_Guardadas
                    .Where(r => r.fecha_acceso >= fechaInicio && r.fecha_acceso <= fechaFin)
                    .ToListAsync();

                return Ok(recetas_guardadas);
            }
            catch (Exception ex)
            {
                // Retornar un error 500 si ocurre una excepción
                return StatusCode(StatusCodes.Status500InternalServerError, $"Error: {ex.Message}");
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
