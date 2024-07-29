﻿using Microsoft.EntityFrameworkCore;
using PROYECTO_PRUEBA.Models;
using PROYECTO_PRUEBA.Models.DTOs;

namespace PROYECTO_PRUEBA.Context
{
    public class AppDbContext:DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Usuario> Usuarios { get; set; }

        public DbSet<Recuperar_Contrasena> Recuperar_Contrasena { get; set; }

        public DbSet<Preguntas_Contrasena> Preguntas_Contrasena { get; set; }

        public DbSet<Recetas> Recetas { get; set; }

        public DbSet<Ingredientes> Ingredientes { get; set; }

        public DbSet<Recetas_Ingredientes> Recetas_Ingredientes { get; set; }

        public DbSet<Recetas_Guardadas> Recetas_Guardadas { get; set; }

        public DbSet<Medida> Medida { get; set; }

        public DbSet<Detalles_Usuario> Detalles_Usuario { get; set; }

       // public DbSet<Despensa> Despensa { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Recetas_Ingredientes>()
                .HasKey(ri => new { ri.id_receta, ri.id_ingrediente });

            modelBuilder.Entity<Recetas_Guardadas>()
                .HasKey(rg => new { rg.id_receta, rg.id_usuario });

            modelBuilder.Entity<Detalles_Usuario>()
                .HasKey(du => new { du.id_usuario, du.id_ingrediente });

            //modelBuilder.Entity<Despensa>()
                //.HasNoKey();

            base.OnModelCreating(modelBuilder);
        }

        /*  protected override void OnModelCreating(ModelBuilder modelBuilder)
          {
              // Configura la clave primaria compuesta para la tabla intermedia Recetas_Ingredientes
              modelBuilder.Entity<Recetas_Ingredientes>()
                  .HasKey(ri => new { ri.id_receta, ri.id_ingrediente });

              // Configura la relación muchos a muchos entre Receta y Ingrediente a través de Recetas_Ingredientes
              modelBuilder.Entity<Recetas_Ingredientes>()
                  .HasOne(ri => ri.Recetas)
                  .WithMany(r => r.Recetas_Ingredientes)
                  .HasForeignKey(ri => ri.id_receta);


              modelBuilder.Entity<Recetas_Ingredientes>()
                  .HasOne(ri => ri.Ingrediente)
                  .WithMany(i => i.Recetas_Ingredientes)
                  .HasForeignKey(ri => ri.id_ingrediente);
          }*/
    }
}
