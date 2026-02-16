using Microsoft.EntityFrameworkCore;
using FutDeQuarta.Api.Data;
using FutDeQuarta.Api.DTOs;
using FutDeQuarta.Api.Models;

namespace FutDeQuarta.Api.Services;

public interface IJogadorService
{
    Task<List<JogadorDto>> ListarTodosAsync();
    Task<List<JogadorDto>> ListarAtivosAsync();
    Task<JogadorDto?> ObterPorIdAsync(int id);
    Task<JogadorDto> CriarAsync(CriarJogadorDto dto);
    Task<JogadorDto?> AtualizarAsync(int id, AtualizarJogadorDto dto);
    Task<bool> RemoverAsync(int id);
}

public class JogadorService : IJogadorService
{
    private readonly AppDbContext _db;

    public JogadorService(AppDbContext db)
    {
        _db = db;
    }

    public async Task<List<JogadorDto>> ListarTodosAsync()
    {
        return await _db.Jogadores
            .OrderBy(j => j.Nome)
            .Select(j => new JogadorDto(j.Id, j.Nome, j.Apelido, j.Ativo))
            .ToListAsync();
    }

    public async Task<List<JogadorDto>> ListarAtivosAsync()
    {
        return await _db.Jogadores
            .Where(j => j.Ativo)
            .OrderBy(j => j.Nome)
            .Select(j => new JogadorDto(j.Id, j.Nome, j.Apelido, j.Ativo))
            .ToListAsync();
    }

    public async Task<JogadorDto?> ObterPorIdAsync(int id)
    {
        var j = await _db.Jogadores.FindAsync(id);
        return j is null ? null : new JogadorDto(j.Id, j.Nome, j.Apelido, j.Ativo);
    }

    public async Task<JogadorDto> CriarAsync(CriarJogadorDto dto)
    {
        var jogador = new Jogador
        {
            Nome = dto.Nome,
            Apelido = dto.Apelido
        };

        _db.Jogadores.Add(jogador);
        await _db.SaveChangesAsync();

        return new JogadorDto(jogador.Id, jogador.Nome, jogador.Apelido, jogador.Ativo);
    }

    public async Task<JogadorDto?> AtualizarAsync(int id, AtualizarJogadorDto dto)
    {
        var jogador = await _db.Jogadores.FindAsync(id);
        if (jogador is null) return null;

        jogador.Nome = dto.Nome;
        jogador.Apelido = dto.Apelido;
        jogador.Ativo = dto.Ativo;

        await _db.SaveChangesAsync();

        return new JogadorDto(jogador.Id, jogador.Nome, jogador.Apelido, jogador.Ativo);
    }

    public async Task<bool> RemoverAsync(int id)
    {
        var jogador = await _db.Jogadores.FindAsync(id);
        if (jogador is null) return false;

        _db.Jogadores.Remove(jogador);
        await _db.SaveChangesAsync();
        return true;
    }
}
