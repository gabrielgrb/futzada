using Microsoft.AspNetCore.Mvc;
using FutDeQuarta.Api.DTOs;
using FutDeQuarta.Api.Services;

namespace FutDeQuarta.Api.Controllers;

[ApiController]
[Route("api/sessoes")]
public class SessoesJogoController : ControllerBase
{
    private readonly ISessaoJogoService _service;

    public SessoesJogoController(ISessaoJogoService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<ActionResult<List<SessaoJogoDto>>> Listar()
    {
        return Ok(await _service.ListarAsync());
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<SessaoJogoDto>> ObterPorId(int id)
    {
        var sessao = await _service.ObterPorIdAsync(id);
        return sessao is null ? NotFound() : Ok(sessao);
    }

    [HttpPost]
    public async Task<ActionResult<SessaoJogoDto>> Criar(CriarSessaoJogoDto dto)
    {
        var sessao = await _service.CriarAsync(dto);
        return CreatedAtAction(nameof(ObterPorId), new { id = sessao.Id }, sessao);
    }

    [HttpGet("{sessaoId}/chegadas")]
    public async Task<ActionResult<List<ListaChegadaDto>>> ListaChegada(int sessaoId)
    {
        return Ok(await _service.ObterListaChegadaAsync(sessaoId));
    }

    [HttpPost("{sessaoId}/chegadas")]
    public async Task<ActionResult<ListaChegadaDto>> AdicionarChegada(int sessaoId, AdicionarChegadaDto dto)
    {
        var chegada = await _service.AdicionarChegadaAsync(sessaoId, dto);
        return chegada is null ? BadRequest("Jogador já na lista ou não encontrado.") : Ok(chegada);
    }

    [HttpDelete("{sessaoId}/chegadas/{jogadorId}")]
    public async Task<IActionResult> RemoverChegada(int sessaoId, int jogadorId)
    {
        return await _service.RemoverChegadaAsync(sessaoId, jogadorId) ? NoContent() : NotFound();
    }
}
