namespace PROYECTO_PRUEBA.Models.DTOs
{
    public class UsuarioUpdateDTO
    {
        public string? Correo { get; set; }
        public string? Usuario { get; set; }
        public string? Clave { get; set; }
        public string? UrlFotoPerfil { get; set; }
        public string? Descripcion { get; set; }
        public string? UrlFotoPortada { get; set; }
        public int? CantRecetasGuardadas { get; set; }
        public string? GoogleId { get; set; }
        public string? GithubId { get; set; }
    }
}
