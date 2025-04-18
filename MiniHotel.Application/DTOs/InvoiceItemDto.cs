using MiniHotel.Domain.Enums;

namespace MiniHotel.Application.DTOs
{
    public class InvoiceItemDto
    {
        public int InvoiceItemId { get; set; }
        public string? Description { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public string? ServiceName { get; set; } = string.Empty;
        public InvoiceItemType InvoiceItemType { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool IsLocked { get; set; }
        public decimal LineTotal => Quantity * UnitPrice;
    }
}
