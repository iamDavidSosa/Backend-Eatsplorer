using System.ComponentModel.DataAnnotations;

namespace PROYECTO_PRUEBA.Models
{
    public class Recetas_Ingredientes
    {
        public int id_receta { get; set; }

        public Recetas Recetas { get; set; }
        public string id_ingrediente { get; set; }

        public Ingredientes Ingrediente { get; set; }
    }
}
