using CrudDashboard.Models;
using Microsoft.EntityFrameworkCore;

namespace CrudDashboard.Profiles
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<Conta> Contas { get; set; }
        public DbSet<Movimentacao> Movimentacoes { get; set; }
    }
}
