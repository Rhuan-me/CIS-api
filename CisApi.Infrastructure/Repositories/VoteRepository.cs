using CisApi.Core.Entities;
using CisApi.Core.Interfaces;
using CisApi.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace CisApi.Infrastructure.Repositories;

public class VoteRepository : IVoteRepository
{
    private readonly CisDbContext _context;

    public VoteRepository(CisDbContext context)
    {
        _context = context;
    }

    public async Task<Vote?> GetVoteAsync(int ideaId, string userEmail)
    {
        return await _context.Votes
            .FirstOrDefaultAsync(v => v.IdeaId == ideaId && v.VotedBy == userEmail);
    }
    
    public async Task<Vote> AddAsync(Vote vote)
    {
        await _context.Votes.AddAsync(vote);
        await _context.SaveChangesAsync();
        return vote;
    }

    public async Task DeleteAsync(Vote vote)
    {
        _context.Votes.Remove(vote);
        await _context.SaveChangesAsync();
    }
}