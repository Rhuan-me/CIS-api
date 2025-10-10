namespace CisApi.API.DTOs;

public class CreateIdeaDto
{
    public int TopicId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
}