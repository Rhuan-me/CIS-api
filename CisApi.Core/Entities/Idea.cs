using MongoDB.Bson.Serialization.Attributes;

namespace CisApi.Core.Entities;

public class Idea
{
    [BsonId]
    public int Id { get; set; }
    
    public int TopicId { get; set; } 
    
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string OwnedBy { get; set; } = string.Empty;
    public int VoteCount { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    
    public virtual ICollection<Vote> Votes { get; set; } = new List<Vote>();
}