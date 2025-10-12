using CisApi.Core.Entities;

namespace CisApi.Core.Interfaces;

public interface IVoteRepository
{
    Task<Vote?> GetVoteAsync(int ideaId, string userEmail);
    Task<Vote> AddAsync(Vote vote);
    Task DeleteAsync(Vote vote);
}