using AutoMapper;
using CommandService.DTOs;
using CommandService.Models;

namespace CommandService.Profiles
{
    public class CommandsProfile : Profile 
    {
        public CommandsProfile()
        {
            // Source -> Target
            CreateMap<Platform, PlatformReadDto>();
            CreateMap<Command, CommandReadDto>();
            CreateMap<CommandCreateDto, Command>();
            CreateMap<PlatformPublishedDto, Platform>().ForMember(dest => dest.ExternalID, opt => opt.MapFrom(src => src.Id));
        }       
    }
    
}