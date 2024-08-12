namespace PROYECTO_PRUEBA.Models.DTOs
{
    public class TagsDTO
    {
        public List<string> NombresTags { get; set; }
    }

    public class TagsEliminarDTO
    {
        public int id_tag { get; set; }
    }
}
