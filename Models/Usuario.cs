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
        public string ?url_foto_perfil { get; set; }
        public string ?descripcion { get; set; }
        public string ?url_foto_portada { get; set; }
        public int ?cant_recetas_guardadas { get; set; }

        public string ?google_id { get; set; }
    }
}
