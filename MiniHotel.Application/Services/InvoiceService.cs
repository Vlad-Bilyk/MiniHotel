using AutoMapper;
using MiniHotel.Application.DTOs;
using MiniHotel.Application.Interfaces.IRepository;
using MiniHotel.Application.Interfaces.IService;
using MiniHotel.Domain.Entities;
using MiniHotel.Domain.Enums;

namespace MiniHotel.Application.Services
{
    public class InvoiceService : IInvoiceService
    {
        private readonly IInvoiceRepository _invoiceRepository;
        private readonly IBookingRepository _bookingRepository;
        private readonly IMapper _mapper;

        public InvoiceService(IInvoiceRepository invoiceRepository, IBookingRepository bookingRepository, IMapper mapper)
        {
            _invoiceRepository = invoiceRepository;
            _bookingRepository = bookingRepository;
            _mapper = mapper;
        }

        public async Task<InvoiceDto> AddItemAsync(int bookingId, InvoiceItemCreateDto createItem)
        {
            var invoice = await _invoiceRepository.GetByBookingIdAsync(bookingId)
                          ?? throw new KeyNotFoundException("Invoice not found");

            var item = new InvoiceItem
            {
                InvoiceId = invoice.InvoiceId,
                Description = createItem.Description,
                Quantity = createItem.Quantity,
                UnitPrice = createItem.UnitPrice,
                ServiceId = createItem.ServiceId
            };

            var updatedInvoice = await _invoiceRepository.AddItemAsync(item);
            return _mapper.Map<InvoiceDto>(updatedInvoice);
        }

        public async Task<InvoiceDto> CreateInvoiceForBookingAsync(int bookingId)
        {
            var booking = await _bookingRepository.GetAsync(b => b.BookingId == bookingId, includeProperties: "Room,Room.RoomType")
                          ?? throw new KeyNotFoundException("Booking not found");

            var nights = (booking.EndDate.Date - booking.StartDate.Date).Days;

            var invoice = new Invoice
            {
                BookingId = bookingId,
                CreatedAt = DateTime.UtcNow,
                Status = InvoiceStatus.Pending,
                InvoiceItems = new List<InvoiceItem>
                {
                    new InvoiceItem
                    {
                        Description = $"Бронюванян номеру {booking.Room.RoomNumber} - {nights} ночей",
                        Quantity = nights,
                        UnitPrice = booking.Room.RoomType.PricePerNight,
                        ServiceId = 1
                    }
                }
            };

            await _invoiceRepository.CreateAsync(invoice);
            return _mapper.Map<InvoiceDto>(invoice);
        }

        public async Task<IEnumerable<InvoiceDto>> GetAllInvoices()
        {
            var includeProp = "InvoiceItems";
            var invoices = await _invoiceRepository.GetAllAsync(includeProperties: includeProp);
            return _mapper.Map<IEnumerable<InvoiceDto>>(invoices);
        }

        public async Task<InvoiceDto> GetInvoiceAsync(int bookingId)
        {
            var invoice = await _invoiceRepository.GetByBookingIdAsync(bookingId)
                          ?? throw new KeyNotFoundException("Invoice not found");
            return _mapper.Map<InvoiceDto>(invoice);
        }

        public async Task RemoveItemAsync(int invoiceItemId)
        {
            await _invoiceRepository.RemoveItemAsync(invoiceItemId);
        }

        public async Task<InvoiceDto> UpdateStatusAsync(int invoiceId, InvoiceStatus status)
        {
            var invoice = await _invoiceRepository.GetAsync(i => i.InvoiceId == invoiceId)
                          ?? throw new KeyNotFoundException("Invoice not found");
            invoice.Status = status;
            await _invoiceRepository.UpdateAsync(invoice);
            return _mapper.Map<InvoiceDto>(invoice);
        }
    }
}
