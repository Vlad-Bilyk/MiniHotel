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

        public DbSet<HotelUser> HotelUsers { get; set; }
        public DbSet<Room> Rooms { get; set; }
        public DbSet<RoomType> RoomTypes { get; set; }
        public DbSet<Service> Services { get; set; }
        public DbSet<Booking> Bookings { get; set; }
        public DbSet<Invoice> Invoices { get; set; }
        public DbSet<InvoiceItem> InvoiceItems { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Booking>()
                .ToTable(t => t.HasCheckConstraint("CK_Booking_EndDate", "\"EndDate\" >= \"StartDate\""));

            builder.Entity<Booking>()
                .HasOne(b => b.Invoice)
                .WithOne(i => i.Booking)
                .HasForeignKey<Invoice>(i => i.BookingId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Booking>()
                .HasOne(b => b.User)
                .WithMany(u => u.Bookings)
                .HasForeignKey(b => b.UserId)
                .OnDelete(DeleteBehavior.SetNull);

            builder.Entity<HotelUser>()
                .HasOne<ApplicationUser>()
                .WithOne()
                .HasForeignKey<HotelUser>(u => u.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Room>()
                .HasIndex(r => r.RoomNumber)
                .IsUnique();

            builder.Entity<Invoice>()
                .HasIndex(i => i.BookingId)
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
