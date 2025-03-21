namespace MiniHotel.Application.DTOs
{
    public class InvoiceItemCreateDto
    {
        public int? ServiceId { get; set; }
        public string Description { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
    }
}
