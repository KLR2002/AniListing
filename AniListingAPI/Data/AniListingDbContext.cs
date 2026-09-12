using AniListingAPI.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace AniListingAPI.Data;

public class AniListingDbContext : DbContext
{
    public AniListingDbContext(DbContextOptions<AniListingDbContext> options)
        : base(options)
    {
    }

    public DbSet<User> Users => Set<User>();
    public DbSet<UserMediaItem> UserMediaItems => Set<UserMediaItem>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(u => u.Id);
            entity.HasIndex(u => u.Username).IsUnique();
            entity.Property(u => u.Username).IsRequired().HasMaxLength(50);
            entity.Property(u => u.PasswordHash).IsRequired();
        });

        modelBuilder.Entity<UserMediaItem>(entity =>
        {
            entity.HasKey(m => m.Id);
            entity.HasIndex(m => new { m.UserId, m.MalId, m.MediaType }).IsUnique();
            entity.Property(m => m.MediaType).IsRequired().HasMaxLength(20);
            entity.Property(m => m.Title).IsRequired().HasMaxLength(255);
            entity.Property(m => m.Status).IsRequired().HasMaxLength(30);

            entity.HasOne(m => m.User)
                .WithMany(u => u.MediaItems)
                .HasForeignKey(m => m.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }
}
