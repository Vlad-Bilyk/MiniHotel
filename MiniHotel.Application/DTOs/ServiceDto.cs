namespace MiniHotel.Application.DTOs
{
    public class ServiceDto
    {
        public int ServiceId { get; set; }
        public string Name { get; set; } = default!;
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public bool IsAvailable { get; set; } = false;
    }
}
