using Microsoft.EntityFrameworkCore;
using FutDeQuarta.Api.Data;
using FutDeQuarta.Api.DTOs;
using FutDeQuarta.Api.Models;

namespace FutDeQuarta.Api.Services;

public interface IPartidaService
{
    Task<PartidaDto?> ObterPorIdAsync(int id);
    Task<List<PartidaDto>> ListarPorSessaoAsync(int sessaoJogoId);
    Task<PartidaDto?> FormarTimesECriarPartidaAsync(int sessaoJogoId);
    Task<PartidaDto?> IniciarPartidaAsync(int partidaId);
    Task<PartidaDto?> FinalizarPartidaAsync(int partidaId);
    Task<GolDto?> RegistrarGolAsync(int partidaId, RegistrarGolDto dto);
    Task<List<PartidaDto>> HistoricoAsync(int? ano, int? mes, DateTime? dataInicio, DateTime? dataFim);
}

public class PartidaService : IPartidaService
{
    private readonly AppDbContext _db;

    public PartidaService(AppDbContext db)
    {
        _db = db;
    }

    public async Task<PartidaDto?> ObterPorIdAsync(int id)
    {
        return await MapPartidaQuery(_db.Partidas.Where(p => p.Id == id)).FirstOrDefaultAsync();
    }

    public async Task<List<PartidaDto>> ListarPorSessaoAsync(int sessaoJogoId)
    {
        return await MapPartidaQuery(_db.Partidas.Where(p => p.SessaoJogoId == sessaoJogoId))
            .OrderBy(p => p.Numero)
            .ToListAsync();
    }

    /// <summary>
    /// Forma dois times de 5 jogadores a partir da lista de chegada e cria uma partida.
    /// Os primeiros 10 jogadores disponíveis (que não estão jogando) são selecionados.
    /// </summary>
    public async Task<PartidaDto?> FormarTimesECriarPartidaAsync(int sessaoJogoId)
    {
        var sessao = await _db.SessoesJogo
            .Include(s => s.ListaChegadas)
                .ThenInclude(l => l.Jogador)
            .FirstOrDefaultAsync(s => s.Id == sessaoJogoId);

        if (sessao is null) return null;

        // Get all players in arrival order
        var jogadoresOrdenados = sessao.ListaChegadas
            .OrderBy(l => l.OrdemChegada)
            .Select(l => l.Jogador)
            .ToList();

        // Find players currently in an active match
        var jogadoresEmPartida = await _db.ParticipacaoPartidas
            .Where(pp => pp.Partida.SessaoJogoId == sessaoJogoId
                && pp.Partida.Status == StatusPartida.EmAndamento)
            .Select(pp => pp.JogadorId)
            .ToListAsync();

        // Get available players (not in active match)
        var disponiveis = jogadoresOrdenados
            .Where(j => !jogadoresEmPartida.Contains(j.Id))
            .ToList();

        if (disponiveis.Count < 10) return null; // Need at least 10 players

        // Take first 10 available: 5 for each team
        var timeAJogadores = disponiveis.Take(5).ToList();
        var timeBJogadores = disponiveis.Skip(5).Take(5).ToList();

        // Get next match number
        var proximoNumero = await _db.Partidas
            .Where(p => p.SessaoJogoId == sessaoJogoId)
            .MaxAsync(p => (int?)p.Numero) ?? 0;

        var partida = new Partida
        {
            SessaoJogoId = sessaoJogoId,
            Numero = proximoNumero + 1,
            Status = StatusPartida.Aguardando
        };

        _db.Partidas.Add(partida);
        await _db.SaveChangesAsync();

        // Create teams
        var timeA = new Time
        {
            PartidaId = partida.Id,
            Nome = "Time A",
            Cor = "Azul"
        };
        var timeB = new Time
        {
            PartidaId = partida.Id,
            Nome = "Time B",
            Cor = "Vermelho"
        };

        _db.Times.AddRange(timeA, timeB);
        await _db.SaveChangesAsync();

        // Add players to teams
        // In rotational goalkeeper mode, first player in each team is goalkeeper
        var participacoes = new List<ParticipacaoPartida>();

        for (int i = 0; i < timeAJogadores.Count; i++)
        {
            participacoes.Add(new ParticipacaoPartida
            {
                PartidaId = partida.Id,
                TimeId = timeA.Id,
                JogadorId = timeAJogadores[i].Id,
                EhGoleiro = sessao.ModoGoleiro == ModoGoleiro.Rotativo && i == 0
            });
        }

        for (int i = 0; i < timeBJogadores.Count; i++)
        {
            participacoes.Add(new ParticipacaoPartida
            {
                PartidaId = partida.Id,
                TimeId = timeB.Id,
                JogadorId = timeBJogadores[i].Id,
                EhGoleiro = sessao.ModoGoleiro == ModoGoleiro.Rotativo && i == 0
            });
        }

        _db.ParticipacaoPartidas.AddRange(participacoes);
        await _db.SaveChangesAsync();

        return await ObterPorIdAsync(partida.Id);
    }

