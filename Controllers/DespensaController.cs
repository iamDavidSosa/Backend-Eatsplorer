using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PROYECTO_PRUEBA.Context;
using PROYECTO_PRUEBA.Models;
using PROYECTO_PRUEBA.Models.DTOs;
using System.Security.Claims;

namespace PROYECTO_PRUEBA.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class DespensaController : ControllerBase
    {
        private readonly AppDbContext _context;

        public DespensaController(AppDbContext context)
        {
            _context = context;
        }

        // POST: api/Despensa/agregarActualizarDespensa
        [HttpPost("agregarActualizarDespensa")]
        public async Task<IActionResult> AgregarActualizarDespensa(DespensaDTO despensaDTO)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
            {
                return Unauthorized(new { isSuccess = false, mesasge = "Usuario no autenticado." });
            }

            int id_usuario = int.Parse(userIdClaim.Value);

            var existente = await _context.Despensa
                .FirstOrDefaultAsync(d => d.id_usuario == id_usuario && d.id_ingrediente == despensaDTO.id_ingrediente);

            if (existente != null)
            {
                existente.cantidad += despensaDTO.cantidad;
                existente.fecha_agregado = despensaDTO.fecha_agregado; // Actualiza la fecha de agregado si es necesario
                _context.Despensa.Update(existente);
            }
            else
            {
                Despensa despensa = new Despensa
                {
                    id_usuario = id_usuario,
                    id_ingrediente = despensaDTO.id_ingrediente,
                    cantidad = despensaDTO.cantidad,
                    fecha_agregado = despensaDTO.fecha_agregado
                };
                _context.Despensa.Add(despensa);
            }

            await _context.SaveChangesAsync();
            return Ok(new { isSuccess = true, message = "Operación exitosa." });
        }

        // GET: api/Despensa/listarDespensa
        [HttpGet("listarDespensa")]
        public async Task<IActionResult> ListarDespensa()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
            {
                return Unauthorized(new { isSuccess = false, mesasge = "Usuario no autenticado." });
            }

            int id_usuario = int.Parse(userIdClaim.Value);

            var despensa = await _context.Despensa
                .Where(d => d.id_usuario == id_usuario)
                .Join(_context.Ingredientes, d => d.id_ingrediente, i => i.id_ingrediente, (d, i) => new { d.id_ingrediente, i.nombre, d.cantidad, d.fecha_agregado })
                .Select(d => new
                {
                    d.id_ingrediente,
                    d.nombre,
                    d.cantidad,
                    d.fecha_agregado
                })
                .ToListAsync();

            if (despensa == null || despensa.Count == 0)
            {
                return NotFound(new { isSuccess = false, message = "No se encontraron registros." });
            }

            return Ok(new { isSuccess = true, despensa });
        }

        // UPDATE: api/Despensa/actualizarCantidad
        [HttpPut("actualizarCantidad")]
        public async Task<IActionResult> ActualizarCantidad(ActualizarDespensaDTO actualizarDespensa)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
            {
                return Unauthorized(new { isSuccess = false, mesasge = "Usuario no autenticado." });
            }

            int id_usuario = int.Parse(userIdClaim.Value);

            var existente = await _context.Despensa
                .FirstOrDefaultAsync(d => d.id_usuario == id_usuario && d.id_ingrediente == actualizarDespensa.id_ingrediente);

            if (existente == null)
            {
                return NotFound(new { isSuccess = false, message = "Registro no encontrado." });
            }

            existente.cantidad = actualizarDespensa.cantidad;
            _context.Despensa.Update(existente);
            await _context.SaveChangesAsync();

            return Ok(new { isSuccess = true, message = "Cantidad actualizada con éxito." });
        }

        // DELETE: api/Despensa/eliminarIngrediente
        [HttpDelete("eliminarIngrediente")]
        public async Task<IActionResult> EliminarIngrediente(EliminarIngredienteDTO eliminarIngrediente)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
            {
                return Unauthorized(new { isSuccess = false, mesasge = "Usuario no autenticado." });
            }

            int id_usuario = int.Parse(userIdClaim.Value);

            var existente = await _context.Despensa
                .FirstOrDefaultAsync(d => d.id_usuario == id_usuario && d.id_ingrediente == eliminarIngrediente.id_ingrediente);

            if (existente == null)
            {
                return NotFound(new { isSuccess = false, message = "Registro no encontrado." });
            }

            _context.Despensa.Remove(existente);
            await _context.SaveChangesAsync();

            return Ok(new { isSuccess = true, message = "Ingrediente eliminado con éxito." });
        }

        public class EliminarIngredienteDTO
        {
            public int id_ingrediente { get; set; }
        }
    }
}
