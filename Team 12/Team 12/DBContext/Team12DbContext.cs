using Team_12.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Team_12.DBContext
{
    public class Team12DbContext: IdentityDbContext<ApplicationUser>
    {
        public Team12DbContext(DbContextOptions<Team12DbContext> options ) : base(options)
        {
            ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
            ChangeTracker.LazyLoadingEnabled = false;
        }

        public DbSet<Facility> Facilities { get; set; }
        public DbSet<Booking> Bookings { get; set; }
        public DbSet<Rating> Ratings { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
            optionsBuilder.EnableSensitiveDataLogging();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Facility - Rating (One-to-Many)
            modelBuilder.Entity<Facility>()
                .HasMany(f => f.Ratings)
                .WithOne(r => r.Facility)
                .HasForeignKey(r => r.FacilityId)
                .OnDelete(DeleteBehavior.Cascade);

            // Facility - Booking (One-to-Many)
            modelBuilder.Entity<Facility>()
                .HasMany(f => f.Bookings)
                .WithOne(b => b.Facility)
                .HasForeignKey(b => b.FacilityId)
                .OnDelete(DeleteBehavior.Cascade);

            // Rating - User (One-to-Many)
            modelBuilder.Entity<Rating>()
                .HasOne(r => r.User)
                .WithMany() // Assuming users can rate many facilities
                .HasForeignKey(r => r.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            // Booking - client (One-to-Many)
            modelBuilder.Entity<Booking>()
                .HasOne(b => b.User)
                .WithMany() // Assuming a user can book multiple facilities
                .HasForeignKey(b => b.UserId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
