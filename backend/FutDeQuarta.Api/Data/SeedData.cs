using FutDeQuarta.Api.Models;

namespace FutDeQuarta.Api.Data;

public static class SeedData
{
    public static async Task SeedAsync(AppDbContext context)
    {
        if (context.Jogadores.Any()) return;

        var jogadores = new List<Jogador>
        {
            new() { Nome = "Gabriel", Apelido = "Gabigol" },
            new() { Nome = "Lucas", Apelido = "Luquinha" },
            new() { Nome = "Rafael", Apelido = "Rafa" },
            new() { Nome = "Pedro", Apelido = "Pedrão" },
            new() { Nome = "Matheus", Apelido = "Matheus" },
            new() { Nome = "Bruno", Apelido = "Bruninho" },
            new() { Nome = "Felipe", Apelido = "Felipão" },
            new() { Nome = "Thiago", Apelido = "Thiago" },
            new() { Nome = "Carlos", Apelido = "Carlão" },
            new() { Nome = "Anderson", Apelido = "Anderson" },
            new() { Nome = "Diego", Apelido = "Didico" },
            new() { Nome = "Rodrigo", Apelido = "Digão" },
            new() { Nome = "Marcelo", Apelido = "Marcelinho" },
            new() { Nome = "Vinícius", Apelido = "Vini" },
            new() { Nome = "João", Apelido = "Joãozinho" },
        };

        context.Jogadores.AddRange(jogadores);
        await context.SaveChangesAsync();
    }
}
