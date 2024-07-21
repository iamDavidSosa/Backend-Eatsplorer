using System.ComponentModel.DataAnnotations;

namespace PROYECTO_PRUEBA.Models
{
    public class Recetas_Ingredientes
    {
        [Key]
        public int id_receta { get; set; }

        //   public Recetas Recetas { get; set; }

        public int id_ingrediente { get; set; }

      //  public Ingredientes Ingrediente { get; set; }
    }
}
