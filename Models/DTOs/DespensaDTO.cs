namespace PROYECTO_PRUEBA.Models.DTOs
{
    public class DespensaDTO
    {
        public int id_ingrediente { get; set; }
        public double cantidad { get; set; }
        public DateTime fecha_agregado { get; set; }
    }

    public class ActualizarDespensaDTO
    {
        public int id_ingrediente { get; set; }
        public double cantidad { get; set; }
    }
}
