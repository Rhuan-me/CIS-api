using System.Security.Claims;
using AutoMapper;
using CisApi.Core.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CisApi.API.Controllers;

// Crie este novo controller na pasta Controllers
[Authorize]
[ApiController]
[Route("api/ideas")]
public class IdeaController : ControllerBase
{
    // Crie IIdeaRepository e IdeaRepository assim como fez para Tópicos
    private readonly IIdeaRepository _ideaRepository;
    private readonly ITopicRepository _topicRepository;
    private readonly IMapper _mapper;

    public IdeaController(IIdeaRepository ideaRepository, ITopicRepository topicRepository, IMapper mapper)
    {
        _ideaRepository = ideaRepository;
        _topicRepository = topicRepository;
        _mapper = mapper;
    }

    [HttpPost]
    public async Task<IActionResult> CreateIdea([FromBody] CreateIdeaDto createIdeaDto)
    {
        var topic = await _topicRepository.GetByIdAsync(createIdeaDto.TopicId);
        if (topic == null)
        {
            return BadRequest("Tópico não encontrado.");
        }
        
        var idea = _mapper.Map<Idea>(createIdeaDto);
        var userEmail = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        
        idea.OwnedBy = userEmail;
        idea.CreatedAt = DateTime.UtcNow;
        idea.UpdatedAt = DateTime.UtcNow;

        var createdIdea = await _ideaRepository.AddAsync(idea);
        var ideaDto = _mapper.Map<IdeaDto>(createdIdea);

        return Ok(ideaDto);
    }
    
    // Adicione os outros endpoints: Get, Update, Delete
}