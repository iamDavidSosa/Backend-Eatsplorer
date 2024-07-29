namespace PROYECTO_PRUEBA.Models.DTOs
{
    public class ActualizacionUsuarioDTO
    {
        public string Usuario { get; set; }
        public string Correo { get; set; }
        public string Descripcion { get; set; }
        public string ?UrlFotoPerfil { get; set; }
        public string ?UrlFotoPortada { get; set; }
        public List<int> Gustos { get; set; } = new List<int>();
        public List<int> NoConsume { get; set; } = new List<int>();
        public List<int> Alergico { get; set; } = new List<int>();
    }
}
