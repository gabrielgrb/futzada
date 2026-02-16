using FutDeQuarta.Api.Models;

namespace FutDeQuarta.Api.DTOs;

public record PartidaDto(
    int Id,
    int SessaoJogoId,
    int Numero,
    StatusPartida Status,
    bool Empate,
    ModoTermino ModoTermino,
    int DuracaoSegundos,
    DateTime? IniciadaEm,
    DateTime? FinalizadaEm,
    TimeDto? TimeA,
    TimeDto? TimeB,
    List<GolDto> Gols
);

public record TimeDto(
    int Id,
    string Nome,
    string? Cor,
    int GolsMarcados,
    bool Vencedor,
    List<ParticipacaoDto> Jogadores
);

public record ParticipacaoDto(
    int Id,
    int JogadorId,
    string NomeJogador,
    string? Apelido,
    bool EhGoleiro,
    ResultadoPartida? Resultado
);

public record GolDto(
    int Id,
    int JogadorId,
    string NomeJogador,
    int TimeId,
    string NomeTime,
    int? Minuto,
    bool GolContra,
    AssistenciaDto? Assistencia
);

public record AssistenciaDto(int Id, int JogadorId, string NomeJogador);

public record RegistrarGolDto(int JogadorId, int TimeId, int? Minuto, bool GolContra, int? AssistenciaJogadorId);

public record CriarPartidaDto(int SessaoJogoId);

public record FormarTimesDto(int SessaoJogoId);

public record FinalizarPartidaDto(int? VencedorTimeId, bool Empate);
