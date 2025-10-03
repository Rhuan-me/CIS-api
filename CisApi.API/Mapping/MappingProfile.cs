using AutoMapper;
using CisApi.API.DTOs;
using CisApi.Core.Entities;

namespace CisApi.API.Mapping;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        // Mapeia da entidade Topic para o TopicDto
        CreateMap<Topic, TopicDto>();
        // Mapeia do CreateTopicDto para a entidade Topic
        CreateMap<CreateTopicDto, Topic>();
    }
}