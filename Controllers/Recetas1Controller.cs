using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using PROYECTO_PRUEBA.Context;
using PROYECTO_PRUEBA.Models;

namespace PROYECTO_PRUEBA.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class Recetas1Controller : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly JsonSerializerOptions _jsonOptions;

        public Recetas1Controller(AppDbContext context)
        {
            _context = context;

            // Configurar JsonSerializerOptions para manejar referencias circulares
            _jsonOptions = new JsonSerializerOptions
            {
                ReferenceHandler = ReferenceHandler.Preserve,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = true // Opcional: para una salida JSON indentada para mayor legibilidad
            };
        }

        // GET: api/Recetas1
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Recetas>>> GetRecetas()
        {
            return await _context.Recetas.ToListAsync();
        }

        // GET: api/Recetas1/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Recetas>> GetRecetas(int id)
        {
            var recetas = await _context.Recetas.FindAsync(id);

            if (recetas == null)
            {
                return NotFound();
            }

            return recetas;
        }

        // PUT: api/Recetas1/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutRecetas(int id, Recetas recetas)
        {
            if (id != recetas.id_receta)
            {
                return BadRequest();
            }

            _context.Entry(recetas).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!RecetasExists(id))
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

        // POST: api/Recetas1
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        /*[HttpPost]
        public async Task<ActionResult<Recetas>> PostRecetas(Recetas recetas)
        {
            _context.Recetas.Add(recetas);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetRecetas", new { id = recetas.id_receta }, recetas);
        }/*/


        // POST: api/Recetas1
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
       
        // Tu método para insertar recetas
        [HttpPost]
        public async Task<ActionResult> CrearReceta([FromBody] Recetas receta)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Insertar lógica para guardar la receta en la base de datos
            _context.Recetas.Add(receta);
            await _context.SaveChangesAsync();

            // Retornar una respuesta exitosa junto con la receta insertada
            return Ok(receta);
        }




        // DELETE: api/Recetas1/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRecetas(int id)
        {
            var recetas = await _context.Recetas.FindAsync(id);
            if (recetas == null)
            {
                return NotFound();
            }

            _context.Recetas.Remove(recetas);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool RecetasExists(int id)
        {
            return _context.Recetas.Any(e => e.id_receta == id);
        }
    }
}
