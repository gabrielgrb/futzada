import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import {
  Jogador,
  SessaoJogo,
  ListaChegada,
  Partida,
  Gol,
  RankingJogador,
  RankingConfiguracao,
  ModoGoleiro,
} from '../models/models';
import { environment } from '../../environments/environment';

@Injectable({ providedIn: 'root' })
export class ApiService {
  private baseUrl = environment.apiUrl;

  constructor(private http: HttpClient) {}

  // ---- Jogadores ----
  getJogadores(apenasAtivos = false): Observable<Jogador[]> {
    return this.http.get<Jogador[]>(`${this.baseUrl}/jogadores`, {
      params: { apenasAtivos: apenasAtivos.toString() },
    });
  }

  getJogador(id: number): Observable<Jogador> {
    return this.http.get<Jogador>(`${this.baseUrl}/jogadores/${id}`);
  }

  criarJogador(nome: string, apelido?: string): Observable<Jogador> {
    return this.http.post<Jogador>(`${this.baseUrl}/jogadores`, { nome, apelido });
  }

  atualizarJogador(id: number, nome: string, apelido: string | null, ativo: boolean): Observable<Jogador> {
    return this.http.put<Jogador>(`${this.baseUrl}/jogadores/${id}`, { nome, apelido, ativo });
  }

  removerJogador(id: number): Observable<void> {
    return this.http.delete<void>(`${this.baseUrl}/jogadores/${id}`);
  }

  // ---- Sessoes ----
  getSessoes(): Observable<SessaoJogo[]> {
    return this.http.get<SessaoJogo[]>(`${this.baseUrl}/sessoes`);
  }

  getSessao(id: number): Observable<SessaoJogo> {
    return this.http.get<SessaoJogo>(`${this.baseUrl}/sessoes/${id}`);
  }

  criarSessao(data: string, local: string | null, modoGoleiro: ModoGoleiro): Observable<SessaoJogo> {
    return this.http.post<SessaoJogo>(`${this.baseUrl}/sessoes`, { data, local, modoGoleiro });
  }

  getListaChegada(sessaoId: number): Observable<ListaChegada[]> {
    return this.http.get<ListaChegada[]>(`${this.baseUrl}/sessoes/${sessaoId}/chegadas`);
  }

  adicionarChegada(sessaoId: number, jogadorId: number): Observable<ListaChegada> {
    return this.http.post<ListaChegada>(`${this.baseUrl}/sessoes/${sessaoId}/chegadas`, { jogadorId });
  }

  removerChegada(sessaoId: number, jogadorId: number): Observable<void> {
    return this.http.delete<void>(`${this.baseUrl}/sessoes/${sessaoId}/chegadas/${jogadorId}`);
  }

  // ---- Partidas ----
  getPartida(id: number): Observable<Partida> {
    return this.http.get<Partida>(`${this.baseUrl}/partidas/${id}`);
  }

  getPartidasSessao(sessaoId: number): Observable<Partida[]> {
    return this.http.get<Partida[]>(`${this.baseUrl}/partidas/sessao/${sessaoId}`);
  }

  formarTimes(sessaoJogoId: number): Observable<Partida> {
    return this.http.post<Partida>(`${this.baseUrl}/partidas/formar-times`, { sessaoJogoId });
  }

  iniciarPartida(partidaId: number): Observable<Partida> {
    return this.http.post<Partida>(`${this.baseUrl}/partidas/${partidaId}/iniciar`, {});
  }

  finalizarPartida(partidaId: number): Observable<Partida> {
    return this.http.post<Partida>(`${this.baseUrl}/partidas/${partidaId}/finalizar`, {});
  }

  registrarGol(
    partidaId: number,
    jogadorId: number,
    timeId: number,
    minuto: number | null,
    golContra: boolean,
    assistenciaJogadorId: number | null
  ): Observable<Gol> {
    return this.http.post<Gol>(`${this.baseUrl}/partidas/${partidaId}/gols`, {
      jogadorId,
      timeId,
      minuto,
      golContra,
      assistenciaJogadorId,
    });
  }

  getHistorico(ano?: number, mes?: number, dataInicio?: string, dataFim?: string): Observable<Partida[]> {
    let params = new HttpParams();
    if (ano) params = params.set('ano', ano.toString());
    if (mes) params = params.set('mes', mes.toString());
    if (dataInicio) params = params.set('dataInicio', dataInicio);
    if (dataFim) params = params.set('dataFim', dataFim);
    return this.http.get<Partida[]>(`${this.baseUrl}/partidas/historico`, { params });
  }

  // ---- Ranking ----
  getRankingGeral(ano?: number, mes?: number): Observable<RankingJogador[]> {
    let params = new HttpParams();
    if (ano) params = params.set('ano', ano.toString());
    if (mes) params = params.set('mes', mes.toString());
    return this.http.get<RankingJogador[]>(`${this.baseUrl}/ranking/geral`, { params });
  }

  getArtilharia(ano?: number, mes?: number): Observable<RankingJogador[]> {
    let params = new HttpParams();
    if (ano) params = params.set('ano', ano.toString());
    if (mes) params = params.set('mes', mes.toString());
    return this.http.get<RankingJogador[]>(`${this.baseUrl}/ranking/artilharia`, { params });
  }

  getAssistencias(ano?: number, mes?: number): Observable<RankingJogador[]> {
    let params = new HttpParams();
    if (ano) params = params.set('ano', ano.toString());
    if (mes) params = params.set('mes', mes.toString());
    return this.http.get<RankingJogador[]>(`${this.baseUrl}/ranking/assistencias`, { params });
  }

  getParticipacaoGols(ano?: number, mes?: number): Observable<RankingJogador[]> {
    let params = new HttpParams();
    if (ano) params = params.set('ano', ano.toString());
    if (mes) params = params.set('mes', mes.toString());
    return this.http.get<RankingJogador[]>(`${this.baseUrl}/ranking/participacao-gols`, { params });
  }

  getRankingConfiguracoes(): Observable<RankingConfiguracao[]> {
    return this.http.get<RankingConfiguracao[]>(`${this.baseUrl}/ranking/configuracoes`);
  }

  atualizarRankingConfiguracao(id: number, valor: number): Observable<RankingConfiguracao> {
    return this.http.put<RankingConfiguracao>(`${this.baseUrl}/ranking/configuracoes/${id}`, { valor });
  }
}
