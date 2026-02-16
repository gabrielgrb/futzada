namespace FutDeQuarta.Api.Models;

/// <summary>
/// Liga um jogador a um time em uma partida.
/// Registra se é goleiro e o resultado individual.
/// </summary>
public class ParticipacaoPartida
{
    public int Id { get; set; }

    public int PartidaId { get; set; }
    public Partida Partida { get; set; } = null!;

    public int TimeId { get; set; }
    public Time Time { get; set; } = null!;

    public int JogadorId { get; set; }
    public Jogador Jogador { get; set; } = null!;

    public bool EhGoleiro { get; set; } = false;

    public ResultadoPartida? Resultado { get; set; }
}
