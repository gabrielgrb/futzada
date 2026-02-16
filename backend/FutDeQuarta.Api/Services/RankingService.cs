using Microsoft.EntityFrameworkCore;
using FutDeQuarta.Api.Data;
using FutDeQuarta.Api.DTOs;
using FutDeQuarta.Api.Models;

namespace FutDeQuarta.Api.Services;

public interface IRankingService
{
    Task<List<RankingJogadorDto>> CalcularRankingGeralAsync(FiltroRankingDto? filtro);
    Task<List<RankingJogadorDto>> ArtilhariaAsync(FiltroRankingDto? filtro);
    Task<List<RankingJogadorDto>> AssistenciasAsync(FiltroRankingDto? filtro);
    Task<List<RankingJogadorDto>> ParticipacaoGolsAsync(FiltroRankingDto? filtro);
    Task<List<RankingConfiguracaoDto>> ObterConfiguracoesAsync();
    Task<RankingConfiguracaoDto?> AtualizarConfiguracaoAsync(int id, AtualizarRankingConfiguracaoDto dto);
}

public class RankingService : IRankingService
{
    private readonly AppDbContext _db;

    public RankingService(AppDbContext db)
    {
        _db = db;
    }

    public async Task<List<RankingJogadorDto>> CalcularRankingGeralAsync(FiltroRankingDto? filtro)
    {
        var config = await ObterPontosConfigAsync();
        var stats = await CalcularEstatisticasAsync(filtro);

        var ranking = stats
            .Select(s => new RankingJogadorDto(
                0, // position set below
                s.JogadorId,
                s.Nome,
                s.Apelido,
                s.Partidas,
                s.Vitorias,
                s.Empates,
                s.Derrotas,
                s.Gols,
                s.Assistencias,
                s.Gols + s.Assistencias,
                s.GolsContra,
                (s.Vitorias * config.PontosVitoria) +
                (s.Empates * config.PontosEmpate) +
                (s.Derrotas * config.PontosDerrota) +
                (s.Gols * config.PontosGol) +
                (s.Assistencias * config.PontosAssistencia) +
                (s.GolsContra * config.PontosGolContra)
            ))
            .OrderByDescending(r => r.PontuacaoTotal)
            .ThenByDescending(r => r.Vitorias)
            .ThenByDescending(r => r.Gols)
            .ToList();

        // Assign positions
        return ranking.Select((r, i) => r with { Posicao = i + 1 }).ToList();
    }

    public async Task<List<RankingJogadorDto>> ArtilhariaAsync(FiltroRankingDto? filtro)
    {
        var ranking = await CalcularRankingGeralAsync(filtro);
        return ranking
            .OrderByDescending(r => r.Gols)
            .ThenByDescending(r => r.Partidas > 0 ? (double)r.Gols / r.Partidas : 0)
            .Select((r, i) => r with { Posicao = i + 1 })
            .ToList();
    }

    public async Task<List<RankingJogadorDto>> AssistenciasAsync(FiltroRankingDto? filtro)
    {
        var ranking = await CalcularRankingGeralAsync(filtro);
        return ranking
            .OrderByDescending(r => r.Assistencias)
            .ThenByDescending(r => r.Partidas > 0 ? (double)r.Assistencias / r.Partidas : 0)
            .Select((r, i) => r with { Posicao = i + 1 })
            .ToList();
    }

    public async Task<List<RankingJogadorDto>> ParticipacaoGolsAsync(FiltroRankingDto? filtro)
    {
        var ranking = await CalcularRankingGeralAsync(filtro);
        return ranking
            .OrderByDescending(r => r.ParticipacaoGols)
            .ThenByDescending(r => r.Gols)
            .Select((r, i) => r with { Posicao = i + 1 })
            .ToList();
    }

    public async Task<List<RankingConfiguracaoDto>> ObterConfiguracoesAsync()
    {
        return await _db.RankingConfiguracoes
            .OrderBy(c => c.Id)
            .Select(c => new RankingConfiguracaoDto(c.Id, c.Chave, c.Valor, c.Descricao))
            .ToListAsync();
    }

    public async Task<RankingConfiguracaoDto?> AtualizarConfiguracaoAsync(int id, AtualizarRankingConfiguracaoDto dto)
    {
        var config = await _db.RankingConfiguracoes.FindAsync(id);
        if (config is null) return null;

        config.Valor = dto.Valor;
        await _db.SaveChangesAsync();

        return new RankingConfiguracaoDto(config.Id, config.Chave, config.Valor, config.Descricao);
    }

