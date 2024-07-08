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

    }
}
