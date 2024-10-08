﻿using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.Blazor;
using System.ComponentModel.DataAnnotations;

namespace PROYECTO_PRUEBA.Models
{
    public class Recetas
    {
        [Key]    
        public int id_receta { get; set; }
        public string titulo { get; set; }
        public string descripcion { get; set; }
        public string instrucciones { get; set; }
        public string foto_receta { get; set; }
        public int usuario_id { get; set; }
        public DateTime fecha_creacion { get; set; }
        public string ?porciones { get; set; }
        public int ?likes { get; set; }
    }
}
