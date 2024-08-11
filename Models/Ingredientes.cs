﻿using System.ComponentModel.DataAnnotations;

namespace PROYECTO_PRUEBA.Models
{
    public class Ingredientes
    {
        [Key]
        public int id_ingrediente { get; set; }
        public string nombre { get; set; }
        public string medida { get; set; }
        public int dias_vencimiento { get; set; }
    }
}
