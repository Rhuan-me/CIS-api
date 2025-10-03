// CisApi.Core/Entities/Topic.cs
namespace CisApi.Core.Entities;

/// <summary>
/// Representa os principais tópicos de discussão no sistema.
/// </summary>
public class Topic
{
    public int Id { get; set; } // Chave primária
    public string Title { get; set; } = string.Empty; // Título (obrigatório)
    public string? Description { get; set; } // Descrição (opcional)
    public string OwnedBy { get; set; } = string.Empty; // Email do proprietário (obrigatório, indexado)
    public DateTime CreatedAt { get; set; } // Timestamp de criação
    public DateTime UpdatedAt { get; set; } // Timestamp de atualização
    
    /// <summary>
    /// Propriedade de navegação: um tópico pode ter muitas ideias.
    /// </summary>
    public virtual ICollection<Idea> Ideas { get; set; } = new List<Idea>();
}