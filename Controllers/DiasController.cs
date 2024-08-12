using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PROYECTO_PRUEBA.Context;
using PROYECTO_PRUEBA.Custom;
using PROYECTO_PRUEBA.Models;
using Microsoft.EntityFrameworkCore;
using PROYECTO_PRUEBA.Models.DTOs;

namespace PROYECTO_PRUEBA.Controllers
{

    [Route("api/[controller]")]
    [Authorize]
    [ApiController]
    public class DiasController : ControllerBase
    {
        private readonly AppDbContext _context;

        public DiasController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost("Ingresar")]
        public async Task<IActionResult> Ingresar([FromBody] Dias day)
        {
            try
            {

                // Agregamos el dia a la base de datos
                _context.Dias.Add(day);
                await _context.SaveChangesAsync();

                return Ok(new { message = "Dias ingresado con éxito." });

            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Error al ingresar el numero de dias: {ex.Message}");
            }
        }

        [HttpGet("Listar")]
        public async Task<ActionResult<IEnumerable<Dias>>> Listar()
        {
            try
            {
                var dias = await _context.Dias.ToListAsync();
                return Ok(dias);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Error al obtener los dias: {ex.Message}");
            }


        }

        [HttpPut]
        public async Task<IActionResult> ActualizarDia([FromBody] DiasDTO request)
        {
            if (request == null || request.id_dias <= 0)
            {
                return BadRequest("Datos inválidos.");
            }

            try
            {
                var dia = await _context.Dias.FindAsync(request.id_dias);

                if (dia == null)
                {
                    return NotFound("Día no encontrado.");
                }

                // Actualiza el campo 'dias' del registro
                dia.dias = request.dias;

                // Guarda los cambios en la base de datos
                await _context.SaveChangesAsync();

                return Ok("Día actualizado exitosamente.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error: {ex.Message}");
            }
        }

    }
}