    private async Task<List<EstatisticaJogador>> CalcularEstatisticasAsync(FiltroRankingDto? filtro)
    {
        // Get all finished matches optionally filtered
        var partidasQuery = _db.Partidas
            .Where(p => p.Status == StatusPartida.Finalizada);

        if (filtro is not null)
        {
            if (filtro.Ano.HasValue)
                partidasQuery = partidasQuery.Where(p => p.SessaoJogo.Data.Year == filtro.Ano.Value);
            if (filtro.Mes.HasValue)
                partidasQuery = partidasQuery.Where(p => p.SessaoJogo.Data.Month == filtro.Mes.Value);
            if (filtro.DataInicio.HasValue)
                partidasQuery = partidasQuery.Where(p => p.SessaoJogo.Data >= filtro.DataInicio.Value);
            if (filtro.DataFim.HasValue)
                partidasQuery = partidasQuery.Where(p => p.SessaoJogo.Data <= filtro.DataFim.Value);
        }

        var partidaIds = await partidasQuery.Select(p => p.Id).ToListAsync();

        // Get participations for those matches
        var participacoes = await _db.ParticipacaoPartidas
            .Where(pp => partidaIds.Contains(pp.PartidaId))
            .Include(pp => pp.Jogador)
            .ToListAsync();

        // Get goals
        var gols = await _db.Gols
            .Where(g => partidaIds.Contains(g.PartidaId))
            .ToListAsync();

        // Get assists
        var golIds = gols.Select(g => g.Id).ToList();
        var assistencias = await _db.Assistencias
            .Where(a => golIds.Contains(a.GolId))
            .ToListAsync();

        // Group by player
        var jogadorIds = participacoes.Select(pp => pp.JogadorId).Distinct();

        var stats = new List<EstatisticaJogador>();

        foreach (var jogadorId in jogadorIds)
        {
            var jogadorParticipacoes = participacoes.Where(pp => pp.JogadorId == jogadorId).ToList();
            var jogador = jogadorParticipacoes.First().Jogador;

            var jogadorGols = gols.Count(g => g.JogadorId == jogadorId && !g.GolContra);
            var jogadorGolsContra = gols.Count(g => g.JogadorId == jogadorId && g.GolContra);
            var jogadorAssistencias = assistencias.Count(a => a.JogadorId == jogadorId);

            stats.Add(new EstatisticaJogador
            {
                JogadorId = jogadorId,
                Nome = jogador.Nome,
                Apelido = jogador.Apelido,
                Partidas = jogadorParticipacoes.Count,
                Vitorias = jogadorParticipacoes.Count(pp => pp.Resultado == ResultadoPartida.Vitoria),
                Empates = jogadorParticipacoes.Count(pp => pp.Resultado == ResultadoPartida.Empate),
                Derrotas = jogadorParticipacoes.Count(pp => pp.Resultado == ResultadoPartida.Derrota),
                Gols = jogadorGols,
                Assistencias = jogadorAssistencias,
                GolsContra = jogadorGolsContra
            });
        }

        return stats;
    }

    private async Task<PontosConfig> ObterPontosConfigAsync()
    {
        var configs = await _db.RankingConfiguracoes.ToListAsync();

        return new PontosConfig
        {
            PontosVitoria = configs.FirstOrDefault(c => c.Chave == "PontosVitoria")?.Valor ?? 3,
            PontosEmpate = configs.FirstOrDefault(c => c.Chave == "PontosEmpate")?.Valor ?? 1,
            PontosDerrota = configs.FirstOrDefault(c => c.Chave == "PontosDerrota")?.Valor ?? 0,
            PontosGol = configs.FirstOrDefault(c => c.Chave == "PontosGol")?.Valor ?? 2,
            PontosAssistencia = configs.FirstOrDefault(c => c.Chave == "PontosAssistencia")?.Valor ?? 1,
            PontosGolContra = configs.FirstOrDefault(c => c.Chave == "PontosGolContra")?.Valor ?? -1
        };
    }

    private class EstatisticaJogador
    {
        public int JogadorId { get; set; }
        public string Nome { get; set; } = string.Empty;
        public string? Apelido { get; set; }
        public int Partidas { get; set; }
        public int Vitorias { get; set; }
        public int Empates { get; set; }
        public int Derrotas { get; set; }
        public int Gols { get; set; }
        public int Assistencias { get; set; }
        public int GolsContra { get; set; }
    }

    private class PontosConfig
    {
        public int PontosVitoria { get; set; }
        public int PontosEmpate { get; set; }
        public int PontosDerrota { get; set; }
        public int PontosGol { get; set; }
        public int PontosAssistencia { get; set; }
        public int PontosGolContra { get; set; }
    }
}
