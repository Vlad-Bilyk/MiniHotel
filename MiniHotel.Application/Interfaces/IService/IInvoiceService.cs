namespace MiniHotel.Application.Interfaces.IService
{
    public interface IInvoiceService
    {
        Task<decimal> CalculateFinalInvoiceAsync(int bookingId);
    }
}
