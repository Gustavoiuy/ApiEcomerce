using System;
using ApiEcommerce.Models.Dtos;
using AutoMapper;
namespace ApiEcommerce.Maping;

public class UserProfile : Profile
{
    public UserProfile()
    {
        CreateMap<Users, UserDto>().ReverseMap();
        CreateMap<Users, CreateUserDto>().ReverseMap();
        CreateMap<Users, UserLogginDto>().ReverseMap();
        CreateMap<Users, UserLogginResponseDto>().ReverseMap();
    }
}
