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
        public DbSet<Payment> Payments { get; set; }

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

            builder.Entity<RoomType>()
                .HasIndex(rt => rt.RoomCategory)
                .IsUnique();

            builder.Entity<Payment>()
                .HasOne(p => p.Invoice)
                .WithMany(i => i.Payments)
                .HasForeignKey(p => p.InvoiceId)
                .OnDelete(DeleteBehavior.Cascade);

            // Indexes
            builder.Entity<Booking>()
                .HasIndex(b => new
                {
                    b.RoomId,
                    b.StartDate,
                    b.EndDate,
                    b.BookingStatus
                })
                .HasDatabaseName("IX_Booking_RoomId_StartDate_EndDate_Status");

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

            // Global convention: for all DateTime properties, use UTC conversion
            var dateTimeConverter = new ValueConverter<DateTime, DateTime>(
                toDb => DateTime.SpecifyKind(toDb, DateTimeKind.Utc),
                fromDb => DateTime.SpecifyKind(fromDb, DateTimeKind.Utc)
            );

            foreach (var entityType in builder.Model.GetEntityTypes())
            {
                var props = entityType.ClrType
                    .GetProperties()
                    .Where(p => p.PropertyType == typeof(DateTime));

                foreach (var prop in props)
                {
                    builder.Entity(entityType.Name)
                        .Property(prop.Name)
                        .HasConversion(dateTimeConverter);
                }
            }
        }
    }
}
