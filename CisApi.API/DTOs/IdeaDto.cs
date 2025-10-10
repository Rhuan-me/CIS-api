namespace CisApi.API.DTOs;

public class IdeaDto
{
    public int Id { get; set; }
    public int TopicId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string OwnedBy { get; set; } = string.Empty;
    public int VoteCount { get; set; }
    public DateTime CreatedAt { get; set; }
}