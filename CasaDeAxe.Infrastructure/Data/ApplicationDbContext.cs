using Microsoft.EntityFrameworkCore;
using CasaDeAxe.Domain.Entities;

namespace CasaDeAxe.Infrastructure.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<StatusUsuario> StatusUsuarios { get; set; }
        public DbSet<Gira> Giras { get; set; }
        public DbSet<TextoPonto> TextoPontos { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Role>().HasData(
                new Role { Id = 1, Nome = "ADM", Ativo = true },
                new Role { Id = 2, Nome = "PaiDeSanto", Ativo = true },
                new Role { Id = 3, Nome = "Filho", Ativo = true },
                new Role { Id = 4, Nome = "Assistencia", Ativo = true }
            );

            modelBuilder.Entity<StatusUsuario>().HasData(
                new StatusUsuario { Id = 1, Nome = "Ativo" },
                new StatusUsuario { Id = 2, Nome = "Inativo" }
            );
        }

    }
}
