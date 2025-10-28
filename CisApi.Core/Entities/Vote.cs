using MongoDB.Bson.Serialization.Attributes;

namespace CisApi.Core.Entities;

/// <summary>
/// Representa os votos dos usuários em ideias específicas.
/// </summary>
public class Vote
{
    [BsonId]
    public int Id { get; set; }
    
    public int IdeaId { get; set; } 
    
    public string VotedBy { get; set; } = string.Empty;
    public DateTime VotedAt { get; set; }
}