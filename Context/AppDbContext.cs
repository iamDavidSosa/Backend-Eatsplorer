using Microsoft.EntityFrameworkCore;
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

        public DbSet<Detalles_Usuario> Detalles_Usuario { get; set; }

        //public DbSet<Despensa> Despensa { get; set; }

        public DbSet<Dias> Dias { get; set; }

        public DbSet<Tags> Tags { get; set; }

        public DbSet<Categoria> Categoria { get; set; }

        public DbSet<TagsCategorias> TagsCategorias { get; set; }

        public DbSet<RecetasTags> RecetasTags { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Recetas_Ingredientes>()
                .HasKey(ri => new { ri.id_receta, ri.id_ingrediente });

            modelBuilder.Entity<Recetas_Guardadas>()
                .HasKey(rg => new { rg.id_receta, rg.id_usuario });

            modelBuilder.Entity<Detalles_Usuario>()
                .HasKey(du => new { du.id_usuario, du.id_ingrediente });

            modelBuilder.Entity<TagsCategorias>()
                .HasKey(tc => new { tc.id_tag, tc.id_categoria });

            modelBuilder.Entity<RecetasTags>()
                .HasKey(rt => new { rt.id_receta, rt.id_tag });

            //modelBuilder.Entity<Despensa>()
            //.HasNoKey();

            base.OnModelCreating(modelBuilder);
        }

       
    }
}
