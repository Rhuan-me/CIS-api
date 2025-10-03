using AutoMapper;
using CisApi.API.DTOs;
using CisApi.Core.Entities;
using CisApi.Core.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace CisApi.API.Controllers;

[ApiController]
[Route("api/topics")] // Rota baseada na documentação da API
public class TopicsController : ControllerBase
{
    private readonly ITopicRepository _topicRepository;
    private readonly IMapper _mapper;

    public TopicsController(ITopicRepository topicRepository, IMapper mapper)
    {
        _topicRepository = topicRepository;
        _mapper = mapper;
    }

    [HttpGet]
    public async Task<IActionResult> GetTopics()
    {
        var topics = await _topicRepository.GetAllAsync();
        var topicDtos = _mapper.Map<IEnumerable<TopicDto>>(topics);
        return Ok(topicDtos);
    }

    [HttpPost]
    public async Task<IActionResult> CreateTopic([FromBody] CreateTopicDto createTopicDto)
    {
        // O FluentValidation já executa automaticamente e retorna um erro 400 se for inválido
        var topic = _mapper.Map<Topic>(createTopicDto);

        topic.CreatedAt = DateTime.UtcNow;
        topic.UpdatedAt = DateTime.UtcNow;
        topic.OwnedBy = "user-from-java-api@example.com"; // Placeholder: TODO: Obter do usuário autenticado

        await _topicRepository.AddAsync(topic);

        var topicDto = _mapper.Map<TopicDto>(topic);

        // Retorna um status 201 Created com a localização do novo recurso
        return CreatedAtAction(nameof(GetTopics), new { id = topicDto.Id }, topicDto);
    }
}