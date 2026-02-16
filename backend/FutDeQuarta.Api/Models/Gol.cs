namespace FutDeQuarta.Api.Models;

public class Gol
{
    public int Id { get; set; }

    public int PartidaId { get; set; }
    public Partida Partida { get; set; } = null!;

    public int TimeId { get; set; }
    public Time Time { get; set; } = null!;

    public int JogadorId { get; set; }
    public Jogador Jogador { get; set; } = null!;

    public int? Minuto { get; set; }

    public bool GolContra { get; set; } = false;

    public DateTime CriadoEm { get; set; } = DateTime.UtcNow;

    // Navigation property
    public Assistencia? Assistencia { get; set; }
}
