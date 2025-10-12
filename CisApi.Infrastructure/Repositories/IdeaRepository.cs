using CisApi.Core.Entities;
using CisApi.Core.Interfaces;
using CisApi.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace CisApi.Infrastructure.Repositories;

public class IdeaRepository : IIdeaRepository
{
    private readonly CisDbContext _context;

    public IdeaRepository(CisDbContext context)
    {
        _context = context;
    }

    public async Task<Idea?> GetByIdAsync(int id)
    {
        return await _context.Ideas.FindAsync(id);
    }

    public async Task<IEnumerable<Idea>> GetByTopicIdAsync(int topicId)
    {
        return await _context.Ideas
            .Where(i => i.TopicId == topicId)
            .ToListAsync();
    }

    public async Task<Idea> AddAsync(Idea idea)
    {
        await _context.Ideas.AddAsync(idea);
        await _context.SaveChangesAsync();
        return idea;
    }

    public async Task UpdateAsync(Idea idea)
    {
        _context.Entry(idea).State = EntityState.Modified;
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var idea = await _context.Ideas.FindAsync(id);
        if (idea != null)
        {
            _context.Ideas.Remove(idea);
            await _context.SaveChangesAsync();
        }
    }

    public async Task IncrementVoteCountAsync(int id)
    {
        var idea = await _context.Ideas.FindAsync(id);
        if (idea != null)
        {
            idea.VoteCount++;
            await _context.SaveChangesAsync();
        }
    }

    public async Task DecrementVoteCountAsync(int id)
    {
        var idea = await _context.Ideas.FindAsync(id);
        if (idea != null && idea.VoteCount > 0)
        {
            idea.VoteCount--;
            await _context.SaveChangesAsync();
        }
    }
}