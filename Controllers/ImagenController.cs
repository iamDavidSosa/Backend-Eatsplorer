using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace PROYECTO_PRUEBA.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ImagenController : ControllerBase
    {
        private readonly ImageUploadService _imageUploadService;

    public ImagenController(ImageUploadService imageUploadService)
    {
        _imageUploadService = imageUploadService;
    }

    [HttpPost("subirImagen")]
    public async Task<IActionResult> UploadImage([FromForm] IFormFile image)
    {
        if (image == null || image.Length == 0)
            return BadRequest(new { isSuccess = false, message = "Error al subir la imagen" });

        var imageUrl = await _imageUploadService.UploadImageAsync(image, "fotos");

        return Ok(new { isSuccess = true, Url = imageUrl });
    }
    }
}
