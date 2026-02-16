namespace FutDeQuarta.Api.Models;

public enum ModoGoleiro
{
    Fixo = 0,
    Rotativo = 1
}

public enum ModoTermino
{
    Tempo = 0,    // 7 minutos
    Gols = 1      // 2 gols
}

public enum StatusPartida
{
    Aguardando = 0,
    EmAndamento = 1,
    Finalizada = 2
}

public enum ResultadoPartida
{
    Vitoria = 0,
    Empate = 1,
    Derrota = 2
}
