using Microsoft.EntityFrameworkCore;
using MiniHotel.Domain.Entities;

namespace MiniHotel.Infrastructure.Data
{
    public class MiniHotelDbContext : DbContext
    {
        public MiniHotelDbContext(DbContextOptions<MiniHotelDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Room> Rooms { get; set; }
        public DbSet<Service> Services { get; set; }
        public DbSet<Booking> Bookings { get; set; }
        public DbSet<BookingService> BookingServices { get; set; }
        public DbSet<Payment> Payments { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<BookingService>()
                .HasKey(bs => new { bs.BookingId, bs.ServiceId });

            modelBuilder.Entity<Booking>()
                .ToTable(t => t.HasCheckConstraint("CK_Booking_EndDate", "\"EndDate\" >= \"StartDate\""));
        }
    }
}
