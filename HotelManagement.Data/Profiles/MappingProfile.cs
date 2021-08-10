using AutoMapper;
using HotelManagement.Data.Dtos;
using HotelManagement.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelManagement.Data.Profiles
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<AppUser, AppUserDto>().ReverseMap();

            CreateMap<Room, RoomResponseDto>().ReverseMap();

            CreateMap<UpdateAppUserDto, AppUser>().ReverseMap();

            CreateMap<BookingDto, Room>().ReverseMap();
        }
    }
}
