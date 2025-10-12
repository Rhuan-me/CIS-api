using CisApi.Core.Entities;

namespace CisApi.Core.Interfaces;

public interface ITopicRepository
{
    Task<Topic?> GetByIdAsync(int id);
    Task<IEnumerable<Topic>> GetAllAsync();
    Task<Topic> AddAsync(Topic topic);
    Task UpdateAsync(Topic topic);
    Task DeleteAsync(int id);
}