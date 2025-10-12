using CisApi.Core.Entities;
using CisApi.Core.Interfaces;
using CisApi.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace CisApi.Infrastructure.Repositories;

public class TopicRepository : ITopicRepository
{
    private readonly CisDbContext _context;

    public TopicRepository(CisDbContext context)
    {
        _context = context;
    }

    public async Task<Topic?> GetByIdAsync(int id)
    {
        return await _context.Topics.FindAsync(id);
    }

    public async Task<IEnumerable<Topic>> GetAllAsync()
    {
        return await _context.Topics.ToListAsync();
    }

    public async Task<Topic> AddAsync(Topic topic)
    {
        await _context.Topics.AddAsync(topic);
        await _context.SaveChangesAsync();
        return topic;
    }

    public async Task UpdateAsync(Topic topic)
    {
        _context.Entry(topic).State = EntityState.Modified;
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var topic = await _context.Topics.FindAsync(id);
        if (topic != null)
        {
            _context.Topics.Remove(topic);
            await _context.SaveChangesAsync();
        }
    }
}