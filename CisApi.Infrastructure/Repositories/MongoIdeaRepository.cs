using CisApi.Core.Entities;
using CisApi.Core.Interfaces;
using MongoDB.Driver;

namespace CisApi.Infrastructure.Repositories;

public class MongoIdeaRepository : IIdeaRepository
{
    private readonly IMongoCollection<Topic> _topics;

    public MongoIdeaRepository(IMongoDatabase database)
    {
        _topics = database.GetCollection<Topic>("topics");
    }

    // --- Seu código (correto) ---
    public async Task<Idea?> GetByIdAsync(int ideaId)
    {
        // Encontra o tópico que contém a ideia
        var filter = Builders<Topic>.Filter.ElemMatch(t => t.Ideas, i => i.Id == ideaId);
        var topic = await _topics.Find(filter).FirstOrDefaultAsync();

        if (topic == null) return null;

        // Retorna a ideia específica
        return topic.Ideas.FirstOrDefault(i => i.Id == ideaId);
    }

    // --- Seu código (correto) ---
    public async Task AddAsync(int topicId, Idea idea)
    {
        // 1. Buscar o tópico primeiro
        var filterTopic = Builders<Topic>.Filter.Eq(t => t.Id, topicId);
        var topic = await _topics.Find(filterTopic).FirstOrDefaultAsync();

        if (topic == null)
        {
            throw new KeyNotFoundException($"Tópico com ID {topicId} não encontrado.");
        }

        // 2. Calcular o novo ID da Idea
        // Contamos as ideias existentes e somamos 1
        int nextIndex = (topic.Ideas?.Count ?? 0) + 1;

        // 3. Aplicar a lógica: TopicID=1, Index=1 -> "11"
        string newIdString = $"{topic.Id}{nextIndex}";
        
        // 4. Definir o novo ID (agora como int)
        idea.Id = int.Parse(newIdString);

        // 5. Adicionar (Push) a nova ideia no array
        var update = Builders<Topic>.Update.Push(t => t.Ideas, idea);
        await _topics.UpdateOneAsync(filterTopic, update);
    }

    // --- INÍCIO DAS CORREÇÕES ---

    // ALTERADO: Assinatura agora recebe 'topicId'
    public async Task UpdateAsync(int topicId, Idea idea)
    {
        // 1. O filtro usa 'topicId' (vinda do parâmetro) e 'idea.Id'
        //    Removemos a referência ao 'idea.TopicId' que causava o erro
        var filter = Builders<Topic>.Filter.And(
            Builders<Topic>.Filter.Eq(t => t.Id, topicId),
            Builders<Topic>.Filter.ElemMatch(t => t.Ideas, i => i.Id == idea.Id)
        );
        
        var update = Builders<Topic>.Update
            .Set("ideas.$.title", idea.Title)
            .Set("ideas.$.description", idea.Description)
            .Set("ideas.$.updatedAt", idea.UpdatedAt);

        await _topics.UpdateOneAsync(filter, update);

        var result = await _topics.UpdateOneAsync(filter, update);

        if (result.MatchedCount == 0)
        {
            throw new KeyNotFoundException($"Ideia com ID {idea.Id} no tópico {topicId} não encontrada.");
        }
    }

    // ALTERADO: Assinatura agora recebe 'topicId' e 'ideaId'
    public async Task DeleteAsync(int topicId, int ideaId)
    {
        // 1. Filtro usa 'topicId' (vinda do parâmetro)
        var filter = Builders<Topic>.Filter.Eq(t => t.Id, topicId);
        
        // 2. 'PullFilter' remove o item do array 'Ideas' onde o Id bate
        var update = Builders<Topic>.Update.PullFilter(t => t.Ideas, i => i.Id == ideaId);

        var result = await _topics.UpdateOneAsync(filter, update);

        if (result.ModifiedCount == 0)
        {
            throw new KeyNotFoundException($"Ideia com ID {ideaId} no tópico {topicId} não encontrada para deleção.");
        }
    }
}