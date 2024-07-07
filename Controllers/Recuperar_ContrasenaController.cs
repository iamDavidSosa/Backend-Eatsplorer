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
    public class Recuperar_ContrasenaController : ControllerBase
    {
        private readonly AppDbContext _context;

        public Recuperar_ContrasenaController(AppDbContext context)
        {
            _context = context;
        }

        //CODIGO PARA RESPONDER LAS PREGUNTAS

        [HttpPost("verificarRespuestas")]
        public async Task<IActionResult> VerificarRespuestas(int idUsuario, string nuevaClave, [FromBody] List<string> respuestas)
        {

            // Consulta para obtener el usuario por su ID
            var Usuario = await _context.Usuarios.FindAsync(idUsuario);

            // Verifica si el usuario existe
            if (Usuario == null)
            {
                return NotFound("Usuario no encontrado.");
            }


            // Consulta que haya un usuario con el id que proporcionamos
            var recuperacion = await _context.Recuperar_Contrasena
                .FirstOrDefaultAsync(r => r.IdUsuario == idUsuario);

            // Verifica si no se encontró ninguna recuperación para el usuario especificado
            if (recuperacion == null)
            {
                // Devuelve un resultado NotFound con un mensaje si no se encontraron preguntas
                return NotFound("No se encontraron preguntas para el usuario especificado.");
            }

            // Si se encontró la recuperación, crea una lista de cadenas que contienen las preguntas de recuperación
            var respuestasCorrectas = new List<string>
        {
            recuperacion.Respuesta1,
            recuperacion.Respuesta2,
            recuperacion.Respuesta3
        };

            // Compara cada respuesta proporcionada con las respuestas correctas
            for (int i = 0; i < respuestas.Count; i++)
            {
                // Compara ignorando mayúsculas y minúsculas
                if (!respuestas[i].Equals(respuestasCorrectas[i], StringComparison.OrdinalIgnoreCase))
                {
                    // Si alguna respuesta no coincide, devuelve un error 401
                    return Unauthorized("Una o más respuestas son incorrectas.");
                }
            }

            // Si todas las respuestas coinciden, permite al usuario cambiar su contraseña
            Usuario.clave = nuevaClave;

            // Guarda los cambios en la base de datos
            await _context.SaveChangesAsync();

            // Si todas las respuestas coinciden, devuelve un mensaje de éxito
            return Ok("La contraseña ha sido cambiada con exito.");
        }





        // GET: api/Recuperar_Contrasena
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Recuperar_Contrasena>>> GetRecuperar_Contrasena()
        {
            return await _context.Recuperar_Contrasena.ToListAsync();
        }

        // GET: api/Recuperar_Contrasena/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Recuperar_Contrasena>> GetRecuperar_Contrasena(int id)
        {
            var recuperar_Contrasena = await _context.Recuperar_Contrasena.FindAsync(id);

            if (recuperar_Contrasena == null)
            {
                return NotFound();
            }

            return recuperar_Contrasena;
        }

        // PUT: api/Recuperar_Contrasena/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutRecuperar_Contrasena(int id, Recuperar_Contrasena recuperar_Contrasena)
        {
            if (id != recuperar_Contrasena.IdRecuperarContrasena)
            {
                return BadRequest();
            }

            _context.Entry(recuperar_Contrasena).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!Recuperar_ContrasenaExists(id))
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

        // POST: api/Recuperar_Contrasena
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Recuperar_Contrasena>> PostRecuperar_Contrasena(Recuperar_Contrasena recuperar_Contrasena)
        {
            _context.Recuperar_Contrasena.Add(recuperar_Contrasena);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetRecuperar_Contrasena", new { id = recuperar_Contrasena.IdRecuperarContrasena }, recuperar_Contrasena);
        }

        // DELETE: api/Recuperar_Contrasena/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRecuperar_Contrasena(int id)
        {
            var recuperar_Contrasena = await _context.Recuperar_Contrasena.FindAsync(id);
            if (recuperar_Contrasena == null)
            {
                return NotFound();
            }

            _context.Recuperar_Contrasena.Remove(recuperar_Contrasena);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool Recuperar_ContrasenaExists(int id)
        {
            return _context.Recuperar_Contrasena.Any(e => e.IdRecuperarContrasena == id);
        }
    }
}
