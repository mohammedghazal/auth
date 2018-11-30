using auth.API.Dtos;
using auth.API.Models;
using AutoMapper;

namespace auth.API.Helpers
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<UserForRegisterDto, User>();

    
        }
    }

}