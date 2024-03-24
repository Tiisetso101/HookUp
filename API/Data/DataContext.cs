using API.Entities;
using Microsoft.EntityFrameworkCore;

namespace API.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions options) : base(options)
        {

        }

        public DbSet<User> users { get; set; }

        public DbSet<UserLike> Likes { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<UserLike>()
            .HasKey(k => new { k.SourceUserID, k.TargetUserID });

            modelBuilder.Entity<UserLike>()
            .HasOne(s => s.SourceUser)
            .WithMany(l => l.LikedUsers)
            .HasForeignKey(s => s.SourceUserID)
            .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<UserLike>()
           .HasOne(t => t.TargetUser)
           .WithMany(l => l.LikedByUsers)
           .HasForeignKey(s => s.TargetUserID)
           .OnDelete(DeleteBehavior.Cascade);



        }


    }
}