using System.ComponentModel.DataAnnotations;

namespace FutDeQuarta.Api.Models;

/// <summary>
/// Configurações de pontuação do ranking.
/// Permite alterar os valores de pontos dinamicamente.
/// </summary>
public class RankingConfiguracao
{
    public int Id { get; set; }

    [Required]
    [MaxLength(50)]
    public string Chave { get; set; } = string.Empty;

    public int Valor { get; set; }

    [MaxLength(200)]
    public string? Descricao { get; set; }
}
