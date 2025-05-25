using AutoMapper;
using Microsoft.EntityFrameworkCore.Storage;
using MiniHotel.Application.Interfaces.IRepository;
using MiniHotel.Application.Interfaces.IService;
using MiniHotel.Application.Services;
using Moq;

namespace MiniHotel.Tests.Services.BookingServiceTests
{
    public partial class BookingServiceTests
    {
        private readonly Mock<IBookingRepository> _bookingRepo;
        private readonly Mock<IRoomRepository> _roomRepo;
        private readonly Mock<IInvoiceService> _invoiceService;
        private readonly Mock<IUserRepository> _userRepo;
        private readonly Mock<IMapper> _mapper;
        private readonly Mock<IDbContextTransaction> _mockTransaction;
        private readonly BookingService _bookingService;

        public BookingServiceTests()
        {
            _bookingRepo = new Mock<IBookingRepository>();
            _roomRepo = new Mock<IRoomRepository>();
            _invoiceService = new Mock<IInvoiceService>();
            _userRepo = new Mock<IUserRepository>();
            _mapper = new Mock<IMapper>();
            _mockTransaction = new Mock<IDbContextTransaction>();

            _bookingService = new BookingService(
                _bookingRepo.Object,
                _roomRepo.Object,
                _invoiceService.Object,
                _mapper.Object,
                _userRepo.Object
            );
        }
    }
}
