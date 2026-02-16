namespace FutDeQuarta.Api.Models;

/// <summary>
/// Registra a ordem de chegada dos jogadores em uma sessão.
/// A ordem define a prioridade para jogar.
/// </summary>
public class ListaChegada
{
    public int Id { get; set; }

    public int SessaoJogoId { get; set; }
    public SessaoJogo SessaoJogo { get; set; } = null!;

    public int JogadorId { get; set; }
    public Jogador Jogador { get; set; } = null!;

    public int OrdemChegada { get; set; }

    public DateTime HoraChegada { get; set; } = DateTime.UtcNow;
}
