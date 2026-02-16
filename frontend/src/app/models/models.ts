// Enums matching backend
export enum ModoGoleiro {
  Fixo = 0,
  Rotativo = 1,
}

export enum ModoTermino {
  Tempo = 0,
  Gols = 1,
}

export enum StatusPartida {
  Aguardando = 0,
  EmAndamento = 1,
  Finalizada = 2,
}

export enum ResultadoPartida {
  Vitoria = 0,
  Empate = 1,
  Derrota = 2,
}

// DTOs
export interface Jogador {
  id: number;
  nome: string;
  apelido: string | null;
  ativo: boolean;
}

export interface SessaoJogo {
  id: number;
  data: string;
  local: string | null;
  modoGoleiro: ModoGoleiro;
  listaChegada: ListaChegada[];
  totalPartidas: number;
}

export interface ListaChegada {
  id: number;
  jogadorId: number;
  nomeJogador: string;
  apelido: string | null;
  ordemChegada: number;
  horaChegada: string;
}

export interface Partida {
  id: number;
  sessaoJogoId: number;
  numero: number;
  status: StatusPartida;
  empate: boolean;
  modoTermino: ModoTermino;
  duracaoSegundos: number;
  iniciadaEm: string | null;
  finalizadaEm: string | null;
  timeA: Time | null;
  timeB: Time | null;
  gols: Gol[];
}

export interface Time {
  id: number;
  nome: string;
  cor: string | null;
  golsMarcados: number;
  vencedor: boolean;
  jogadores: Participacao[];
}

export interface Participacao {
  id: number;
  jogadorId: number;
  nomeJogador: string;
  apelido: string | null;
  ehGoleiro: boolean;
  resultado: ResultadoPartida | null;
}

export interface Gol {
  id: number;
  jogadorId: number;
  nomeJogador: string;
  timeId: number;
  nomeTime: string;
  minuto: number | null;
  golContra: boolean;
  assistencia: Assistencia | null;
}

export interface Assistencia {
  id: number;
  jogadorId: number;
  nomeJogador: string;
}

export interface RankingJogador {
  posicao: number;
  jogadorId: number;
  nome: string;
  apelido: string | null;
  partidas: number;
  vitorias: number;
  empates: number;
  derrotas: number;
  gols: number;
  assistencias: number;
  participacaoGols: number;
  golsContra: number;
  pontuacaoTotal: number;
}

export interface RankingConfiguracao {
  id: number;
  chave: string;
  valor: number;
  descricao: string | null;
}
