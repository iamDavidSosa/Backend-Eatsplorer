﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PROYECTO_PRUEBA.Models
{
    public class Recetas_Ingredientes
    {
        public int id_receta { get; set; }

        //   public Recetas Recetas { get; set; }

        public int id_ingrediente { get; set; }

      //  public Ingredientes Ingrediente { get; set; }
    }
}
