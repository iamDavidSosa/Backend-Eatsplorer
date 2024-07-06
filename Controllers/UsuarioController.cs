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
    public class UsuarioController : ControllerBase
    {
        private readonly AppDbContext _context;

        public UsuarioController(AppDbContext context)
        {
            _context = context;
        }

        // POST: api/Usuario/register
        [HttpPost("register")]

        //Define un método asincrónico que retorna un resultado de acción para las solicitudes HTTP.
        public async Task<IActionResult> Register([FromBody] Usuario usuario)
        {
            // Verifica si ya existe un usuario con el mismo correo en la base de datos
            if (_context.Usuarios.Any(u => u.correo == usuario.correo))
            {
                // Si existe, devuelve una respuesta de error indicando que el correo ya está en uso
                return BadRequest("El correo ya está en uso");
            }

            // Agrega el nuevo usuario al contexto de la base de datos
            _context.Usuarios.Add(usuario);

            // Guarda los cambios en la base de datos de manera asíncrona
            await _context.SaveChangesAsync();


            // Devuelve una respuesta de éxito indicando que el usuario se registró correctamente
            return Ok("Usuario registrado exitosamente");
        }

        // POST: api/Usuario/login
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] Usuario login)
        {
            // Busca un usuario en la base de datos que coincida con el correo y la clave proporcionados
            var usuario = await _context.Usuarios
                .FirstOrDefaultAsync(u => u.correo == login.correo && u.clave == login.clave);

            // Si no se encuentra un usuario que coincida, retorna una respuesta no autorizada
            if (usuario == null)
            {
                return Unauthorized("Credenciales inválidas");
            }

            // Si se encuentra un usuario que coincida, retorna una respuesta de éxito
            return Ok("Inicio de sesión exitoso");
        }

        // GET: api/Usuario
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Usuario>>> GetUsuarios()
        {
            return await _context.Usuarios.ToListAsync();
        }

        // GET: api/Usuario/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Usuario>> GetUsuario(int id)
        {
            var usuario = await _context.Usuarios.FindAsync(id);

            if (usuario == null)
            {
                return NotFound();
            }

            return usuario;
        }

        // PUT: api/Usuario/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutUsuario(int id, Usuario usuario)
        {
            if (id != usuario.id_usuario)
            {
                return BadRequest();
            }

            _context.Entry(usuario).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UsuarioExists(id))
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

        // POST: api/Usuario
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Usuario>> PostUsuario(Usuario usuario)
        {
            _context.Usuarios.Add(usuario);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetUsuario", new { id = usuario.id_usuario }, usuario);
        }

        // DELETE: api/Usuario/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUsuario(int id)
        {
            var usuario = await _context.Usuarios.FindAsync(id);
            if (usuario == null)
            {
                return NotFound();
            }

            _context.Usuarios.Remove(usuario);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool UsuarioExists(int id)
        {
            return _context.Usuarios.Any(e => e.id_usuario == id);
        }
    }
}
