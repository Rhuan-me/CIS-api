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

    // --- SEU MÉTODO (CORRETO) ---
    public async Task<bool> HasVotedAsync(int ideaId, string userEmail)
    {
        var filter = Builders<Topic>.Filter.ElemMatch(
            t => t.Ideas,
            i => i.Id == ideaId && i.Votes.Any(v => v.VotedBy == userEmail)
        );
        var topic = await _topics.Find(filter).FirstOrDefaultAsync();
        return topic != null;
    }

    // --- SEU MÉTODO (CORRETO) ---
    // Este método já estava correto no seu snippet,
    // corrigindo o typo "ideas" para "Ideas".
    public async Task AddAsync(Vote vote, int ideaId)
    {
        var filter = Builders<Topic>.Filter.ElemMatch(t => t.Ideas, i => i.Id == ideaId);

        var update = Builders<Topic>.Update.Push("ideas.$.votes", vote);

        await _topics.UpdateOneAsync(filter, update);

        // 3. Executar
        var result = await _topics.UpdateOneAsync(filter, update);

        if (result.MatchedCount == 0)
        {
            throw new KeyNotFoundException($"Ideia com ID {ideaId} não encontrada em nenhum tópico.");
        }
    }

    // --- MÉTODO CORRIGIDO ---
    // Assinatura alterada para usar identificadores
    public async Task DeleteAsync(int ideaId, string userEmail)
    {
        var filter = Builders<Topic>.Filter.ElemMatch(t => t.Ideas, i => i.Id == ideaId);

        // CORRIGIDO: Voltando para camelCase
        var update = Builders<Topic>.Update.PullFilter(
            "ideas.$.votes", // O caminho para o array
            Builders<Vote>.Filter.Eq(v => v.VotedBy, userEmail)
        );
        
        await _topics.UpdateOneAsync(filter, update);

        // 3. Executar
        var result = await _topics.UpdateOneAsync(filter, update);

        if (result.ModifiedCount == 0)
        {
            throw new KeyNotFoundException($"Voto do usuário {userEmail} na ideia {ideaId} não encontrado.");
        }
    }
    
    // O método GetByIdAsync(int id) foi removido pois um Voto não tem
    // um ID inteiro global. Ele é identificado pelo 'VotedBy' dentro de uma 'Idea'.
}