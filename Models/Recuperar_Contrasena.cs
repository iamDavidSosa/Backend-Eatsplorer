using System.ComponentModel.DataAnnotations;

namespace PROYECTO_PRUEBA.Models
{
    public class Recuperar_Contrasena
    {
        [Key]
        public int IdRecuperarContrasena { get; set; }
        public int IdUsuario { get; set; }
        public int id_pregunta { get; set; }
        public string respuesta { get; set; }

    }
}
