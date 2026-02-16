namespace FutDeQuarta.Api.Models;

public class Assistencia
{
    public int Id { get; set; }

    public int GolId { get; set; }
    public Gol Gol { get; set; } = null!;

    public int JogadorId { get; set; }
    public Jogador Jogador { get; set; } = null!;

    public DateTime CriadoEm { get; set; } = DateTime.UtcNow;
}
