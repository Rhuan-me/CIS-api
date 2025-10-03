// CisApi.Core/Entities/Vote.cs
namespace CisApi.Core.Entities;

/// <summary>
/// Representa os votos dos usuários em ideias específicas.
/// </summary>
public class Vote
{
    public int Id { get; set; } // Chave primária
    public int IdeaId { get; set; } // Chave estrangeira para Idea
    public string VotedBy { get; set; } = string.Empty; // Email do usuário que votou (obrigatório, indexado)
    public DateTime VotedAt { get; set; } // Timestamp do voto
    
    /// <summary>
    /// Propriedade de navegação para a ideia associada.
    /// </summary>
    public virtual Idea Idea { get; set; } = null!;
}