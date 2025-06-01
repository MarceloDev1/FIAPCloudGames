using FIAPCloudGames.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace FIAPCloudGames.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<Game> Games { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();

            // Configuração do Game
            modelBuilder.Entity<Game>(entity =>
            {
                // Configuração do nome
                entity.Property(g => g.Name)
                    .IsRequired()
                    .HasMaxLength(100);

                // Configuração da descrição
                entity.Property(g => g.Description)
                    .IsRequired()
                    .HasMaxLength(500);

                // Configuração do preço
                entity.Property(g => g.Price)
                    .HasColumnType("decimal(18,2)")
                    .IsRequired();

                // Configuração do relacionamento com User
                entity.HasOne(g => g.CreatedBy)
                    .WithMany(u => u.GamesCreated)
                    .HasForeignKey(g => g.CreatedById)
                    .OnDelete(DeleteBehavior.Restrict); // Ou Cascade se preferir
            });
        }
    }
}