using System.ComponentModel.DataAnnotations;

namespace FutDeQuarta.Api.Models;

/// <summary>
/// Representa um dia de jogo (uma "quarta-feira" de futebol).
/// Agrupa a lista de chegada, times e partidas daquele dia.
/// </summary>
public class SessaoJogo
{
    public int Id { get; set; }

    public DateTime Data { get; set; }

    [MaxLength(100)]
    public string? Local { get; set; }

    public ModoGoleiro ModoGoleiro { get; set; } = ModoGoleiro.Rotativo;

    public DateTime CriadoEm { get; set; } = DateTime.UtcNow;

    // Navigation properties
    public ICollection<ListaChegada> ListaChegadas { get; set; } = new List<ListaChegada>();
    public ICollection<Partida> Partidas { get; set; } = new List<Partida>();
}
