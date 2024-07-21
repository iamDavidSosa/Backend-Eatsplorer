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
    //[Route("api/[controller]")]
    //[Authorize]
    //[ApiController]
    //public class RecetasController : ControllerBase
    //{
       /*  private readonly AppDbContext _appDbContext;

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

       /* private readonly AppDbContext _appDbContext;

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

        //CODIGO PARA CREAR LAS RECETAS CON SUS RESPECTIVOS INGREDIENTES
        [HttpPost]
        [Route("Crear")]
        public async Task<IActionResult> CrearReceta([FromBody] Recetas receta)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
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
                await _appDbContext.SaveChangesAsync();


                // Proceso para agregar los ingredientes
                foreach (var ingrediente in receta.Recetas_Ingredientes.Select(ri => ri.Ingrediente))
                {
                    // Verificar si el ingrediente ya existe en la base de datos
                    var ingredienteExistente = await _appDbContext.Ingredientes
                        .FirstOrDefaultAsync(i => i.nombre == ingrediente.nombre);

                    // Si no existe, agregar el nuevo ingrediente
                    if (ingredienteExistente == null)
                    {
                        ingredienteExistente = new Ingredientes { nombre = ingrediente.nombre };
                        _appDbContext.Ingredientes.Add(ingredienteExistente);
                        await _appDbContext.SaveChangesAsync();
                    }

                    // Crear la relación entre la receta y el ingrediente
                    var nuevaRecetaIngrediente = new Recetas_Ingredientes
                    {
                        id_receta = nuevaReceta.id_receta,
                        id_ingrediente = ingredienteExistente.id_ingrediente
                    };

                    _appDbContext.Recetas_Ingredientes.Add(nuevaRecetaIngrediente);
                }

                // Guardar los cambios en la tabla Recetas_Ingredientes
                await _appDbContext.SaveChangesAsync();

                return StatusCode(StatusCodes.Status201Created);  // Recurso creado exitosamente
            }
            catch (DbUpdateException ex)
            {
                // Manejar errores de base de datos
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }



    }*/
}
