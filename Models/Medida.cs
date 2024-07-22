using System.ComponentModel.DataAnnotations;

namespace PROYECTO_PRUEBA.Models
{
    public class Medida
    {
        [Key]
        public int id_medida { get; set; }
        public string medida { get; set; }
    }
}
