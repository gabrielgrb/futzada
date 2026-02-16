using Microsoft.AspNetCore.Mvc;
using FutDeQuarta.Api.DTOs;
using FutDeQuarta.Api.Services;

namespace FutDeQuarta.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class RankingController : ControllerBase
{
    private readonly IRankingService _service;

    public RankingController(IRankingService service)
    {
        _service = service;
    }

    [HttpGet("geral")]
    public async Task<ActionResult<List<RankingJogadorDto>>> RankingGeral(
        [FromQuery] int? ano, [FromQuery] int? mes,
        [FromQuery] DateTime? dataInicio, [FromQuery] DateTime? dataFim)
    {
        var filtro = new FiltroRankingDto { Ano = ano, Mes = mes, DataInicio = dataInicio, DataFim = dataFim };
        return Ok(await _service.CalcularRankingGeralAsync(filtro));
    }

    [HttpGet("artilharia")]
    public async Task<ActionResult<List<RankingJogadorDto>>> Artilharia(
        [FromQuery] int? ano, [FromQuery] int? mes,
        [FromQuery] DateTime? dataInicio, [FromQuery] DateTime? dataFim)
    {
        var filtro = new FiltroRankingDto { Ano = ano, Mes = mes, DataInicio = dataInicio, DataFim = dataFim };
        return Ok(await _service.ArtilhariaAsync(filtro));
    }

    [HttpGet("assistencias")]
    public async Task<ActionResult<List<RankingJogadorDto>>> Assistencias(
        [FromQuery] int? ano, [FromQuery] int? mes,
        [FromQuery] DateTime? dataInicio, [FromQuery] DateTime? dataFim)
    {
        var filtro = new FiltroRankingDto { Ano = ano, Mes = mes, DataInicio = dataInicio, DataFim = dataFim };
        return Ok(await _service.AssistenciasAsync(filtro));
    }

    [HttpGet("participacao-gols")]
    public async Task<ActionResult<List<RankingJogadorDto>>> ParticipacaoGols(
        [FromQuery] int? ano, [FromQuery] int? mes,
        [FromQuery] DateTime? dataInicio, [FromQuery] DateTime? dataFim)
    {
        var filtro = new FiltroRankingDto { Ano = ano, Mes = mes, DataInicio = dataInicio, DataFim = dataFim };
        return Ok(await _service.ParticipacaoGolsAsync(filtro));
    }

    [HttpGet("configuracoes")]
    public async Task<ActionResult<List<RankingConfiguracaoDto>>> Configuracoes()
    {
        return Ok(await _service.ObterConfiguracoesAsync());
    }

    [HttpPut("configuracoes/{id}")]
    public async Task<ActionResult<RankingConfiguracaoDto>> AtualizarConfiguracao(int id, AtualizarRankingConfiguracaoDto dto)
    {
        var config = await _service.AtualizarConfiguracaoAsync(id, dto);
        return config is null ? NotFound() : Ok(config);
    }
}
