using System.ComponentModel.DataAnnotations;

namespace PROYECTO_PRUEBA.Models
{
    public class Tags
    {
        [Key]
        public int id_tag { get; set; }
        public int nombre { get; set; }
    }
}
