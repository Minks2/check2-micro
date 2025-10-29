using GestaoEstoque.Models;
using Microsoft.EntityFrameworkCore;

namespace GestaoEstoque.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<Produto> Produtos { get; set; }
        public DbSet<MovimentacaoEstoque> Movimentacoes { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Garante que o Enum seja salvo como string no banco
            modelBuilder.Entity<Produto>()
                .Property(p => p.Categoria)
                .HasConversion<string>();

            modelBuilder.Entity<MovimentacaoEstoque>()
                .Property(m => m.Tipo)
                .HasConversion<string>();
        }
    }
}