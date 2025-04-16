using MiniHotel.Application.Interfaces;
using MiniHotel.Domain.Entities;

namespace MiniHotel.Infrastructure.Data.Seed
{
    public class ServiceSeeder : ISeeder
    {
        private readonly MiniHotelDbContext _db;

        public ServiceSeeder(MiniHotelDbContext db)
        {
            _db = db;
        }

        public async Task SeedAsync()
        {
            if (_db.Services.Any()) return;

            var services = new[]
            {
                new Service { Name = "Бронювання", Description = "Бронювання номеру", Price = 0, IsAvailable = true }, // default service dont delete it
                new Service { Name = "Сніданок", Description = "Континентальний сніданок: кава або чай, свіжі булочки з маслом та джемом, натуральний йогурт, сезонні фрукти.", Price = 100, IsAvailable = true },
                new Service { Name = "Доступ в спа", Description = "Повний спа-центр", Price = 300, IsAvailable = false },
                new Service { Name = "Трансфер до аеропорту", Description = "Таксі до аеропорту", Price = 200, IsAvailable = true },
                new Service { Name = "Прання", Description = "Прання та прасування одягу з доставкою в номер", Price = 150, IsAvailable = true }
            };

            _db.Services.AddRange(services);
            await _db.SaveChangesAsync();
        }
    }
}
