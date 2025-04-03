using MiniHotel.Application.DTOs;

namespace MiniHotel.Application.Interfaces.IService
{
    public interface IPaymentService
    {
        Task<string> CreatePaymentUrlAsync(int invoiceId, string description);
        Task<InvoiceDto> MarkPaidOfflineAsync(int invoiceId);
        Task<InvoiceDto> MarkRefundAsync(int invoiceId);
        Task ProcessCallbackAsync(LiqPayCallbackDto dto);
    }
}