    public async Task<PartidaDto?> IniciarPartidaAsync(int partidaId)
    {
        var partida = await _db.Partidas.FindAsync(partidaId);
        if (partida is null || partida.Status != StatusPartida.Aguardando) return null;

        partida.Status = StatusPartida.EmAndamento;
        partida.IniciadaEm = DateTime.UtcNow;

        await _db.SaveChangesAsync();
        return await ObterPorIdAsync(partidaId);
    }

    /// <summary>
    /// Finaliza a partida calculando o vencedor baseado nos gols.
    /// </summary>
    public async Task<PartidaDto?> FinalizarPartidaAsync(int partidaId)
    {
        var partida = await _db.Partidas
            .Include(p => p.Times)
            .Include(p => p.Gols)
            .FirstOrDefaultAsync(p => p.Id == partidaId);

        if (partida is null || partida.Status != StatusPartida.EmAndamento) return null;

        var times = partida.Times.ToList();
        if (times.Count != 2) return null;

        // Count goals per team (own goals count for the opposing team)
        foreach (var time in times)
        {
            var golsNormais = partida.Gols.Count(g => g.TimeId == time.Id && !g.GolContra);
            var golsContraAdversario = partida.Gols.Count(g => g.TimeId != time.Id && g.GolContra);
            time.GolsMarcados = golsNormais + golsContraAdversario;
        }

        // Determine winner
        if (times[0].GolsMarcados == times[1].GolsMarcados)
        {
            partida.Empate = true;
            times[0].Vencedor = false;
            times[1].Vencedor = false;
        }
        else
        {
            var vencedor = times[0].GolsMarcados > times[1].GolsMarcados ? times[0] : times[1];
            vencedor.Vencedor = true;
            partida.Empate = false;
        }

        // Set player results
        var participacoes = await _db.ParticipacaoPartidas
            .Where(pp => pp.PartidaId == partidaId)
            .ToListAsync();

        foreach (var pp in participacoes)
        {
            var time = times.First(t => t.Id == pp.TimeId);
            if (partida.Empate)
                pp.Resultado = ResultadoPartida.Empate;
            else if (time.Vencedor)
                pp.Resultado = ResultadoPartida.Vitoria;
            else
                pp.Resultado = ResultadoPartida.Derrota;
        }

        partida.Status = StatusPartida.Finalizada;
        partida.FinalizadaEm = DateTime.UtcNow;

        if (partida.IniciadaEm.HasValue)
        {
            partida.DuracaoSegundos = (int)(partida.FinalizadaEm.Value - partida.IniciadaEm.Value).TotalSeconds;
        }

        // Determine finish mode
        var maxGols = times.Max(t => t.GolsMarcados);
        partida.ModoTermino = maxGols >= 2 ? ModoTermino.Gols : ModoTermino.Tempo;

        await _db.SaveChangesAsync();
        return await ObterPorIdAsync(partidaId);
    }

