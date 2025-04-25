using AutoMapper;
using MiniHotel.Application.Common;
using MiniHotel.Application.DTOs;
using MiniHotel.Domain.Entities;
using MiniHotel.Infrastructure.Identity;

namespace MiniHotel.Infrastructure.Mapping
{
    public class MappingConfig : Profile
    {
        public MappingConfig()
        {
            CreateMap<RegisterRequestDto, HotelUser>();
            CreateMap<ApplicationUser, HotelUser>()
                .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.Id));

            CreateMap<HotelUser, UserDto>().ReverseMap();

            CreateMap<Room, RoomDto>()
                .ForMember(dest => dest.RoomCategory, opt => opt.MapFrom(src => src.RoomType.RoomCategory))
                .ForMember(dest => dest.PricePerDay, opt => opt.MapFrom(src => src.RoomType.PricePerNight));
            CreateMap<RoomUpsertDto, Room>().ReverseMap();

            CreateMap<Service, ServiceDto>().ReverseMap();
            CreateMap<Service, ServiceUpsertDto>().ReverseMap();

            CreateMap<Booking, BookingDto>()
                .ForMember(dest => dest.RoomNumber, opt => opt.MapFrom(src => src.Room.RoomNumber))
                .ForMember(dest => dest.InvoiceId, opt => opt.MapFrom(src => src.Invoice.InvoiceId))
                .ForMember(dest => dest.RoomCategory, opt => opt.MapFrom(src => src.Room.RoomType.RoomCategory))
                .ForMember(dest => dest.Amount, opt => opt.MapFrom(src => src.Invoice.TotalAmount))
                .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => GetFullName(src)))
                .ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(src => GetPhoneNumber(src)));

            CreateMap<Booking, UserBookingsDto>()
                .ForMember(dest => dest.RoomNumber, opt => opt.MapFrom(src => src.Room.RoomNumber))
                .ForMember(dest => dest.RoomCategory, opt => opt.MapFrom(src => src.Room.RoomType.RoomCategory))
                .ForMember(dest => dest.Amount, opt => opt.MapFrom(src => src.Invoice.TotalAmount));

            CreateMap(typeof(PagedResult<>), typeof(PagedResult<>));

            CreateMap<Booking, BookingCreateDto>().ReverseMap();
            CreateMap<Booking, BookingCreateByReceptionDto>().ReverseMap();
            CreateMap<Booking, BookingUpdateDto>().ReverseMap();

            CreateMap<Invoice, InvoiceDto>().ReverseMap();

            CreateMap<InvoiceItem, InvoiceItemDto>().ReverseMap();
            CreateMap<InvoiceItem, InvoiceItemCreateDto>().ReverseMap();

            CreateMap<RoomType, RoomTypeDto>().ReverseMap();
            CreateMap<RoomType, RoomTypeUpsertDto>().ReverseMap();

            CreateMap<Payment, PaymentDto>();

            CreateMap<DateTime, DateOnly>().ConvertUsing(dateTime => DateOnly.FromDateTime(dateTime));
        }

        private static string? GetFullName(Booking booking)
        {
            if (!string.IsNullOrWhiteSpace(booking.FullName))
                return booking.FullName;

            return booking.User != null
                ? $"{booking.User.FirstName} {booking.User.LastName}"
                : null;
        }

        private static string? GetPhoneNumber(Booking booking)
        {
            if (!string.IsNullOrWhiteSpace(booking.PhoneNumber))
                return booking.PhoneNumber;

            return booking.User != null
                ? booking.User.PhoneNumber
                : null;
        }
    }
}
