using MiniHotel.Application.DTOs;
using MiniHotel.Domain.Enums;

namespace MiniHotel.Application.Interfaces.IService
{
    public interface IInvoiceService
    {
        Task<IEnumerable<InvoiceDto>> GetAllInvoicesAsync();
        Task<InvoiceDto> GetInvoiceByBookingIdAsync(int bookingId);
        Task<InvoiceDto> CreateInvoiceForBookingAsync(int bookingId);
        Task<InvoiceDto> AddItemAsync(int invoiceId, InvoiceItemCreateDto createItem);
        Task<InvoiceDto> UpdateStatusAsync(int invoiceId, InvoiceStatus status);
        Task RemoveItemAsync(int invoiceItemId);
        Task RecalculateAsync(int invoiceId);
        Task UpdateBookingTypeItemAsync(int bookingId);
    }
}
