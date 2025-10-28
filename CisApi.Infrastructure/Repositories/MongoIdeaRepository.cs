using CisApi.Core.Entities;
using CisApi.Core.Interfaces;
using MongoDB.Driver;

namespace CisApi.Infrastructure.Repositories;

public class MongoIdeaRepository : IIdeaRepository
{
    // Este repositório opera na coleção 'topics', 
    // pois as ideias estão aninhadas dentro dos tópicos.
    private readonly IMongoCollection<Topic> _topics;

    public MongoIdeaRepository(IMongoDatabase database)
    {
        _topics = database.GetCollection<Topic>("topics");
    }

    public async Task<Idea?> GetByIdAsync(int id)
    {
        // Encontra o tópico que contém a ideia
        var filter = Builders<Topic>.Filter.ElemMatch(t => t.Ideas, i => i.Id == id);
        var topic = await _topics.Find(filter).FirstOrDefaultAsync();
        
        // Retorna a ideia específica dessa lista
        return topic?.Ideas.FirstOrDefault(i => i.Id == id);
    }

    public async Task AddAsync(Idea idea)
    {
        // Para adicionar uma ideia, temos de "empurrá-la" para
        // o array 'Ideas' do Tópico pai correspondente.
        var filter = Builders<Topic>.Filter.Eq(t => t.Id, idea.TopicId);
        var update = Builders<Topic>.Update.Push(t => t.Ideas, idea);

        await _topics.UpdateOneAsync(filter, update);
    }

    public async Task UpdateAsync(Idea idea)
    {
        // Atualizar um sub-documento é mais complexo.
        // Precisamos encontrar o Tópico e, em seguida, o elemento 
        // específico no array 'Ideas' para o substituir.
        var filter = Builders<Topic>.Filter.And(
            Builders<Topic>.Filter.Eq(t => t.Id, idea.TopicId),
            Builders<Topic>.Filter.ElemMatch(t => t.Ideas, i => i.Id == idea.Id)
        );
        
        var update = Builders<Topic>.Update.Set("ideas.$", idea);

        await _topics.UpdateOneAsync(filter, update);
    }

    public async Task DeleteAsync(Idea idea)
    {
        // Para apagar uma ideia, temos de "retirá-la" do
        // array 'Ideas' do Tópico pai.
        var filter = Builders<Topic>.Filter.Eq(t => t.Id, idea.TopicId);
        var update = Builders<Topic>.Update.Pull(t => t.Ideas, idea);

        await _topics.UpdateOneAsync(filter, update);
    }
}