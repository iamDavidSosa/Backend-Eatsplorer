using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
    public class Preguntas_ContrasenaController : ControllerBase
    {
        private readonly AppDbContext _context;

        public Preguntas_ContrasenaController(AppDbContext context)
        {
            _context = context;
        }
        

        // POST: api/Preguntas_Contrasena
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Preguntas_Contrasena>> PostPreguntas_Contrasena(Preguntas_Contrasena preguntas_Contrasena)
        {
            _context.Preguntas_Contrasena.Add(preguntas_Contrasena);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetPreguntas_Contrasena", new { id = preguntas_Contrasena.IdPreguntasContrasena }, preguntas_Contrasena);
        }

        // GET: api/Preguntas_Contrasena
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Preguntas_Contrasena>>> GetPreguntas_Contrasena()
        {
            return await _context.Preguntas_Contrasena.ToListAsync();
        }

        [HttpDelete("Eliminar")]
        public async Task<IActionResult> Eliminar([FromBody] Preguntas_ContraseñaDTO request)
        {
            if (request == null || request.id <= 0)
            {
                return BadRequest("Datos inválidos.");
            }

            try
            {
                var preguntas = await _context.Preguntas_Contrasena.FindAsync(request.id);

                if (preguntas == null)
                {
                    return NotFound("Pregunta no encontrada.");
                }

                _context.Preguntas_Contrasena.Remove(preguntas);
                await _context.SaveChangesAsync();

                return Ok("Pregunta eliminada exitosamente.");
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Error al eliminar la pregunta: {ex.Message}");
            }
        }

        private bool Preguntas_ContrasenaExists(int id)
        {
            return _context.Preguntas_Contrasena.Any(e => e.IdPreguntasContrasena == id);
        }
    }
}
