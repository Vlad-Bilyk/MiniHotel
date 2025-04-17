using MiniHotel.Application.DTOs;
using MiniHotel.Domain.Enums;

namespace MiniHotel.Application.Interfaces.IService
{
    public interface IPaymentService
    {
        Task<string> CreatePaymentUrlAsync(int invoiceId, string description);
        Task<InvoiceDto> PayOfflineAsync(int invoiceId);
        Task<InvoiceDto> MarkRefundAsync(int invoiceId);
        Task ProcessCallbackAsync(LiqPayCallbackDto dto);
        Task<InvoiceDto> AddPaymentAsync(int invoiceId, decimal amount, PaymentMethod method, string? externalId = null);
    }
}
