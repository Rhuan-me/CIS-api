using System.Security.Claims;
using CisApi.Core.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CisApi.API.Controllers;

[Authorize]
[ApiController]
[Route("api/ideas/{ideaId}/vote")]
public class VoteController : ControllerBase
{
    private readonly IVoteRepository _voteRepository;
    private readonly IIdeaRepository _ideaRepository;

    public VoteController(IVoteRepository voteRepository, IIdeaRepository ideaRepository)
    {
        _voteRepository = voteRepository;
        _ideaRepository = ideaRepository;
    }

    [HttpPost]
    public async Task<IActionResult> Vote(int ideaId)
    {
        var userEmail = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        
        // Lógica para verificar se a ideia existe e se o usuário já não votou
        // ...

        var vote = new Vote
        {
            IdeaId = ideaId,
            VotedBy = userEmail,
            VotedAt = DateTime.UtcNow
        };

        await _voteRepository.AddAsync(vote);

        // Incrementar o contador de votos na ideia (VoteCount)
        await _ideaRepository.IncrementVoteCountAsync(ideaId);

        return Ok();
    }

    // Adicione um endpoint [HttpDelete] para remover o voto
}