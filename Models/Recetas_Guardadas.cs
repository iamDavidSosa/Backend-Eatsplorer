using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.Blazor;

namespace PROYECTO_PRUEBA.Models
{
    public class Recetas_Guardadas
    {
        public int id_receta { get; set; }
        public int id_usuario { get; set; }
        public DateTime fecha_acceso { get; set; }

    }
}
