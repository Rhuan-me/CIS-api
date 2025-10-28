using MongoDB.Bson.Serialization.Attributes;

namespace CisApi.Core.Entities;

/// <summary>
/// Representa os principais tópicos de discussão no sistema.
/// </summary>
public class Topic
{
    [BsonId] // Mapeia esta propriedade para o campo _id do MongoDB
    public int Id { get; set; }
    
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string OwnedBy { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    
    public virtual ICollection<Idea> Ideas { get; set; } = new List<Idea>();
}