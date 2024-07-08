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



    }
}
