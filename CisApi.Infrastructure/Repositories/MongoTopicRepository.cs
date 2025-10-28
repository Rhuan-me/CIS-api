using CisApi.Core.Entities;
using CisApi.Core.Interfaces;
using MongoDB.Driver;

namespace CisApi.Infrastructure.Repositories;

public class MongoTopicRepository : ITopicRepository
{
    private readonly IMongoCollection<Topic> _topics;

    public MongoTopicRepository(IMongoDatabase database)
    {
        _topics = database.GetCollection<Topic>("topics");
    }

    public async Task<IEnumerable<Topic>> GetAllAsync()
    {
        return await _topics.Find(t => true).ToListAsync();
    }

    public async Task<Topic> GetByIdAsync(int id)
    {
        return await _topics.Find(t => t.Id == id).FirstOrDefaultAsync();
    }

    public async Task AddAsync(Topic topic)
    {
        await _topics.InsertOneAsync(topic);
    }

    public async Task UpdateAsync(Topic topic)
    {
        // Substitui o documento inteiro
        await _topics.ReplaceOneAsync(t => t.Id == topic.Id, topic);
    }

    public async Task DeleteAsync(int id)
    {
        await _topics.DeleteOneAsync(t => t.Id == id);
    }
}