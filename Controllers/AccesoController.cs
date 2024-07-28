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

            if (usuario.id_usuario != 0)
            {
                return await LoginAfterRegistration(usuarioDTO.correo, usuarioDTO.clave);
            }
            else
            {
                return BadRequest(new { isSuccess = false });
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
            else return Ok(new { exists = true, token = _utilidades.GenerarToken(usuarioEncontrado), id_usuario = usuarioEncontrado.id_usuario });
        }

        // POST: api/Acceso/google-login
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
        // GET: api/Acceso/github-login
        [HttpGet("github-login")]
        public IActionResult GitHubLogin()
        {
            var clientId = "Ov23liG2JWBOhTxJfG8B";
            var redirectUri = "https://api-eat.azurewebsites.net/api/Acceso/github-callback";
            var githubUrl = $"https://github.com/login/oauth/authorize?client_id={clientId}&scope=user:email&redirect_uri={redirectUri}";
            return Redirect(githubUrl);
        }

        //var clientId = "Ov23liyMUlVD4dfFzXMX";
        //var clientSecret = "4437c5513ebdd6a5ebf83cb8f6f54c6c6a670a08";
        
        // POST: api/Acceso/github-callback
        [HttpPost("github-callback")]
        public async Task<IActionResult> GitHubCallback([FromBody] LoginGithubDTO loginGithubDTO)
        {
            if (string.IsNullOrEmpty(loginGithubDTO.Code))
            {
                return BadRequest(new { isSuccess = false, message = "Código de autorización no proporcionado" });
            }

            var tokenResponse = await GetGitHubAccessToken(loginGithubDTO.Code);

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
                    id_rol = 2,
                    clave = "",
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
                correo = usuarioEncontrado.correo,
                url_foto_perfil = usuarioEncontrado.url_foto_perfil
            });
        }


        private async Task<GitHubTokenResponse> GetGitHubAccessToken(string code)
        {
            using var httpClient = new HttpClient();
            var request = new HttpRequestMessage(HttpMethod.Post, "https://github.com/login/oauth/access_token");
            request.Headers.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

            var clientId = "Ov23liG2JWBOhTxJfG8B";
            var clientSecret = "353e5180132be310353e348e34408ebb0a09d286";
            var requestData = new
            {
                client_id = clientId,
                client_secret = clientSecret,
                code = code
            };

            request.Content = new StringContent(JsonConvert.SerializeObject(requestData), Encoding.UTF8, "application/json");

            var response = await httpClient.SendAsync(request);

            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine($"Error al obtener el token: {response.ReasonPhrase}");
                return null;
            }

            var responseContent = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<GitHubTokenResponse>(responseContent);
        }

        private async Task<GitHubUserResponse> GetGitHubUser(string accessToken)
        {
            using var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);
            httpClient.DefaultRequestHeaders.UserAgent.Add(new System.Net.Http.Headers.ProductInfoHeaderValue("YourApp", "1.0"));

            // Obtener la información del usuario
            var userResponse = await httpClient.GetAsync("https://api.github.com/user");
            userResponse.EnsureSuccessStatusCode();
            var userData = JsonConvert.DeserializeObject<GitHubUserResponse>(await userResponse.Content.ReadAsStringAsync());

            // Obtener el correo electrónico si no está disponible en la respuesta inicial
            if (string.IsNullOrEmpty(userData.Email))
            {
                var emailsResponse = await httpClient.GetAsync("https://api.github.com/user/emails");
                emailsResponse.EnsureSuccessStatusCode();
                var emailsData = JsonConvert.DeserializeObject<List<GitHubEmailResponse>>(await emailsResponse.Content.ReadAsStringAsync());
                var primaryEmail = emailsData.FirstOrDefault(email => email.Primary && email.Verified);

                if (primaryEmail != null)
                {
                    userData.Email = primaryEmail.Email;
                }
            }

            return userData;
        }
    }

    public class GitHubTokenResponse
    {
        [JsonProperty("access_token")]
        public string AccessToken { get; set; }
    }

    public class GitHubUserResponse
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("login")]
        public string Login { get; set; }

        [JsonProperty("avatar_url")]
        public string AvatarUrl { get; set; }

        [JsonProperty("bio")]
        public string Bio { get; set; }

        [JsonProperty("email")]
        public string Email { get; set; }
    }

    public class GitHubEmailResponse
    {
        public string Email { get; set; }
        public bool Primary { get; set; }
        public bool Verified { get; set; }
    }

}
