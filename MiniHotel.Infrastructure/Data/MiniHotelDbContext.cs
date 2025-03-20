using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using MiniHotel.Domain.Entities;
using MiniHotel.Infrastructure.Identity;

namespace MiniHotel.Infrastructure.Data
{
    public class MiniHotelDbContext : IdentityDbContext<ApplicationUser, IdentityRole, string>
    {
        public MiniHotelDbContext(DbContextOptions<MiniHotelDbContext> options) : base(options)
        {
        }

        public DbSet<User> HotelUsers { get; set; }
        public DbSet<Room> Rooms { get; set; }
        public DbSet<Service> Services { get; set; }
        public DbSet<Booking> Bookings { get; set; }
        public DbSet<BookingService> BookingServices { get; set; }
        public DbSet<Payment> Payments { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<BookingService>()
                .HasKey(bs => new { bs.BookingId, bs.ServiceId });

            builder.Entity<Booking>()
                .ToTable(t => t.HasCheckConstraint("CK_Booking_EndDate", "\"EndDate\" >= \"StartDate\""));

            builder.Entity<User>()
                .HasOne<ApplicationUser>()
                .WithOne()
                .HasForeignKey<User>(u => u.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Room>()
                .HasIndex(r => r.RoomNumber)
                .IsUnique();

            // Global convention: for all enum properties, use EnumToStringConverter
            foreach (var entityType in builder.Model.GetEntityTypes())
            {
                foreach (var property in entityType.GetProperties())
                {
                    if (property.ClrType.IsEnum)
                    {
                        var converterType = typeof(EnumToStringConverter<>).MakeGenericType(property.ClrType);
                        var converter = (ValueConverter)Activator.CreateInstance(converterType)!;
                        property.SetValueConverter(converter);
                    }
                }
            }
        }
    }
}
