using Cozy.Models;
using Microsoft.EntityFrameworkCore;

namespace Cozy.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        // DbSets for your models
        public DbSet<User> Users { get; set; }
        public DbSet<Hotel> Hotels { get; set; }
        public DbSet<Room> Rooms { get; set; }
        public DbSet<Booking> Bookings { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<Review> Reviews { get; set; }
        public DbSet<Cancellation> Cancellations { get; set; }

        // Overriding OnModelCreating to define relationships or configurations
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // User entity configuration
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(u => u.UserID);
                entity.Property(u => u.Email).IsRequired();
            });

            // Hotel entity configuration
            modelBuilder.Entity<Hotel>(entity =>
            {
                entity.HasKey(h => h.HotelID);
                entity.Property(h => h.Name).IsRequired().HasMaxLength(100);

                entity.HasMany(h => h.Rooms)
                      .WithOne(r => r.Hotel)
                      .HasForeignKey(r => r.HotelID)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasMany(h => h.Reviews)
                      .WithOne(r => r.Hotel)
                      .HasForeignKey(r => r.HotelID)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // Room entity configuration
            modelBuilder.Entity<Room>(entity =>
            {
                entity.HasKey(r => r.RoomID);
                entity.Property(r => r.RoomSize).IsRequired();
                entity.Property(r => r.BedType).IsRequired();
            });

            // Booking entity configuration
            modelBuilder.Entity<Booking>(entity =>
            {
                entity.HasKey(b => b.BookingID);
                entity.HasOne(b => b.User)
                      .WithMany(u => u.Bookings)
                      .HasForeignKey(b => b.UserID);
                entity.HasOne(b => b.Room)
                      .WithMany(r => r.Bookings)
                      .HasForeignKey(b => b.RoomID);
            });

            // Payment entity configuration
            modelBuilder.Entity<Payment>(entity =>
            {
                entity.HasKey(p => p.PaymentID);
                entity.HasOne(p => p.Booking)
                      .WithOne(b => b.Payment)
                      .HasForeignKey<Payment>(p => p.BookingID);
            });

            // Review entity configuration
            modelBuilder.Entity<Review>(entity =>
            {
                entity.HasKey(r => r.ReviewID);
                entity.HasOne(r => r.User)
                      .WithMany(u => u.Reviews)
                      .HasForeignKey(r => r.UserID);
                entity.HasOne(r => r.Hotel)
                      .WithMany(h => h.Reviews)
                      .HasForeignKey(r => r.HotelID);
            });

            // Cancellation entity configuration
            modelBuilder.Entity<Cancellation>(entity =>
            {
                entity.HasKey(c => c.CancellationID);
                entity.HasOne(c => c.Booking)
                      .WithMany(b => b.Cancellations)
                      .HasForeignKey(c => c.BookingID);
            });
        }
    }
}
