using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PROYECTO_PRUEBA.Custom;
using PROYECTO_PRUEBA.Models;
using PROYECTO_PRUEBA.Models.DTOs;
using Microsoft.AspNetCore.Authorization;
using PROYECTO_PRUEBA.Context;
using Google.Apis.Auth;
using Newtonsoft.Json;
using System.Text;

namespace PROYECTO_PRUEBA.Controllers
{
    [Route("api/[controller]")]
    [AllowAnonymous]
    [ApiController]
    public class AccesoController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly Utilidades _utilidades;

        public AccesoController(AppDbContext context, Utilidades utilidades)
        {
            _context = context;
            _utilidades = utilidades;
        }

        // POST: api/Acceso/register
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] UsuarioDTO usuarioDTO)
        {
            if (_context.Usuarios.Any(u => u.correo == usuarioDTO.correo))
            {
                return BadRequest(new { isSuccess = false });
            }

            var usuario = new Usuario
            {
                correo = usuarioDTO.correo,
                usuario = usuarioDTO.usuario,
                clave = _utilidades.EncriptarSHA256(usuarioDTO.clave),
                id_rol = 2,
                fecha_creacion = DateTime.Now
            };

            await _context.Usuarios.AddAsync(usuario);
            await _context.SaveChangesAsync();

            if(usuario.id_usuario != 0)
            {
                return await LoginAfterRegistration(usuarioDTO.correo, usuarioDTO.clave);
            }
            else
            {
                return BadRequest(new {isSuccess = false});
            }
        }

        private async Task<IActionResult> LoginAfterRegistration(string correo, string clave)
        {
            var usuarioEncontrado = await _context.Usuarios
                .Where(u => u.correo == correo && u.clave == _utilidades.EncriptarSHA256(clave))
                .FirstOrDefaultAsync();

            if (usuarioEncontrado == null)
            {
                return Unauthorized(new { isSuccess = false, token = "", message = "Usuario no autorizado." });
            }
            else
            {
                // Generar token y devolverlo junto con el éxito
                return Ok(new { isSuccess = true, token = _utilidades.GenerarToken(usuarioEncontrado), id_usuario = usuarioEncontrado.id_usuario, usuario = usuarioEncontrado.usuario, correo = usuarioEncontrado.correo });
            }
        }

        // POST: api/Acceso/Login
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDTO loginDIO)
        {
            var usuarioEncontrado = await _context.Usuarios
                .Where(u => u.correo == loginDIO.correo && u.clave == _utilidades.EncriptarSHA256(loginDIO.clave)).FirstOrDefaultAsync();

            if (usuarioEncontrado == null) { return Unauthorized(new { isSuccess = false, token = "" }); }
            else return Ok(new { isSuccess = true, token = _utilidades.GenerarToken(usuarioEncontrado), id_usuario = usuarioEncontrado.id_usuario, usuario = usuarioEncontrado.usuario, correo = usuarioEncontrado.correo });

        }

        // POST: api/Acceso/LoginDirecto
        [HttpPost("loginDirecto")]
        public async Task<IActionResult> LoginDirecto([FromBody] LoginDirectoDTO loginDirectoDTO)
        {
            var usuarioEncontrado = await _context.Usuarios
                .Where(u => u.correo == loginDirectoDTO.correo).FirstOrDefaultAsync();

            if (usuarioEncontrado == null) { return Unauthorized(new { exists = false, token = "", id_usuario = 0 }); }
            else return Ok(new { exists = true, token = _utilidades.GenerarToken(usuarioEncontrado), id_usuario = usuarioEncontrado.id_usuario});
        }


        [HttpPost("google-login")]
        public async Task<IActionResult> GoogleLogin([FromBody] LoginGoogleDTO googleLoginDTO)
        {
            try
            {
                var payload = await GoogleJsonWebSignature.ValidateAsync(googleLoginDTO.IdToken);
                var googleId = payload.Subject;

                var usuarioEncontrado = await _context.Usuarios
                    .Where(u => u.google_id == googleId).FirstOrDefaultAsync();

                if (usuarioEncontrado == null)
                {
                    return Unauthorized(new { isSuccess = false, message = "Usuario no encontrado" });
                }

                return Ok(new
                {
                    isSuccess = true,
                    token = _utilidades.GenerarToken(usuarioEncontrado),
                    id_usuario = usuarioEncontrado.id_usuario,
                    usuario = usuarioEncontrado.usuario,
                    correo = usuarioEncontrado.correo
                });
            }
            catch (InvalidJwtException)
            {
                return Unauthorized(new { isSuccess = false, message = "Token de Google inválido" });
            }
        }

        [HttpGet("github-login")]
        public async Task<IActionResult> GitHubCallback([FromQuery] string code)
        {
            if (string.IsNullOrEmpty(code))
            {
                return BadRequest(new { isSuccess = false, message = "Código de autorización no proporcionado" });
            }

            var clientId = "Ov23liyMUlVD4dfFzXMX";
            var clientSecret = "dfd55f63b076f0dcfb094f89efebccb97a98afc6";
            var tokenResponse = await GetGitHubAccessToken(code, clientId, clientSecret);

            if (tokenResponse == null || string.IsNullOrEmpty(tokenResponse.AccessToken))
            {
                return Unauthorized(new { isSuccess = false, message = "Token de GitHub inválido" });
            }

            var userResponse = await GetGitHubUser(tokenResponse.AccessToken);

            if (userResponse == null)
            {
                return Unauthorized(new { isSuccess = false, message = "No se pudo obtener la información del usuario de GitHub" });
            }

            var usuarioEncontrado = await _context.Usuarios
                .Where(u => u.github_id == userResponse.Id.ToString()).FirstOrDefaultAsync();

            if (usuarioEncontrado == null)
            {
                var nuevoUsuario = new Usuario
                {
                    usuario = userResponse.Login,
                    correo = userResponse.Email,
                    github_id = userResponse.Id.ToString(),
                    fecha_creacion = DateTime.Now,
                    url_foto_perfil = userResponse.AvatarUrl,
                    descripcion = userResponse.Bio
                };

                _context.Usuarios.Add(nuevoUsuario);
                await _context.SaveChangesAsync();

                usuarioEncontrado = nuevoUsuario;
            }

            return Ok(new
            {
                isSuccess = true,
                token = _utilidades.GenerarToken(usuarioEncontrado),
                id_usuario = usuarioEncontrado.id_usuario,
                usuario = usuarioEncontrado.usuario,
                correo = usuarioEncontrado.correo
            });
        }

        private async Task<GitHubTokenResponse> GetGitHubAccessToken(string code, string clientId, string clientSecret)
        {
            using var httpClient = new HttpClient();
            var requestData = new
            {
                client_id = clientId,
                client_secret = clientSecret,
                code = code
            };

            var content = new StringContent(JsonConvert.SerializeObject(requestData), Encoding.UTF8, "application/json");
            var response = await httpClient.PostAsync("https://github.com/login/oauth/access_token", content);

            var responseContent = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"GitHub Access Token Response: {responseContent}");

            if (!response.IsSuccessStatusCode)
            {
                return null;
            }

            var queryParams = System.Web.HttpUtility.ParseQueryString(responseContent);
            return new GitHubTokenResponse
            {
                AccessToken = queryParams["access_token"]
            };
        }

        private async Task<GitHubUserResponse> GetGitHubUser(string accessToken)
        {
            using var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);
            httpClient.DefaultRequestHeaders.UserAgent.Add(new System.Net.Http.Headers.ProductInfoHeaderValue("YourApp", "1.0"));

            var response = await httpClient.GetAsync("https://api.github.com/user");

            var responseContent = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"GitHub User Response: {responseContent}");

            if (!response.IsSuccessStatusCode)
            {
                return null;
            }

            return JsonConvert.DeserializeObject<GitHubUserResponse>(responseContent);
        }
    }

    public class GitHubLoginDTO
    {
        public string Code { get; set; }
    }

    public class GitHubTokenResponse
    {
        public string AccessToken { get; set; }
    }

    public class GitHubUserResponse
    {
        public int Id { get; set; }
        public string Login { get; set; }
        public string Email { get; set; }
        public string AvatarUrl { get; set; }
        public string Bio { get; set; }
    }
}