using Microsoft.EntityFrameworkCore;
using MiniHotel.Application.Interfaces;
using MiniHotel.Domain.Entities;

namespace MiniHotel.Infrastructure.Data.Seed
{
    public class RoomTypeSeeder : ISeeder
    {
        private readonly MiniHotelDbContext _db;

        public RoomTypeSeeder(MiniHotelDbContext db)
        {
            _db = db;
        }

        public async Task SeedAsync()
        {
            if (await _db.RoomTypes.AnyAsync()) return;

            var types = new[]
            {
                new RoomType
                {
                    RoomCategory = "Одномісний",
                    PricePerNight = 500m,
                    Description = "Невеликий затишний номер для одного гостя, обладнаний односпальним ліжком, невеликим письмовим столом, телевізором і ванною кімнатою з душем. Ідеально підходить для короткотривалого перебування під час ділових поїздок чи подорожей."
                },
                new RoomType
                {
                    RoomCategory = "Двомісний",
                    PricePerNight = 750m,
                    Description = "Комфортний номер для двох гостей із двоспальним або двома окремими ліжками. У номері є телевізор, шафа для одягу, кондиціонер та окрема ванна кімната з душем. Підійде для пар або друзів, що подорожують разом."
                },
                new RoomType
                {
                    RoomCategory = "Сімейний",
                    PricePerNight = 1000m,
                    Description = "Просторий номер для сім'ї, обладнаний двоспальним та одним або двома односпальними ліжками. В номері є обідній стіл, телевізор, міні-холодильник та ванна кімната з душовою кабіною. Ідеальне рішення для комфортного відпочинку всією родиною."
                },
                new RoomType
                {
                    RoomCategory = "Люкс",
                    PricePerNight = 1500m,
                    Description = "Покращений номер із підвищеним комфортом. Складається зі спальні та невеликої зони відпочинку. Оснащений великим двоспальним ліжком, телевізором, міні-баром, чайником і ванною кімнатою з душем. Добре підходить для гостей, які шукають більше простору."
                }
            };

            _db.RoomTypes.AddRange(types);
            await _db.SaveChangesAsync();
        }
    }
}
