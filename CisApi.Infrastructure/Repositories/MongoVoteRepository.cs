using CisApi.Core.Entities;
using CisApi.Core.Interfaces;
using MongoDB.Driver;

namespace CisApi.Infrastructure.Repositories;

public class MongoVoteRepository : IVoteRepository
{
    private readonly IMongoCollection<Topic> _topics;

    public MongoVoteRepository(IMongoDatabase database)
    {
        _topics = database.GetCollection<Topic>("topics");
    }

    public async Task<Vote?> GetByIdAsync(int id)
    {
        // Esta é uma operação muito ineficiente em MongoDB e deve ser evitada
        // se possível. Estamos a procurar em todos os tópicos e todas as ideias.
        var filter = Builders<Topic>.Filter.ElemMatch(
            t => t.Ideas, 
            i => i.Votes.Any(v => v.Id == id)
        );
        var topic = await _topics.Find(filter).FirstOrDefaultAsync();
        
        // Encontra e retorna o voto
        return topic?.Ideas
            .SelectMany(i => i.Votes)
            .FirstOrDefault(v => v.Id == id);
    }

    public async Task AddAsync(Vote vote)
    {
        // Para adicionar um Voto, temos de o "empurrar" para o array 'Votes'
        // da Ideia específica, dentro do Tópico específico.
        
        // 1. Encontrar o Tópico que contém a Ideia
        var ideaFilter = Builders<Topic>.Filter.ElemMatch(t => t.Ideas, i => i.Id == vote.IdeaId);
        
        // 2. Definir a atualização para adicionar o voto ao array "votes" da ideia
        // O operador $[] é o "all positional operator" para encontrar a ideia certa
        // e .$ é o "positional operator" para atualizar o array
        var update = Builders<Topic>.Update.Push("ideas.$[idea].votes", vote);
        
        // 3. Opções para dizer ao MongoDB como encontrar a "idea"
        var arrayFilters = new[]
        {
            new BsonDocumentArrayFilterDefinition<Idea>(
                new BsonDocument("idea._id", vote.IdeaId))
        };
        
        await _topics.UpdateOneAsync(ideaFilter, update, new UpdateOptions { ArrayFilters = arrayFilters });
    }

    public async Task DeleteAsync(Vote vote)
    {
        // Semelhante a adicionar, mas usamos "Pull"
        var ideaFilter = Builders<Topic>.Filter.ElemMatch(t => t.Ideas, i => i.Id == vote.IdeaId);
        
        var update = Builders<Topic>.Update.Pull("ideas.$[idea].votes", vote);

        var arrayFilters = new[]
        {
            new BsonDocumentArrayFilterDefinition<Idea>(
                new BsonDocument("idea._id", vote.IdeaId))
        };

        await _topics.UpdateOneAsync(ideaFilter, update, new UpdateOptions { ArrayFilters = arrayFilters });
    }
}