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

            CreateMap<Room, RoomDto>().ReverseMap();
            CreateMap<Room, RoomCreateDto>().ReverseMap();
        }
    }
}
