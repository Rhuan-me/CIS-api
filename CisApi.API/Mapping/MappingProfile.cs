using AutoMapper;
using CisApi.API.DTOs;
using CisApi.Core.Entities;

namespace CisApi.API.Mapping;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        // Mapeamentos de Topic
        CreateMap<Topic, TopicDto>();
        CreateMap<CreateTopicDto, Topic>();

        // NOVOS MAPEAMENTOS DE IDEA
        CreateMap<Idea, IdeaDto>();
        CreateMap<CreateIdeaDto, Idea>();
    }
}