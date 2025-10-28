using CisApi.Core.Entities;

namespace CisApi.Core.Interfaces;

public interface IIdeaRepository
{
    Task<Idea?> GetByIdAsync(int id);
    Task AddAsync(Idea idea);
    Task UpdateAsync(Idea idea);
    Task DeleteAsync(Idea idea); // Precisa da ideia completa para saber o TopicId pai
}