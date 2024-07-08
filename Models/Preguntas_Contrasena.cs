using System.ComponentModel.DataAnnotations;

namespace PROYECTO_PRUEBA.Models
{
    public class Preguntas_Contrasena
    {
        [Key]
        public int IdPreguntasContrasena { get; set; }
        public string Pregunta { get; set; }
    }
}
