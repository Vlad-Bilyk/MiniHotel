namespace MiniHotel.Application.DTOs
{
    public class InvoiceItemDto
    {
        public int InvoiceItemId { get; set; }
        public string? Description { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }

        public string? ServiceName { get; set; }
        public decimal LineTotal => Quantity * UnitPrice;
    }
}
