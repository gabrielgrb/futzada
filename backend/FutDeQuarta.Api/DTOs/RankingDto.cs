namespace FutDeQuarta.Api.DTOs;

public record RankingJogadorDto(
    int Posicao,
    int JogadorId,
    string Nome,
    string? Apelido,
    int Partidas,
    int Vitorias,
    int Empates,
    int Derrotas,
    int Gols,
    int Assistencias,
    int ParticipacaoGols, // gols + assistências
    int GolsContra,
    int PontuacaoTotal
);

public record RankingConfiguracaoDto(int Id, string Chave, int Valor, string? Descricao);

public record AtualizarRankingConfiguracaoDto(int Valor);

public record FiltroRankingDto
{
    public int? Ano { get; init; }
    public int? Mes { get; init; }
    public DateTime? DataInicio { get; init; }
    public DateTime? DataFim { get; init; }
}
