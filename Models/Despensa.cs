using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PROYECTO_PRUEBA.Models
{
    public class Despensa
    {
        [Key]
        [Column(Order = 1)]
        public int id_usuario { get; set; }

        [Key]
        [Column(Order = 2)]
        public int id_ingrediente { get; set; }
        public double cantidad { get; set; }
        public DateTime fecha_agregado { get; set; }
    }
}
