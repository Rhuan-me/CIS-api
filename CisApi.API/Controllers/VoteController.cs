using CisApi.Core.Entities;
using CisApi.Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CisApi.API.Controllers;

[Authorize]
[ApiController]
[Route("api/ideas/{ideaId}/votes")]
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
    public async Task<IActionResult> AddVote(int ideaId)
    {
        var idea = await _ideaRepository.GetByIdAsync(ideaId);
        if (idea == null) return NotFound("Ideia não encontrada.");

        var userEmail = User.FindFirstValue(ClaimTypes.NameIdentifier)!;

        if (await _voteRepository.GetVoteAsync(ideaId, userEmail) != null)
            return BadRequest("Você já votou nesta ideia.");

        var vote = new Vote { IdeaId = ideaId, VotedBy = userEmail, VotedAt = DateTime.UtcNow };
        await _voteRepository.AddAsync(vote);
        await _ideaRepository.IncrementVoteCountAsync(ideaId);

        return Ok("Voto registrado com sucesso.");
    }

    [HttpDelete]
    public async Task<IActionResult> RemoveVote(int ideaId)
    {
        var userEmail = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var vote = await _voteRepository.GetVoteAsync(ideaId, userEmail);

        if (vote == null) return NotFound("Voto não encontrado para ser removido.");

        await _voteRepository.DeleteAsync(vote);
        await _ideaRepository.DecrementVoteCountAsync(ideaId);

        return NoContent();
    }
}