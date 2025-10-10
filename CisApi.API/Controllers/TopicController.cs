using AutoMapper;
using CisApi.API.DTOs;
using CisApi.Core.Entities;
using CisApi.Core.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace CisApi.API.Controllers;

[Authorize] // Garante que todos os endpoints neste controller exigem autenticação
[ApiController]
[Route("api/topics")]
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
        var topic = _mapper.Map<Topic>(createTopicDto);

        // --- ALTERAÇÃO PRINCIPAL AQUI ---
        // Pega o 'login' (email) do usuário a partir do token JWT.
        // O 'sub' (subject) do token JWT é mapeado para NameIdentifier.
        var userEmail = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        // Validação para garantir que o email foi encontrado no token
        if (string.IsNullOrEmpty(userEmail))
        {
            return Unauthorized("Não foi possível identificar o usuário a partir do token.");
        }

        topic.CreatedAt = DateTime.UtcNow;
        topic.UpdatedAt = DateTime.UtcNow;
        topic.OwnedBy = userEmail; // Atribui o email do usuário autenticado

        await _topicRepository.AddAsync(topic);

        var topicDto = _mapper.Map<TopicDto>(topic);

        // Retorna um status 201 Created com a localização do novo recurso
        return CreatedAtAction(nameof(GetTopics), new { id = topicDto.Id }, topicDto);
    }
    
    [HttpGet("{id}")]
    public async Task<IActionResult> GetTopic(int id)
    {
        var topic = await _topicRepository.GetByIdAsync(id);
        if (topic == null)
        {
            return NotFound();
        }
        return Ok(_mapper.Map<TopicDto>(topic));
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateTopic(int id, [FromBody] CreateTopicDto updateTopicDto)
    {
        var topic = await _topicRepository.GetByIdAsync(id);
        if (topic == null)
        {
            return NotFound();
        }

        // Apenas o proprietário pode editar
        var userEmail = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (topic.OwnedBy != userEmail)
        {
            return Forbid();
        }

        // Mapeia os dados do DTO para a entidade existente
        _mapper.Map(updateTopicDto, topic);
        topic.UpdatedAt = DateTime.UtcNow;

        await _topicRepository.UpdateAsync(topic);
        return NoContent(); // Resposta 204 No Content para sucesso
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteTopic(int id)
    {
        var topic = await _topicRepository.GetByIdAsync(id);
        if (topic == null)
        {
            return NotFound();
        }

        // Apenas o proprietário pode deletar
        var userEmail = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (topic.OwnedBy != userEmail)
        {
            return Forbid();
        }

        await _topicRepository.DeleteAsync(id);
        return NoContent();
    }

    // Altere o seu CreateTopic para usar o retorno do repositório
    [HttpPost]
    public async Task<IActionResult> CreateTopic([FromBody] CreateTopicDto createTopicDto)
    {
        var topic = _mapper.Map<Topic>(createTopicDto);
        var userEmail = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(userEmail))
        {
            return Unauthorized();
        }

        topic.CreatedAt = DateTime.UtcNow;
        topic.UpdatedAt = DateTime.UtcNow;
        topic.OwnedBy = userEmail;

        var createdTopic = await _topicRepository.AddAsync(topic); // Captura o tópico criado
        var topicDto = _mapper.Map<TopicDto>(createdTopic);

        return CreatedAtAction(nameof(GetTopic), new { id = topicDto.Id }, topicDto);
    }
}