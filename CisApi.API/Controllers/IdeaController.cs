using AutoMapper;
using CisApi.API.DTOs;
using CisApi.Core.Entities;
using CisApi.Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CisApi.API.Controllers;

[Authorize]
[ApiController]
[Route("api/ideas")]
public class IdeaController : ControllerBase
{
    private readonly IIdeaRepository _ideaRepository;
    private readonly ITopicRepository _topicRepository;
    private readonly IMapper _mapper;

    public IdeaController(IIdeaRepository ideaRepository, ITopicRepository topicRepository, IMapper mapper)
    {
        _ideaRepository = ideaRepository;
        _topicRepository = topicRepository;
        _mapper = mapper;
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetIdea(int id)
    {
        var idea = await _ideaRepository.GetByIdAsync(id);
        if (idea == null) return NotFound();
        return Ok(_mapper.Map<IdeaDto>(idea));
    }
    
    [HttpGet("topic/{topicId}")]
    public async Task<IActionResult> GetIdeasByTopic(int topicId)
    {
        var ideas = await _ideaRepository.GetByTopicIdAsync(topicId);
        return Ok(_mapper.Map<IEnumerable<IdeaDto>>(ideas));
    }

    [HttpPost]
    public async Task<IActionResult> CreateIdea([FromBody] CreateIdeaDto createIdeaDto)
    {
        if (await _topicRepository.GetByIdAsync(createIdeaDto.TopicId) == null)
            return BadRequest("O Tópico especificado não existe.");

        var idea = _mapper.Map<Idea>(createIdeaDto);
        idea.OwnedBy = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        idea.CreatedAt = DateTime.UtcNow;
        idea.UpdatedAt = DateTime.UtcNow;

        var createdIdea = await _ideaRepository.AddAsync(idea);
        return CreatedAtAction(nameof(GetIdea), new { id = createdIdea.Id }, _mapper.Map<IdeaDto>(createdIdea));
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateIdea(int id, [FromBody] CreateIdeaDto updateIdeaDto)
    {
        var idea = await _ideaRepository.GetByIdAsync(id);
        if (idea == null) return NotFound();

        if (idea.OwnedBy != User.FindFirstValue(ClaimTypes.NameIdentifier)) return Forbid();

        _mapper.Map(updateIdeaDto, idea);
        idea.UpdatedAt = DateTime.UtcNow;
        await _ideaRepository.UpdateAsync(idea);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteIdea(int id)
    {
        var idea = await _ideaRepository.GetByIdAsync(id);
        if (idea == null) return NotFound();

        if (idea.OwnedBy != User.FindFirstValue(ClaimTypes.NameIdentifier)) return Forbid();

        await _ideaRepository.DeleteAsync(id);
        return NoContent();
    }
}