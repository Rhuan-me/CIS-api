// CisApi.Infrastructure/Repositories/TopicRepository.cs
using CisApi.Core.Entities;
using CisApi.Core.Interfaces;
using CisApi.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace CisApi.Infrastructure.Repositories;

// Esta classe implementa a interface ITopicRepository usando o Entity Framework.
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

    public async Task AddAsync(Topic topic)
    {
        await _context.Topics.AddAsync(topic);
        await _context.SaveChangesAsync();
    }
}