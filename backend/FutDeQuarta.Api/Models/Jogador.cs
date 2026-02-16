using System.ComponentModel.DataAnnotations;

namespace FutDeQuarta.Api.Models;

public class Jogador
{
    public int Id { get; set; }

    [Required]
    [MaxLength(100)]
    public string Nome { get; set; } = string.Empty;

    [MaxLength(50)]
    public string? Apelido { get; set; }

    public bool Ativo { get; set; } = true;

    public DateTime CriadoEm { get; set; } = DateTime.UtcNow;

    // Navigation properties
    public ICollection<ListaChegada> ListaChegadas { get; set; } = new List<ListaChegada>();
    public ICollection<ParticipacaoPartida> Participacoes { get; set; } = new List<ParticipacaoPartida>();
    public ICollection<Gol> Gols { get; set; } = new List<Gol>();
    public ICollection<Assistencia> Assistencias { get; set; } = new List<Assistencia>();
}
