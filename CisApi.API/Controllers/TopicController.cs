using AutoMapper;
using CisApi.API.DTOs;
using CisApi.Core.Entities;
using CisApi.Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using CisApi.API.Validators;

namespace CisApi.API.Controllers;

[Authorize]
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
        return Ok(_mapper.Map<IEnumerable<TopicDto>>(topics));
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetTopic(int id)
    {
        var topic = await _topicRepository.GetByIdAsync(id);
        if (topic == null) return NotFound();
        return Ok(_mapper.Map<TopicDto>(topic));
    }

    [HttpPost]
    public async Task<IActionResult> CreateTopic([FromBody] CreateTopicDto createTopicDto)
    {
        var topic = _mapper.Map<Topic>(createTopicDto);
        /*
        var userEmail = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userEmail)) return Unauthorized();

        topic.OwnedBy = userEmail;*/
        topic.CreatedAt = DateTime.UtcNow;
        topic.UpdatedAt = DateTime.UtcNow;

        var createdTopic = await _topicRepository.AddAsync(topic);
        return CreatedAtAction(nameof(GetTopic), new { id = createdTopic.Id }, _mapper.Map<TopicDto>(createdTopic));
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateTopic(int id, [FromBody] CreateTopicDto updateTopicDto)
    {
        var topic = await _topicRepository.GetByIdAsync(id);
        if (topic == null) return NotFound();

        var userEmail = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (topic.OwnedBy != userEmail) return Forbid();

        _mapper.Map(updateTopicDto, topic);
        topic.UpdatedAt = DateTime.UtcNow;
        await _topicRepository.UpdateAsync(topic);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteTopic(int id)
    {
        var topic = await _topicRepository.GetByIdAsync(id);
        if (topic == null) return NotFound();

        var userEmail = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (topic.OwnedBy != userEmail) return Forbid();

        await _topicRepository.DeleteAsync(id);
        return NoContent();
    }
}