using System.ComponentModel.DataAnnotations;

namespace PROYECTO_PRUEBA.Models
{
    public class Recuperar_Contrasena
    {
        [Key]
        public int IdRecuperarContrasena { get; set; }
        public int IdUsuario { get; set; }
        public string Pregunta1 { get; set; }
        public string Respuesta1 { get; set; }
        public string Pregunta2 { get; set; }
        public string Respuesta2 { get; set; }
        public string Pregunta3 { get; set; }
        public string Respuesta3 { get; set; }

    }
}
