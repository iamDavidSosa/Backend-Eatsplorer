using System.ComponentModel.DataAnnotations;

namespace PROYECTO_PRUEBA.Models
{
    public class Usuario
    {
        [Key]
        public int id_usuario { get; set; }
        public string correo { get; set; }
        public string usuario { get; set; }
        public string clave { get; set; }
        public int id_rol { get; set; }
        public DateTime fecha_creacion { get; set; }
    }
}
