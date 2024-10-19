using Team_12.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Team_12.DBContext
{
    public class Team12DbContext : IdentityDbContext<ApplicationUser>
    {
        public Team12DbContext(DbContextOptions<Team12DbContext> options) : base(options)
        {
            ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
            ChangeTracker.LazyLoadingEnabled = false;
        }

        public DbSet<Facility> Facilities { get; set; }
        public DbSet<Booking> Bookings { get; set; }
        public DbSet<Rating> Ratings { get; set; }
        public DbSet<ClientType> ClientTypes { get; set; }
        public DbSet<UserLoyalty> UserLoyalties { get; set; }
        public DbSet<QRVerificationModel> QRVerifications { get; set; }

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
                .WithMany()
                .HasForeignKey(r => r.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            // Booking - User (One-to-Many)
            modelBuilder.Entity<Booking>()
                .HasOne(b => b.User)
                .WithMany()
                .HasForeignKey(b => b.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            // Configure QRVerificationModel with composite key
            modelBuilder.Entity<QRVerificationModel>()
                .HasKey(qr => new { qr.BookingId, qr.FacilityId });

            // QRVerification - Booking (One-to-One)
            modelBuilder.Entity<QRVerificationModel>()
                .HasOne(qr => qr.Booking)
                .WithOne()
                .HasForeignKey<QRVerificationModel>(qr => qr.BookingId)
                .OnDelete(DeleteBehavior.Cascade);

            // QRVerification - Facility (Many-to-One)
            modelBuilder.Entity<QRVerificationModel>()
                .HasOne(qr => qr.Facility)
                .WithMany()
                .HasForeignKey(qr => qr.FacilityId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<UserLoyalty>()
                .HasOne(r => r.User)
                .WithMany()
                .HasForeignKey(r => r.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            // Configure ClientTypes to store List<string> as a comma-separated string
            var stringListConverter = new ValueConverter<List<string>, string>(
                v => string.Join(',', v),
                v => v.Split(',', StringSplitOptions.RemoveEmptyEntries).ToList()
            );

            modelBuilder.Entity<Booking>()
                .Property(b => b.ClientTypes)
                .HasConversion(stringListConverter);
        }
    }
}