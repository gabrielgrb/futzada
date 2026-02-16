using Microsoft.EntityFrameworkCore;
using FutDeQuarta.Api.Models;

namespace FutDeQuarta.Api.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Jogador> Jogadores => Set<Jogador>();
    public DbSet<SessaoJogo> SessoesJogo => Set<SessaoJogo>();
    public DbSet<ListaChegada> ListaChegadas => Set<ListaChegada>();
    public DbSet<Partida> Partidas => Set<Partida>();
    public DbSet<Time> Times => Set<Time>();
    public DbSet<ParticipacaoPartida> ParticipacaoPartidas => Set<ParticipacaoPartida>();
    public DbSet<Gol> Gols => Set<Gol>();
    public DbSet<Assistencia> Assistencias => Set<Assistencia>();
    public DbSet<RankingConfiguracao> RankingConfiguracoes => Set<RankingConfiguracao>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Jogador
        modelBuilder.Entity<Jogador>(entity =>
        {
            entity.HasIndex(e => e.Nome);
            entity.HasIndex(e => e.Apelido);
        });

        // SessaoJogo
        modelBuilder.Entity<SessaoJogo>(entity =>
        {
            entity.HasIndex(e => e.Data);
        });

        // ListaChegada - unique per session+player
        modelBuilder.Entity<ListaChegada>(entity =>
        {
            entity.HasIndex(e => new { e.SessaoJogoId, e.JogadorId }).IsUnique();
            entity.HasIndex(e => new { e.SessaoJogoId, e.OrdemChegada }).IsUnique();

            entity.HasOne(e => e.SessaoJogo)
                .WithMany(s => s.ListaChegadas)
                .HasForeignKey(e => e.SessaoJogoId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.Jogador)
                .WithMany(j => j.ListaChegadas)
                .HasForeignKey(e => e.JogadorId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // Partida
        modelBuilder.Entity<Partida>(entity =>
        {
            entity.HasIndex(e => new { e.SessaoJogoId, e.Numero }).IsUnique();

            entity.HasOne(e => e.SessaoJogo)
                .WithMany(s => s.Partidas)
                .HasForeignKey(e => e.SessaoJogoId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // Time
        modelBuilder.Entity<Time>(entity =>
        {
            entity.HasOne(e => e.Partida)
                .WithMany(p => p.Times)
                .HasForeignKey(e => e.PartidaId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // ParticipacaoPartida - unique per match+player
        modelBuilder.Entity<ParticipacaoPartida>(entity =>
        {
            entity.HasIndex(e => new { e.PartidaId, e.JogadorId }).IsUnique();

            entity.HasOne(e => e.Partida)
                .WithMany()
                .HasForeignKey(e => e.PartidaId)
                .OnDelete(DeleteBehavior.NoAction);

            entity.HasOne(e => e.Time)
                .WithMany(t => t.Participacoes)
                .HasForeignKey(e => e.TimeId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.Jogador)
                .WithMany(j => j.Participacoes)
                .HasForeignKey(e => e.JogadorId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // Gol
        modelBuilder.Entity<Gol>(entity =>
        {
            entity.HasOne(e => e.Partida)
                .WithMany(p => p.Gols)
                .HasForeignKey(e => e.PartidaId)
                .OnDelete(DeleteBehavior.NoAction);

            entity.HasOne(e => e.Time)
                .WithMany(t => t.Gols)
                .HasForeignKey(e => e.TimeId)
                .OnDelete(DeleteBehavior.NoAction);

            entity.HasOne(e => e.Jogador)
                .WithMany(j => j.Gols)
                .HasForeignKey(e => e.JogadorId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // Assistencia - one per goal (optional)
        modelBuilder.Entity<Assistencia>(entity =>
        {
            entity.HasIndex(e => e.GolId).IsUnique();

            entity.HasOne(e => e.Gol)
                .WithOne(g => g.Assistencia)
                .HasForeignKey<Assistencia>(e => e.GolId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.Jogador)
                .WithMany(j => j.Assistencias)
                .HasForeignKey(e => e.JogadorId)
                .OnDelete(DeleteBehavior.NoAction);
        });

        // RankingConfiguracao
        modelBuilder.Entity<RankingConfiguracao>(entity =>
        {
            entity.HasIndex(e => e.Chave).IsUnique();
        });

        // Seed ranking configuration defaults
        modelBuilder.Entity<RankingConfiguracao>().HasData(
            new RankingConfiguracao { Id = 1, Chave = "PontosVitoria", Valor = 3, Descricao = "Pontos por vitória" },
            new RankingConfiguracao { Id = 2, Chave = "PontosEmpate", Valor = 1, Descricao = "Pontos por empate" },
            new RankingConfiguracao { Id = 3, Chave = "PontosDerrota", Valor = 0, Descricao = "Pontos por derrota" },
            new RankingConfiguracao { Id = 4, Chave = "PontosGol", Valor = 2, Descricao = "Pontos por gol marcado" },
            new RankingConfiguracao { Id = 5, Chave = "PontosAssistencia", Valor = 1, Descricao = "Pontos por assistência" },
            new RankingConfiguracao { Id = 6, Chave = "PontosGolContra", Valor = -1, Descricao = "Pontos por gol contra" }
        );
    }
}
