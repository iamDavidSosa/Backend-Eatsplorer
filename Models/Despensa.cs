using System.ComponentModel.DataAnnotations;

namespace PROYECTO_PRUEBA.Models
{
    public class Despensa
    {
       
        public int id_ususario { get; set; }
        public int id_ingrediente { get; set; }
        public int cantidad { get; set; }
        public DateTime fecha_agregado { get; set; }
    }
}
