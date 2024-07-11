using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using UserMicroservice.DTOs;
using UserMicroservice.Entities;
namespace UserMicroservice.Utils
{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles()
        {
            CreateMap<User, UserDto>();
            CreateMap<User, AdminDto>();
            CreateMap<RegisterDto, User>();
            CreateMap<AccountDto, User>();
            CreateMap<User, AccountDto>();
            CreateMap<UserDto, User>();
            CreateMap<User, UserDto>();
        }
    }
}