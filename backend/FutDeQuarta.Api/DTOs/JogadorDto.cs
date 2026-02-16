namespace FutDeQuarta.Api.DTOs;

public record JogadorDto(int Id, string Nome, string? Apelido, bool Ativo);

public record CriarJogadorDto(string Nome, string? Apelido);

public record AtualizarJogadorDto(string Nome, string? Apelido, bool Ativo);
