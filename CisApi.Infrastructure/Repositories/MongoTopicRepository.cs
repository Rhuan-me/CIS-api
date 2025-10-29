using CisApi.Core.Entities;
using CisApi.Core.Interfaces;
using MongoDB.Driver;
using MongoDB.Bson; // <--- ADICIONE ESTE USING

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
        // Lógica CORRIGIDA para Aggregation
        var maxIdResult = await _topics.Aggregate()
            // Ordena pelos BSON Documents, não por LINQ
            .Sort(Builders<Topic>.Sort.Descending(t => t.Id)) 
            .Limit(1)
            // Projeta usando BSON Document syntax
            .Project(new BsonDocument { { "_id", 0 }, { "Id", "$_id" } }) 
            .FirstOrDefaultAsync(); // Retorna um BsonDocument

        // Se maxIdResult não for nulo e contiver o campo "Id"
        if (maxIdResult != null && maxIdResult.Contains("Id"))
        {
            topic.Id = maxIdResult["Id"].AsInt32 + 1;
        }
        else
        {
            topic.Id = 1; // Se a coleção estiver vazia
        }
    
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