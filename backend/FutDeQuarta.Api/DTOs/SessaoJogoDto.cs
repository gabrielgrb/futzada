using FutDeQuarta.Api.Models;

namespace FutDeQuarta.Api.DTOs;

public record SessaoJogoDto(
    int Id,
    DateTime Data,
    string? Local,
    ModoGoleiro ModoGoleiro,
    List<ListaChegadaDto> ListaChegada,
    int TotalPartidas
);

public record CriarSessaoJogoDto(DateTime Data, string? Local, ModoGoleiro ModoGoleiro);

public record ListaChegadaDto(int Id, int JogadorId, string NomeJogador, string? Apelido, int OrdemChegada, DateTime HoraChegada);

public record AdicionarChegadaDto(int JogadorId);
