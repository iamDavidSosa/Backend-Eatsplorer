using System.ComponentModel.DataAnnotations;

namespace PROYECTO_PRUEBA.Models
{
    public class Categoria
    {
        [Key]
        public int id_categoria { get; set; }
        public string nombre { get; set; }
    }
}
