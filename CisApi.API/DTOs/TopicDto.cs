namespace CisApi.API.DTOs;

public class TopicDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string OwnedBy { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}