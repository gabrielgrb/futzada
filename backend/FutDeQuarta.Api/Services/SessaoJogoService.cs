using Microsoft.EntityFrameworkCore;
using FutDeQuarta.Api.Data;
using FutDeQuarta.Api.DTOs;
using FutDeQuarta.Api.Models;

namespace FutDeQuarta.Api.Services;

public interface ISessaoJogoService
{
    Task<List<SessaoJogoDto>> ListarAsync();
    Task<SessaoJogoDto?> ObterPorIdAsync(int id);
    Task<SessaoJogoDto> CriarAsync(CriarSessaoJogoDto dto);
    Task<ListaChegadaDto?> AdicionarChegadaAsync(int sessaoId, AdicionarChegadaDto dto);
    Task<bool> RemoverChegadaAsync(int sessaoId, int jogadorId);
    Task<List<ListaChegadaDto>> ObterListaChegadaAsync(int sessaoId);
}

public class SessaoJogoService : ISessaoJogoService
{
    private readonly AppDbContext _db;

    public SessaoJogoService(AppDbContext db)
    {
        _db = db;
    }

    public async Task<List<SessaoJogoDto>> ListarAsync()
    {
        return await _db.SessoesJogo
            .OrderByDescending(s => s.Data)
            .Select(s => new SessaoJogoDto(
                s.Id,
                s.Data,
                s.Local,
                s.ModoGoleiro,
                s.ListaChegadas
                    .OrderBy(l => l.OrdemChegada)
                    .Select(l => new ListaChegadaDto(
                        l.Id, l.JogadorId, l.Jogador.Nome, l.Jogador.Apelido,
                        l.OrdemChegada, l.HoraChegada))
                    .ToList(),
                s.Partidas.Count
            ))
            .ToListAsync();
    }

    public async Task<SessaoJogoDto?> ObterPorIdAsync(int id)
    {
        return await _db.SessoesJogo
            .Where(s => s.Id == id)
            .Select(s => new SessaoJogoDto(
                s.Id,
                s.Data,
                s.Local,
                s.ModoGoleiro,
                s.ListaChegadas
                    .OrderBy(l => l.OrdemChegada)
                    .Select(l => new ListaChegadaDto(
                        l.Id, l.JogadorId, l.Jogador.Nome, l.Jogador.Apelido,
                        l.OrdemChegada, l.HoraChegada))
                    .ToList(),
                s.Partidas.Count
            ))
            .FirstOrDefaultAsync();
    }

    public async Task<SessaoJogoDto> CriarAsync(CriarSessaoJogoDto dto)
    {
        var sessao = new SessaoJogo
        {
            Data = dto.Data,
            Local = dto.Local,
            ModoGoleiro = dto.ModoGoleiro
        };

        _db.SessoesJogo.Add(sessao);
        await _db.SaveChangesAsync();

        return new SessaoJogoDto(sessao.Id, sessao.Data, sessao.Local, sessao.ModoGoleiro, new List<ListaChegadaDto>(), 0);
    }

    public async Task<ListaChegadaDto?> AdicionarChegadaAsync(int sessaoId, AdicionarChegadaDto dto)
    {
        var sessao = await _db.SessoesJogo.FindAsync(sessaoId);
        if (sessao is null) return null;

        var jogador = await _db.Jogadores.FindAsync(dto.JogadorId);
        if (jogador is null) return null;

        // Check if player already in list
        var jaExiste = await _db.ListaChegadas
            .AnyAsync(l => l.SessaoJogoId == sessaoId && l.JogadorId == dto.JogadorId);
        if (jaExiste) return null;

        // Get next order number
        var proximaOrdem = await _db.ListaChegadas
            .Where(l => l.SessaoJogoId == sessaoId)
            .MaxAsync(l => (int?)l.OrdemChegada) ?? 0;

        var chegada = new ListaChegada
        {
            SessaoJogoId = sessaoId,
            JogadorId = dto.JogadorId,
            OrdemChegada = proximaOrdem + 1
        };

        _db.ListaChegadas.Add(chegada);
        await _db.SaveChangesAsync();

        return new ListaChegadaDto(chegada.Id, jogador.Id, jogador.Nome, jogador.Apelido, chegada.OrdemChegada, chegada.HoraChegada);
    }

    public async Task<bool> RemoverChegadaAsync(int sessaoId, int jogadorId)
    {
        var chegada = await _db.ListaChegadas
            .FirstOrDefaultAsync(l => l.SessaoJogoId == sessaoId && l.JogadorId == jogadorId);

        if (chegada is null) return false;

        _db.ListaChegadas.Remove(chegada);
        await _db.SaveChangesAsync();
        return true;
    }

    public async Task<List<ListaChegadaDto>> ObterListaChegadaAsync(int sessaoId)
    {
        return await _db.ListaChegadas
            .Where(l => l.SessaoJogoId == sessaoId)
            .OrderBy(l => l.OrdemChegada)
            .Select(l => new ListaChegadaDto(
                l.Id, l.JogadorId, l.Jogador.Nome, l.Jogador.Apelido,
                l.OrdemChegada, l.HoraChegada))
            .ToListAsync();
    }
}
