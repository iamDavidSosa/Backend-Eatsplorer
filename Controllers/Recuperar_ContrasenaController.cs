using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
    public class Recuperar_ContrasenaController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly Utilidades _utilidades;

        public Recuperar_ContrasenaController(AppDbContext context, Utilidades utilidades)
        {
            _context = context;
            _utilidades = utilidades;
        }


        public class VerificarRespuestasDTO
        {
            public int IdUsuario { get; set; }
            public string NuevaClave { get; set; }
            public List<string> Respuestas { get; set; }
        }

        [HttpPost("verificarRespuestas")]
        public async Task<IActionResult> VerificarRespuestas([FromBody] VerificarRespuestasDTO dto)
        {
            // Consulta para obtener el usuario por su ID
            var usuario = await _context.Usuarios.FindAsync(dto.IdUsuario);

            // Verifica si el usuario existe
            if (usuario == null)
            {
                return NotFound("Usuario no encontrado.");
            }

            // Consulta para obtener todas las entradas de recuperación para el usuario especificado
            var recuperaciones = await _context.Recuperar_Contrasena
                .Where(r => r.IdUsuario == dto.IdUsuario)
                .ToListAsync();

            // Verifica si no se encontró ninguna recuperación para el usuario especificado
            if (recuperaciones == null || recuperaciones.Count == 0)
            {
                return NotFound("No se encontraron preguntas para el usuario especificado.");
            }

            // Verifica que el número de respuestas proporcionadas sea igual al número de preguntas esperadas
            if (recuperaciones.Count != dto.Respuestas.Count)
            {
                return BadRequest("El número de respuestas proporcionadas no coincide con el número de preguntas.");
            }

            // Compara cada respuesta proporcionada con las respuestas correctas
            for (int i = 0; i < recuperaciones.Count; i++)
            {
                if (!dto.Respuestas[i].Equals(recuperaciones[i].respuesta, StringComparison.OrdinalIgnoreCase))
                {
                    return Unauthorized("Una o más respuestas son incorrectas.");
                }
            }

            // Encriptar la nueva clave antes de asignarla al usuario
            usuario.clave = _utilidades.EncriptarSHA256(dto.NuevaClave);

            // Guarda los cambios en la base de datos
            await _context.SaveChangesAsync();

            return Ok("La contraseña ha sido cambiada con éxito.");
        }



        // POST: api/Recuperar_Contrasena
        [HttpPost]
        public async Task<ActionResult<Recuperar_Contrasena>> PostRecuperar_Contrasena(Recuperar_Contrasena recuperar_Contrasena)
        {
            _context.Recuperar_Contrasena.Add(recuperar_Contrasena);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetRecuperar_Contrasena", new { id = recuperar_Contrasena.IdRecuperarContrasena }, recuperar_Contrasena);
        }


        // GET: api/Recuperar_Contrasena
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Recuperar_Contrasena>>> GetRecuperar_Contrasena()
        {
            return await _context.Recuperar_Contrasena.ToListAsync();
        }


        private bool Recuperar_ContrasenaExists(int id)
        {
            return _context.Recuperar_Contrasena.Any(e => e.IdRecuperarContrasena == id);
        }


        [HttpPut]
        public async Task<IActionResult> ActualizarRecuperarContrasena([FromBody] Recuperar_ContrasenaDTO request)
        {
            if (request == null || request.IdRecuperarContrasena <= 0)
            {
                return BadRequest("Datos inválidos.");
            }

            try
            {
                // Encuentra el registro por Id
                var recuperarContrasena = await _context.Recuperar_Contrasena.FindAsync(request.IdRecuperarContrasena);

                if (recuperarContrasena == null)
                {
                    return NotFound("Registro no encontrado.");
                }

                // Actualiza el campo 'respuesta'
                recuperarContrasena.respuesta = request.respuesta;

                // Guarda los cambios en la base de datos
                await _context.SaveChangesAsync();

                return Ok("Registro actualizado exitosamente.");
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Error al actualizar el registro: {ex.Message}");
            }
        }
    


    [HttpDelete("Eliminar")]
        public async Task<IActionResult> Eliminar([FromBody] RecuperarEliminarDTO request)
        {
            if (request == null || request.IdRecuperarContrasena <= 0)
            {
                return BadRequest("Datos inválidos.");
            }

            try
            {
                // Encuentra el registro en la tabla Recuperar_Contrasena
                var recuperarContrasena = await _context.Recuperar_Contrasena
                    .FindAsync(request.IdRecuperarContrasena);

                if (recuperarContrasena == null)
                {
                    return NotFound("Registro no encontrado.");
                }

                // Elimina el registro de la base de datos
                _context.Recuperar_Contrasena.Remove(recuperarContrasena);
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
