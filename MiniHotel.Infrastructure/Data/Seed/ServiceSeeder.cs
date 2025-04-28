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
                new Service
                {
                    Name = "Сніданок",
                    Description = "Континентальний сніданок: кава або чай, свіжі булочки з маслом та джемом, натуральний йогурт, сезонні фрукти.",
                    Price = 100m,
                    IsAvailable = true
                },
                new Service
                {
                    Name = "Парковка",
                    Description = "Охоронювана парковка на території готелю з цілодобовим відеоспостереженням для вашого автомобіля.",
                    Price = 80m,
                    IsAvailable = true
                },
                new Service
                {
                    Name = "Трансфер до аеропорту",
                    Description = "Зручний трансфер з готелю до найближчого аеропорту або залізничного вокзалу, включаючи допомогу з багажем.",
                    Price = 250m,
                    IsAvailable = true
                },
                new Service
                {
                    Name = "Пізній виїзд",
                    Description = "Можливість залишитися в номері до 16:00 за додаткову оплату (при наявності доступності).",
                    Price = 150m,
                    IsAvailable = true
                },
                new Service
                {
                    Name = "Прибирання на вимогу",
                    Description = "Додаткове прибирання номеру за запитом під час перебування, включаючи зміну рушників та постільної білизни.",
                    Price = 120m,
                    IsAvailable = true
                },
                new Service
                {
                    Name = "Оренда дитячого ліжечка",
                    Description = "Надання дитячого ліжечка для найменших гостей, із безпечною конструкцією та чистою білизною.",
                    Price = 70m,
                    IsAvailable = true
                }
            };

            _db.Services.AddRange(services);
            await _db.SaveChangesAsync();
        }
    }
}
