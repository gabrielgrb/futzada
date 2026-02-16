using Microsoft.AspNetCore.Mvc;
using FutDeQuarta.Api.DTOs;
using FutDeQuarta.Api.Services;

namespace FutDeQuarta.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PartidasController : ControllerBase
{
    private readonly IPartidaService _service;

    public PartidasController(IPartidaService service)
    {
        _service = service;
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<PartidaDto>> ObterPorId(int id)
    {
        var partida = await _service.ObterPorIdAsync(id);
        return partida is null ? NotFound() : Ok(partida);
    }

    [HttpGet("sessao/{sessaoJogoId}")]
    public async Task<ActionResult<List<PartidaDto>>> ListarPorSessao(int sessaoJogoId)
    {
        return Ok(await _service.ListarPorSessaoAsync(sessaoJogoId));
    }

    [HttpPost("formar-times")]
    public async Task<ActionResult<PartidaDto>> FormarTimes(FormarTimesDto dto)
    {
        var partida = await _service.FormarTimesECriarPartidaAsync(dto.SessaoJogoId);
        if (partida is null)
            return BadRequest("Não há jogadores suficientes (mínimo 10) ou sessão não encontrada.");
        return Ok(partida);
    }

    [HttpPost("{id}/iniciar")]
    public async Task<ActionResult<PartidaDto>> Iniciar(int id)
    {
        var partida = await _service.IniciarPartidaAsync(id);
        return partida is null ? BadRequest("Partida não pode ser iniciada.") : Ok(partida);
    }

    [HttpPost("{id}/finalizar")]
    public async Task<ActionResult<PartidaDto>> Finalizar(int id)
    {
        var partida = await _service.FinalizarPartidaAsync(id);
        return partida is null ? BadRequest("Partida não pode ser finalizada.") : Ok(partida);
    }

    [HttpPost("{id}/gols")]
    public async Task<ActionResult<GolDto>> RegistrarGol(int id, RegistrarGolDto dto)
    {
        var gol = await _service.RegistrarGolAsync(id, dto);
        return gol is null ? BadRequest("Não foi possível registrar o gol.") : Ok(gol);
    }

    [HttpGet("historico")]
    public async Task<ActionResult<List<PartidaDto>>> Historico(
        [FromQuery] int? ano,
        [FromQuery] int? mes,
        [FromQuery] DateTime? dataInicio,
        [FromQuery] DateTime? dataFim)
    {
        return Ok(await _service.HistoricoAsync(ano, mes, dataInicio, dataFim));
    }
}
