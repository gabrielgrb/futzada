namespace FutDeQuarta.Api.Models;

/// <summary>
/// Representa uma partida dentro de uma sessão de jogo.
/// Termina por tempo (7 min) ou por gols (2 gols).
/// </summary>
public class Partida
{
    public int Id { get; set; }

    public int SessaoJogoId { get; set; }
    public SessaoJogo SessaoJogo { get; set; } = null!;

    public int Numero { get; set; } // Ordem da partida na sessão

    public int DuracaoSegundos { get; set; } = 420; // 7 min padrão

    public ModoTermino ModoTermino { get; set; } = ModoTermino.Tempo;

    public bool Empate { get; set; } = false;

    public StatusPartida Status { get; set; } = StatusPartida.Aguardando;

    public DateTime? IniciadaEm { get; set; }

    public DateTime? FinalizadaEm { get; set; }

    // Navigation properties
    public ICollection<Time> Times { get; set; } = new List<Time>();
    public ICollection<Gol> Gols { get; set; } = new List<Gol>();
}
