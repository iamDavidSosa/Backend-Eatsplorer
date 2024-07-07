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
    }
}
