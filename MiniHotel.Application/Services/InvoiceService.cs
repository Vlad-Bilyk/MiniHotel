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
        private const string IncludeProperties = "InvoiceItems,InvoiceItems.Service,Payments";

        private readonly IInvoiceRepository _invoiceRepository;
        private readonly IBookingRepository _bookingRepository;
        private readonly IServiceRepository _serviceRepository;
        private readonly IMapper _mapper;

        public InvoiceService(IInvoiceRepository invoiceRepository, IBookingRepository bookingRepository,
                              IServiceRepository serviceRepository, IMapper mapper)
        {
            _invoiceRepository = invoiceRepository;
            _bookingRepository = bookingRepository;
            _serviceRepository = serviceRepository;
            _mapper = mapper;
        }

        public async Task<InvoiceDto> AddItemAsync(int invoiceId, InvoiceItemCreateDto createItem)
        {
            var invoice = await _invoiceRepository.GetAsync(i => i.InvoiceId == invoiceId)
                          ?? throw new KeyNotFoundException("Invoice not found");

            var service = await _serviceRepository.GetAsync(s => s.Name == createItem.ServiceName)
                            ?? throw new KeyNotFoundException("Service not found");

            var item = new InvoiceItem
            {
                InvoiceId = invoice.InvoiceId,
                ServiceId = service.ServiceId,
                Description = string.IsNullOrEmpty(createItem.Description) ? service.Description : createItem.Description,
                Quantity = createItem.Quantity,
                UnitPrice = service.Price,
                CreatedAt = DateTime.UtcNow,
                ItemType = InvoiceItemType.AdditionalService,
            };

            var updatedInvoice = await _invoiceRepository.AddItemAsync(item);
            await RecalculateAsync(invoiceId);
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
                        Description = $"Бронювання номеру {booking.Room.RoomNumber} ({booking.Room.RoomType.RoomCategory}) - {nights} ночей",
                        Quantity = nights,
                        UnitPrice = booking.Room.RoomType.PricePerNight,
                        ItemType = InvoiceItemType.RoomBooking,
                        CreatedAt = DateTime.UtcNow
                    }
                }
            };

            await _invoiceRepository.CreateAsync(invoice);
            return _mapper.Map<InvoiceDto>(invoice);
        }

        public async Task RecalculateAsync(int invoiceId)
        {
            var invoice = await _invoiceRepository.GetAsync(i => i.InvoiceId == invoiceId, IncludeProperties)
                          ?? throw new KeyNotFoundException("Invoice not found");

            InvoiceStatus newStatus;
            if (invoice.PaidAmount == 0)
            {
                newStatus = InvoiceStatus.Pending;
            }
            else if (invoice.PaidAmount < invoice.TotalAmount)
            {
                newStatus = InvoiceStatus.PartiallyPaid;
            }
            else
            {
                newStatus = InvoiceStatus.Paid;
            }

            invoice.Status = newStatus;
            await _invoiceRepository.UpdateAsync(invoice);
        }

        public async Task<IEnumerable<InvoiceDto>> GetAllInvoicesAsync()
        {
            var invoices = await _invoiceRepository.GetAllAsync(includeProperties: IncludeProperties);
            var invoiceDtoList = _mapper.Map<IEnumerable<InvoiceDto>>(invoices);

            // Refactor this later
            invoiceDtoList
                .Where(i => i.Payments.Any())
                .ToList()
                .ForEach(dto =>
                {
                    var lastPaidAt = dto.Payments.Max(p => p.PaidAt);
                    dto.InvoiceItems.ToList().ForEach(item =>
                    {
                        item.IsLocked = item.CreatedAt <= lastPaidAt;
                    });
                });

            return invoiceDtoList;
        }

        public async Task<InvoiceDto> GetInvoiceByBookingIdAsync(int bookingId)
        {
            var invoice = await _invoiceRepository.GetAsync(i => i.BookingId == bookingId, IncludeProperties)
                          ?? throw new KeyNotFoundException("Invoice not found");
            var invoiceDto = _mapper.Map<InvoiceDto>(invoice);

            if (invoice.Payments.Any())
            {
                var lastPaidAt = invoice.Payments.Max(p => p.PaidAt);
                foreach (var itemDto in invoiceDto.InvoiceItems)
                {
                    itemDto.IsLocked = itemDto.CreatedAt <= lastPaidAt;
                }
            }

            return invoiceDto;
        }

        public async Task RemoveItemAsync(int invoiceItemId)
        {
            var invoiceId = await _invoiceRepository.RemoveItemAsync(invoiceItemId);
            await RecalculateAsync(invoiceId);
        }

        public async Task<InvoiceDto> UpdateStatusAsync(int invoiceId, InvoiceStatus status)
        {
            var invoice = await _invoiceRepository.GetAsync(i => i.InvoiceId == invoiceId)
                          ?? throw new KeyNotFoundException("Invoice not found");
            invoice.Status = status;
            await _invoiceRepository.UpdateAsync(invoice);
            return _mapper.Map<InvoiceDto>(invoice);
        }

        public async Task UpdateBookingTypeItemAsync(int bookingId)
        {
            var invoice = await _invoiceRepository.GetAsync(i => i.BookingId == bookingId, includeProperties: "InvoiceItems,Booking,Booking.Room,Booking.Room.RoomType")
                                ?? throw new KeyNotFoundException("Invoice not found");

            var bookingItem = invoice.InvoiceItems.SingleOrDefault(i => i.ItemType == InvoiceItemType.RoomBooking)
                              ?? throw new InvalidOperationException("RoomBooking item is missing.");
            var nights = (invoice.Booking.EndDate.Date - invoice.Booking.StartDate.Date).Days;

            bookingItem.Description = $"Бронювання номеру {invoice.Booking.Room.RoomNumber} ({invoice.Booking.Room.RoomType.RoomCategory}) - {nights} ночей";
            bookingItem.Quantity = nights;
            bookingItem.UnitPrice = invoice.Booking.Room.RoomType.PricePerNight;

            await _invoiceRepository.SaveAsync();
        }
    }
}
