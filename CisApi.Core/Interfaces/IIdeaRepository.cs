using CisApi.Core.Entities;

namespace CisApi.Core.Interfaces;

public interface IIdeaRepository
{
    Task<Idea?> GetByIdAsync(int id);
    Task<IEnumerable<Idea>> GetByTopicIdAsync(int topicId);
    Task<Idea> AddAsync(Idea idea);
    Task UpdateAsync(Idea idea);
    Task DeleteAsync(int id);
    Task IncrementVoteCountAsync(int id);
    Task DecrementVoteCountAsync(int id);
}