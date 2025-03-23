using MiniHotel.Application.DTOs;
using MiniHotel.Domain.Enums;

namespace MiniHotel.Application.Interfaces.IService
{
    public interface IInvoiceService
    {
        Task<IEnumerable<InvoiceDto>> GetAllInvoicesAsync();
        Task<InvoiceDto> GetInvoiceAsync(int bookingId);
        Task<InvoiceDto> CreateInvoiceForBookingAsync(int bookingId);
        Task<InvoiceDto> AddItemAsync (int bookingId, InvoiceItemCreateDto createItem);
        Task<InvoiceDto> UpdateStatusAsync(int invoiceId, InvoiceStatus status);
        Task RemoveItemAsync(int invoiceItemId);
    }
}
