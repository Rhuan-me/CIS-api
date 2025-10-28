using CisApi.Core.Entities;

namespace CisApi.Core.Interfaces;

public interface IVoteRepository
{
    Task<Vote?> GetByIdAsync(int id);
    Task AddAsync(Vote vote);
    Task DeleteAsync(Vote vote); 
}