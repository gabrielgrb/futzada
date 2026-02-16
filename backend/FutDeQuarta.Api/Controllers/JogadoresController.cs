using Microsoft.AspNetCore.Mvc;
using FutDeQuarta.Api.DTOs;
using FutDeQuarta.Api.Services;

namespace FutDeQuarta.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class JogadoresController : ControllerBase
{
    private readonly IJogadorService _service;

    public JogadoresController(IJogadorService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<ActionResult<List<JogadorDto>>> Listar([FromQuery] bool apenasAtivos = false)
    {
        var jogadores = apenasAtivos
            ? await _service.ListarAtivosAsync()
            : await _service.ListarTodosAsync();
        return Ok(jogadores);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<JogadorDto>> ObterPorId(int id)
    {
        var jogador = await _service.ObterPorIdAsync(id);
        return jogador is null ? NotFound() : Ok(jogador);
    }

    [HttpPost]
    public async Task<ActionResult<JogadorDto>> Criar(CriarJogadorDto dto)
    {
        var jogador = await _service.CriarAsync(dto);
        return CreatedAtAction(nameof(ObterPorId), new { id = jogador.Id }, jogador);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<JogadorDto>> Atualizar(int id, AtualizarJogadorDto dto)
    {
        var jogador = await _service.AtualizarAsync(id, dto);
        return jogador is null ? NotFound() : Ok(jogador);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Remover(int id)
    {
        return await _service.RemoverAsync(id) ? NoContent() : NotFound();
    }
}
