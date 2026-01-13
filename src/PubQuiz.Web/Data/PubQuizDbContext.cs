using Microsoft.EntityFrameworkCore;
using PubQuiz.Web.Models;

namespace PubQuiz.Web.Data;

public class PubQuizDbContext(DbContextOptions<PubQuizDbContext> options) : DbContext(options)
{
    public DbSet<Game> Games => Set<Game>();
    public DbSet<Team> Teams => Set<Team>();
    public DbSet<Question> Questions => Set<Question>();
    public DbSet<Answer> Answers => Set<Answer>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Game>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.Code).IsUnique();
            entity.Property(e => e.Code).HasMaxLength(6);
            entity.Property(e => e.HostPasswordHash).HasMaxLength(100);
        });

        modelBuilder.Entity<Team>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasOne(e => e.Game)
                .WithMany(g => g.Teams)
                .HasForeignKey(e => e.GameId)
                .OnDelete(DeleteBehavior.Cascade);
            entity.Property(e => e.Name).HasMaxLength(50);
        });

        modelBuilder.Entity<Question>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Text).HasMaxLength(500);
            entity.Property(e => e.Options).HasColumnType("jsonb");
        });

        modelBuilder.Entity<Answer>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasOne(e => e.Team)
                .WithMany(t => t.Answers)
                .HasForeignKey(e => e.TeamId)
                .OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(e => e.Game)
                .WithMany(g => g.Answers)
                .HasForeignKey(e => e.GameId)
                .OnDelete(DeleteBehavior.Cascade);
            entity.HasIndex(e => new { e.GameId, e.TeamId, e.QuestionIndex }).IsUnique();
        });
    }
}
