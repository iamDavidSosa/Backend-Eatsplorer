using System.ComponentModel.DataAnnotations;

namespace PROYECTO_PRUEBA.Models
{
    public class Categoria
    {
        [Key]
        public int id_categoria { get; set; }
        public int nombre { get; set; }
    }
}
