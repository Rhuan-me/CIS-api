// CisApi.Core/Interfaces/ITopicRepository.cs
using CisApi.Core.Entities;

namespace CisApi.Core.Interfaces;

// Este contrato é definido na camada Core para que a lógica de negócio
// dependa dele, e não de uma implementação concreta de banco de dados.
public interface ITopicRepository
{
    Task<Topic?> GetByIdAsync(int id);
    Task<IEnumerable<Topic>> GetAllAsync();
    Task AddAsync(Topic topic);
    // Adicione outros métodos conforme necessário (Update, Delete, etc.)
}