using System.ComponentModel.DataAnnotations;

namespace MiniHotel.Domain.Entities
{
    public class Service
    {
        [Key]
        public int ServiceId { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; } = default!;

        [StringLength(500)]
        public string? Description { get; set; }

        [Range(0, 999999.99)]
        public decimal Price { get; set; }

        [Required]
        public bool IsAvailable { get; set; } = false;

        public ICollection<BookingService> BookingServices { get; set; } = new List<BookingService>();
    }
}
