using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PROYECTO_PRUEBA.Custom;
using PROYECTO_PRUEBA.Models;
using PROYECTO_PRUEBA.Models.DTOs;
using Microsoft.AspNetCore.Authorization;
using PROYECTO_PRUEBA.Context;
using Microsoft.EntityFrameworkCore.SqlServer.Query.Internal;

namespace PROYECTO_PRUEBA.Controllers
{
    [Route("api/[controller]")]
    [Authorize]
    [ApiController]
    public class RecetasController : ControllerBase
    {
        /* private readonly AppDbContext _appDbContext;

         public RecetasController(AppDbContext appDbContext)
         {
             _appDbContext = appDbContext;
         }


         [HttpGet]
         [Route("Lista")]


         public async Task<IActionResult> Lista()
         {
             var lista = await _appDbContext.Recetas.ToListAsync();
             return StatusCode(StatusCodes.Status200OK, new{value = lista});

          }*/

        private readonly AppDbContext _appDbContext;

        public RecetasController(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }

        [HttpGet]
        [Route("Lista")]
        public async Task<IActionResult> Lista()
        {
            var lista = await _appDbContext.Recetas     //Usa el contexto de la base de datos (_appDbContext) para obtener una lista de todas las recetas
                .Include(r => r.Recetas_Ingredientes)   //Incluye la colección Recetas_Ingredientes relacionada con cada receta
                    .ThenInclude(ri => ri.Ingrediente)  //Dentro de cada Recetas_Ingredientes, incluye la entidad Ingrediente relacionada
                .ToListAsync();                         //Convierte el resultado en una lista de forma asincrónica


            //Transforma la lista de recetas en una nueva lista de objetos anónimos con las propiedades deseadas
            //Incluye las propiedades de la receta y una lista de nombres de ingredientes asociados

            var result = lista.Select(r => new          
            {
                r.id_receta,
                r.titulo,
                r.descripcion,
                r.instrucciones,
                r.foto_receta,
                r.usuario_id,
                r.fecha_creacion,
                Ingredientes = r.Recetas_Ingredientes.Select(ri => ri.Ingrediente.nombre).ToList()
            }).ToList();

            //Devuelve un resultado de estado HTTP 200 (OK) 
            return StatusCode(StatusCodes.Status200OK, new { value = result });
        }


        ///////////////
        ///////////////
        ///

        [HttpPost]
        [Route("Crear")]
        public async Task<IActionResult> CrearReceta([FromBody] Recetas receta)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Crear una nueva instancia de la clase Recetas utilizando los datos del DTO
            var nuevaReceta = new Recetas
            {
                titulo = receta.titulo,
                descripcion = receta.descripcion,
                instrucciones = receta.instrucciones,
                foto_receta = receta.foto_receta,
                usuario_id = receta.usuario_id,
                fecha_creacion = DateTime.Now
            };

            // Agregar la nueva receta al contexto de la base de datos
            _appDbContext.Recetas.Add(nuevaReceta);

            // Para cada ingrediente, crear un nuevo Recetas_Ingredientes y agregarlo al contexto
            foreach (var ingredienteId in receta.Ingredientes)
            {
                var recetaIngrediente = new Recetas_Ingredientes
                {
                    id_receta = nuevaReceta.id_receta,
                    id_ingrediente = ingredienteId
                };
                _appDbContext.Recetas_Ingredientes.Add(recetaIngrediente);
            }

            try
            {
                // Guardar cambios en la base de datos
                await _appDbContext.SaveChangesAsync();
                return StatusCode(StatusCodes.Status201Created);  // Recurso creado exitosamente
            }
            catch (DbUpdateException ex)
            {
                // Manejar errores de base de datos
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }


    }
}
