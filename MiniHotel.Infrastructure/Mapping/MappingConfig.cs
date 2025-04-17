using AutoMapper;
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
                .ForMember(dest => dest.PricePerDay, opt => opt.MapFrom(src => src.RoomType.PricePerNight))
                .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.RoomType.Description));
            CreateMap<Room, RoomUpsertDto>().ReverseMap();

            CreateMap<Service, ServiceDto>().ReverseMap();
            CreateMap<Service, ServiceUpsertDto>().ReverseMap();

            CreateMap<Booking, BookingDto>()
                .ForMember(dest => dest.RoomNumber, opt => opt.MapFrom(src => src.Room.RoomNumber))
                .ForMember(dest => dest.InvoiceId, opt => opt.MapFrom(src => src.Invoice.InvoiceId))
                .ForMember(dest => dest.RoomCategory, opt => opt.MapFrom(src => src.Room.RoomType.RoomCategory))
                .ForMember(dest => dest.Amount, opt => opt.MapFrom(src => src.Invoice.TotalAmount))
                .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => GetFullName(src)));

            CreateMap<Booking, UserBookingsDto>()
                .ForMember(dest => dest.RoomNumber, opt => opt.MapFrom(src => src.Room.RoomNumber))
                .ForMember(dest => dest.RoomCategory, opt => opt.MapFrom(src => src.Room.RoomType.RoomCategory))
                .ForMember(dest => dest.Amount, opt => opt.MapFrom(src => src.Invoice.TotalAmount));

            CreateMap<Booking, BookingCreateDto>().ReverseMap();
            CreateMap<Booking, BookingCreateByAdminDto>().ReverseMap();
            CreateMap<Booking, BookingUpdateDto>().ReverseMap();

            CreateMap<Invoice, InvoiceDto>().ReverseMap();

            CreateMap<InvoiceItem, InvoiceItemDto>().ReverseMap();
            CreateMap<InvoiceItem, InvoiceItemCreateDto>().ReverseMap();

            CreateMap<RoomType, RoomTypeDto>().ReverseMap();
            CreateMap<RoomType, RoomTypeUpsertDto>().ReverseMap();

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
    }
}
