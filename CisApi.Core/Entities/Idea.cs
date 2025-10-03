// CisApi.Core/Entities/Idea.cs
namespace CisApi.Core.Entities;

/// <summary>
/// Representa ideias individuais propostas dentro de cada tópico.
/// </summary>
public class Idea
{
    public int Id { get; set; } // Chave primária
    public int TopicId { get; set; } // Chave estrangeira para Topic
    public string Title { get; set; } = string.Empty; // Título da ideia (obrigatório)
    public string? Description { get; set; } // Descrição da ideia (opcional)
    public string OwnedBy { get; set; } = string.Empty; // Proprietário da ideia (obrigatório, indexado)
    public int VoteCount { get; set; } // Contador de votos desnormalizado para performance
    public DateTime CreatedAt { get; set; } // Timestamp de criação
    public DateTime UpdatedAt { get; set; } // Timestamp de atualização
    
    /// <summary>
    /// Propriedade de navegação para o tópico pai.
    /// </summary>
    public virtual Topic Topic { get; set; } = null!;
    
    /// <summary>
    /// Propriedade de navegação: uma ideia pode receber muitos votos.
    /// </summary>
    public virtual ICollection<Vote> Votes { get; set; } = new List<Vote>();
}