using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PROYECTO_PRUEBA.Context;
using PROYECTO_PRUEBA.Models;

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


        // GET: api/Preguntas_Contrasena
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Preguntas_Contrasena>>> GetPreguntas_Contrasena()
        {
            return await _context.Preguntas_Contrasena.ToListAsync();
        }

        // GET: api/Preguntas_Contrasena/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Preguntas_Contrasena>> GetPreguntas_Contrasena(int id)
        {
            var preguntas_Contrasena = await _context.Preguntas_Contrasena.FindAsync(id);

            if (preguntas_Contrasena == null)
            {
                return NotFound();
            }

            return preguntas_Contrasena;
        }

        // PUT: api/Preguntas_Contrasena/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutPreguntas_Contrasena(int id, Preguntas_Contrasena preguntas_Contrasena)
        {
            if (id != preguntas_Contrasena.IdPreguntasContrasena)
            {
                return BadRequest();
            }

            _context.Entry(preguntas_Contrasena).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!Preguntas_ContrasenaExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
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

        // DELETE: api/Preguntas_Contrasena/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePreguntas_Contrasena(int id)
        {
            var preguntas_Contrasena = await _context.Preguntas_Contrasena.FindAsync(id);
            if (preguntas_Contrasena == null)
            {
                return NotFound();
            }

            _context.Preguntas_Contrasena.Remove(preguntas_Contrasena);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool Preguntas_ContrasenaExists(int id)
        {
            return _context.Preguntas_Contrasena.Any(e => e.IdPreguntasContrasena == id);
        }
    }
}
