using System.ComponentModel.DataAnnotations;

namespace PROYECTO_PRUEBA.Models
{
    public class Dias
    {
        [Key]
        public int id_dias { get; set; }
        public int dias { get; set; }
    }
}
