using Microsoft.EntityFrameworkCore;
using PROYECTO_PRUEBA.Models;

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

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Configura la clave primaria compuesta para la tabla intermedia Recetas_Ingredientes
            modelBuilder.Entity<Recetas_Ingredientes>()
                .HasKey(ri => new { ri.id_receta, ri.id_ingrediente });

            // Configura la relación muchos a muchos entre Receta y Ingrediente a través de Recetas_Ingredientes
           /* modelBuilder.Entity<Recetas_Ingredientes>()
                .HasOne(ri => ri.Receta)
                .WithMany(r => r.Recetas_Ingredientes)
                .HasForeignKey(ri => ri.RecetaId);


            modelBuilder.Entity<Recetas_Ingredientes>()
                .HasOne(ri => ri.Ingrediente)
                .WithMany(i => i.Recetas_Ingredientes)
                .HasForeignKey(ri => ri.IngredienteId);*/
        }
    }
}
