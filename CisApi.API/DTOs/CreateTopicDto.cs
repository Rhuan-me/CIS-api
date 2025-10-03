namespace CisApi.API.DTOs;

public class CreateTopicDto
{
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
}