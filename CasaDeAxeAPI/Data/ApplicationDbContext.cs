using CasaDeAxeAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace CasaDeAxeAPI.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Gira> Giras { get; set; }
        public DbSet<TextoPonto> TextoPonto { get; set; }
    }
}
