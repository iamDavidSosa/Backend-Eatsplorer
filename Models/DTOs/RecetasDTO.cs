namespace PROYECTO_PRUEBA.Models.DTOs
{
    public class RecetasDTO
    {
        public string titulo { get; set; }
    }

    public class RecetasEliminarDTO
    {
        public int id_receta { get; set; }
    }

    public class RecetasActualizarDTO
    {
        public int id_receta { get; set; }
        public string titulo { get; set; }
        public string descripcion { get; set; }
        public string instrucciones { get; set; }
        public string foto_receta { get; set; }
        public string? porciones { get; set; }
    }

    public class RecetaGuardadasDTO
    {
        public int id_receta { get; set; }
        public string titulo { get; set; }
        public string descripcion { get; set; }
        public string instrucciones { get; set; }
        public string foto_receta { get; set; }
        public string? porciones { get; set; }
        public List<string> Ingredientes { get; set; } = new List<string>();
    }

}
