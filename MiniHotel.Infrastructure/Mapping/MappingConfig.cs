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
            CreateMap<RegisterRequestDto, User>();
            CreateMap<ApplicationUser, User>()
                .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.Id));
            CreateMap<User, UserDto>().ReverseMap();

            CreateMap<Room, RoomDto>().ReverseMap();
            CreateMap<Room, RoomUpsertDto>().ReverseMap();

            CreateMap<Service, ServiceDto>().ReverseMap();
            CreateMap<Service, ServiceUpsertDto>().ReverseMap();

            CreateMap<Booking, BookingDto>()
                .ForMember(dest => dest.RoomNumber,
                opt => opt.MapFrom(src => src.Room.RoomNumber));
            CreateMap<Booking, BookingCreateDto>().ReverseMap();
            CreateMap<Booking, BookingUpdateDto>().ReverseMap();

            CreateMap<BookingService, ServiceToBookingDto>().ReverseMap();

            CreateMap<DateTime, DateOnly>().ConvertUsing(dateTime => DateOnly.FromDateTime(dateTime));
        }
    }
}
