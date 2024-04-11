
using API.Models.Dtos;
using API.Models.Entities;
using AutoMapper;

namespace UserMicroservice.Utils
{
    public class Mapper : Profile
    {
        public Mapper()
        {
            CreateMap<Game, GameDto>();
            CreateMap<AddMessageDto, ChatMessage>();
        }
    }
}