using System.ComponentModel.DataAnnotations;

namespace FutDeQuarta.Api.Models;

/// <summary>
/// Representa um time em uma partida (5 jogadores).
/// Cada partida tem exatamente 2 times.
/// </summary>
public class Time
{
    public int Id { get; set; }

    public int PartidaId { get; set; }
    public Partida Partida { get; set; } = null!;

    [MaxLength(50)]
    public string Nome { get; set; } = string.Empty; // Ex: "Time A", "Time B"

    [MaxLength(20)]
    public string? Cor { get; set; } // Ex: "Azul", "Vermelho"

    public int GolsMarcados { get; set; } = 0;

    public bool Vencedor { get; set; } = false;

    // Navigation properties
    public ICollection<ParticipacaoPartida> Participacoes { get; set; } = new List<ParticipacaoPartida>();
    public ICollection<Gol> Gols { get; set; } = new List<Gol>();
}