    public async Task<GolDto?> RegistrarGolAsync(int partidaId, RegistrarGolDto dto)
    {
        var partida = await _db.Partidas.FindAsync(partidaId);
        if (partida is null || partida.Status != StatusPartida.EmAndamento) return null;

        var jogador = await _db.Jogadores.FindAsync(dto.JogadorId);
        if (jogador is null) return null;

        var time = await _db.Times.FindAsync(dto.TimeId);
        if (time is null) return null;

        var gol = new Gol
        {
            PartidaId = partidaId,
            JogadorId = dto.JogadorId,
            TimeId = dto.TimeId,
            Minuto = dto.Minuto,
            GolContra = dto.GolContra
        };

        _db.Gols.Add(gol);
        await _db.SaveChangesAsync();

        // Register assist if provided
        AssistenciaDto? assistenciaDto = null;
        if (dto.AssistenciaJogadorId.HasValue && !dto.GolContra)
        {
            var assistente = await _db.Jogadores.FindAsync(dto.AssistenciaJogadorId.Value);
            if (assistente is not null)
            {
                var assistencia = new Assistencia
                {
                    GolId = gol.Id,
                    JogadorId = assistente.Id
                };
                _db.Assistencias.Add(assistencia);
                await _db.SaveChangesAsync();
                assistenciaDto = new AssistenciaDto(assistencia.Id, assistente.Id, assistente.Apelido ?? assistente.Nome);
            }
        }

        // Check if match should auto-end (2 goals by any team)
        var golsTime = await _db.Gols
            .Where(g => g.PartidaId == partidaId && g.TimeId == dto.TimeId && !g.GolContra)
            .CountAsync();

        // Also count own goals from the other team
        var golsContraOutroTime = await _db.Gols
            .Where(g => g.PartidaId == partidaId && g.TimeId != dto.TimeId && g.GolContra)
            .CountAsync();

        // No auto-finish here — the frontend cronometer/logic handles it
        // But we return the data so the frontend can decide

        return new GolDto(
            gol.Id, jogador.Id, jogador.Apelido ?? jogador.Nome,
            time.Id, time.Nome, dto.Minuto, dto.GolContra, assistenciaDto);
    }

    public async Task<List<PartidaDto>> HistoricoAsync(int? ano, int? mes, DateTime? dataInicio, DateTime? dataFim)
    {
        var query = _db.Partidas
            .Where(p => p.Status == StatusPartida.Finalizada);

        if (ano.HasValue)
            query = query.Where(p => p.SessaoJogo.Data.Year == ano.Value);

        if (mes.HasValue)
            query = query.Where(p => p.SessaoJogo.Data.Month == mes.Value);

        if (dataInicio.HasValue)
            query = query.Where(p => p.SessaoJogo.Data >= dataInicio.Value);

        if (dataFim.HasValue)
            query = query.Where(p => p.SessaoJogo.Data <= dataFim.Value);

        return await MapPartidaQuery(query)
            .OrderByDescending(p => p.Id)
            .ToListAsync();
    }

    private IQueryable<PartidaDto> MapPartidaQuery(IQueryable<Partida> query)
    {
        return query.Select(p => new PartidaDto(
            p.Id,
            p.SessaoJogoId,
            p.Numero,
            p.Status,
            p.Empate,
            p.ModoTermino,
            p.DuracaoSegundos,
            p.IniciadaEm,
            p.FinalizadaEm,
            p.Times.OrderBy(t => t.Id).Select(t => new TimeDto(
                t.Id,
                t.Nome,
                t.Cor,
                t.GolsMarcados,
                t.Vencedor,
                t.Participacoes.Select(pp => new ParticipacaoDto(
                    pp.Id, pp.JogadorId,
                    pp.Jogador.Apelido ?? pp.Jogador.Nome,
                    pp.Jogador.Apelido,
                    pp.EhGoleiro, pp.Resultado
                )).ToList()
            )).FirstOrDefault(),
            p.Times.OrderBy(t => t.Id).Select(t => new TimeDto(
                t.Id,
                t.Nome,
                t.Cor,
                t.GolsMarcados,
                t.Vencedor,
                t.Participacoes.Select(pp => new ParticipacaoDto(
                    pp.Id, pp.JogadorId,
                    pp.Jogador.Apelido ?? pp.Jogador.Nome,
                    pp.Jogador.Apelido,
                    pp.EhGoleiro, pp.Resultado
                )).ToList()
            )).Skip(1).FirstOrDefault(),
            p.Gols.OrderBy(g => g.CriadoEm).Select(g => new GolDto(
                g.Id, g.JogadorId,
                g.Jogador.Apelido ?? g.Jogador.Nome,
                g.TimeId,
                g.Time.Nome,
                g.Minuto,
                g.GolContra,
                g.Assistencia != null
                    ? new AssistenciaDto(g.Assistencia.Id, g.Assistencia.JogadorId,
                        g.Assistencia.Jogador.Apelido ?? g.Assistencia.Jogador.Nome)
                    : null
            )).ToList()
        ));
    }
}
