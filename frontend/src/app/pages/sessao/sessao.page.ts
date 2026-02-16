import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import {
  IonHeader, IonToolbar, IonTitle, IonContent, IonList, IonItem,
  IonLabel, IonButton, IonIcon, IonCard, IonCardHeader, IonCardTitle,
  IonCardContent, IonSelect, IonSelectOption, IonInput, IonChip,
  IonBadge, IonGrid, IonRow, IonCol, IonNote, IonRefresher,
  IonRefresherContent, IonItemDivider,
} from '@ionic/angular/standalone';
import { addIcons } from 'ionicons';
import {
  addOutline, footballOutline, playOutline, personAddOutline,
  closeCircleOutline, shuffleOutline,
} from 'ionicons/icons';
import { ApiService } from '../../services/api.service';
import { Jogador, SessaoJogo, ListaChegada, Partida, ModoGoleiro, StatusPartida } from '../../models/models';

@Component({
  selector: 'app-sessao',
  standalone: true,
  imports: [
    CommonModule, FormsModule,
    IonHeader, IonToolbar, IonTitle, IonContent, IonList, IonItem,
    IonLabel, IonButton, IonIcon, IonCard, IonCardHeader, IonCardTitle,
    IonCardContent, IonSelect, IonSelectOption, IonInput, IonChip,
    IonBadge, IonGrid, IonRow, IonCol, IonNote, IonRefresher,
    IonRefresherContent, IonItemDivider,
  ],
  template: `
    <ion-header>
      <ion-toolbar color="primary">
        <ion-title>Fut de Quarta</ion-title>
      </ion-toolbar>
    </ion-header>

    <ion-content class="ion-padding">
      <ion-refresher slot="fixed" (ionRefresh)="refresh($event)">
        <ion-refresher-content></ion-refresher-content>
      </ion-refresher>

      <!-- No active session: Create one -->
      <ion-card *ngIf="!sessaoAtiva">
        <ion-card-header>
          <ion-card-title>Nova Sessao</ion-card-title>
        </ion-card-header>
        <ion-card-content>
          <ion-item>
            <ion-input label="Local" labelPlacement="stacked" [(ngModel)]="novoLocal" placeholder="Ex: Quadra do clube"></ion-input>
          </ion-item>
          <ion-item>
            <ion-select label="Modo Goleiro" labelPlacement="stacked" [(ngModel)]="modoGoleiro">
              <ion-select-option [value]="0">Fixo</ion-select-option>
              <ion-select-option [value]="1">Rotativo</ion-select-option>
            </ion-select>
          </ion-item>
          <ion-button expand="block" (click)="criarSessao()" class="ion-margin-top">
            <ion-icon name="football-outline" slot="start"></ion-icon>
            Iniciar Sessao
          </ion-button>
        </ion-card-content>
      </ion-card>

      <!-- Active session -->
      <div *ngIf="sessaoAtiva">
        <ion-card>
          <ion-card-header>
            <ion-card-title>
              Sessao {{ sessaoAtiva.data | date:'dd/MM/yyyy' }}
              <ion-badge color="secondary" style="margin-left: 8px">
                {{ sessaoAtiva.modoGoleiro === 1 ? 'Goleiro Rotativo' : 'Goleiro Fixo' }}
              </ion-badge>
            </ion-card-title>
          </ion-card-header>
          <ion-card-content>
            <p *ngIf="sessaoAtiva.local">{{ sessaoAtiva.local }}</p>
          </ion-card-content>
        </ion-card>

        <!-- Arrival list -->
        <ion-card>
          <ion-card-header>
            <ion-card-title>Lista de Chegada ({{ listaChegada.length }})</ion-card-title>
          </ion-card-header>
          <ion-card-content>
            <ion-item>
              <ion-select label="Jogador" labelPlacement="stacked" [(ngModel)]="jogadorSelecionado" placeholder="Selecione...">
                <ion-select-option *ngFor="let j of jogadoresDisponiveis" [value]="j.id">
                  {{ j.apelido || j.nome }}
                </ion-select-option>
              </ion-select>
              <ion-button slot="end" fill="clear" (click)="adicionarChegada()" [disabled]="!jogadorSelecionado">
                <ion-icon name="person-add-outline"></ion-icon>
              </ion-button>
            </ion-item>

            <ion-list>
              <ion-item *ngFor="let c of listaChegada; let i = index">
                <ion-badge slot="start" color="medium">{{ c.ordemChegada }}</ion-badge>
                <ion-label>{{ c.apelido || c.nomeJogador }}</ion-label>
                <ion-button slot="end" fill="clear" color="danger" (click)="removerChegada(c.jogadorId)">
                  <ion-icon name="close-circle-outline"></ion-icon>
                </ion-button>
              </ion-item>
            </ion-list>

            <ion-button
              expand="block"
              color="secondary"
              (click)="formarTimes()"
              [disabled]="listaChegada.length < 10"
              class="ion-margin-top">
              <ion-icon name="shuffle-outline" slot="start"></ion-icon>
              Formar Times ({{ listaChegada.length }}/10)
            </ion-button>
          </ion-card-content>
        </ion-card>

        <!-- Current matches -->
        <ion-card *ngFor="let p of partidas">
          <ion-card-header>
            <ion-card-title>
              Partida {{ p.numero }}
              <ion-badge [color]="statusColor(p.status)">{{ statusText(p.status) }}</ion-badge>
            </ion-card-title>
          </ion-card-header>
          <ion-card-content>
            <ion-grid>
              <ion-row class="ion-align-items-center ion-text-center">
                <ion-col>
                  <strong>{{ p.timeA?.nome }}</strong>
                  <ion-badge color="primary" style="margin-left: 4px">{{ p.timeA?.cor }}</ion-badge>
                  <div class="score-highlight">{{ p.timeA?.golsMarcados || 0 }}</div>
                  <div *ngFor="let j of p.timeA?.jogadores">
                    <small>{{ j.nomeJogador }} {{ j.ehGoleiro ? '(GK)' : '' }}</small>
                  </div>
                </ion-col>
                <ion-col size="2">
                  <strong>X</strong>
                </ion-col>
                <ion-col>
                  <strong>{{ p.timeB?.nome }}</strong>
                  <ion-badge color="danger" style="margin-left: 4px">{{ p.timeB?.cor }}</ion-badge>
                  <div class="score-highlight">{{ p.timeB?.golsMarcados || 0 }}</div>
                  <div *ngFor="let j of p.timeB?.jogadores">
                    <small>{{ j.nomeJogador }} {{ j.ehGoleiro ? '(GK)' : '' }}</small>
                  </div>
                </ion-col>
              </ion-row>
            </ion-grid>

            <ion-button
              *ngIf="p.status === 0"
              expand="block"
              color="success"
              (click)="abrirPartida(p.id)">
              <ion-icon name="play-outline" slot="start"></ion-icon>
              Abrir Partida
            </ion-button>
            <ion-button
              *ngIf="p.status === 1"
              expand="block"
              (click)="abrirPartida(p.id)">
              <ion-icon name="football-outline" slot="start"></ion-icon>
              Ir para Partida
            </ion-button>
          </ion-card-content>
        </ion-card>
      </div>
    </ion-content>
  `,
})
export class SessaoPage implements OnInit {
  sessaoAtiva: SessaoJogo | null = null;
  listaChegada: ListaChegada[] = [];
  partidas: Partida[] = [];
  jogadores: Jogador[] = [];
  jogadoresDisponiveis: Jogador[] = [];
  jogadorSelecionado: number | null = null;
  novoLocal = '';
  modoGoleiro = ModoGoleiro.Rotativo;

  constructor(private api: ApiService, private router: Router) {
    addIcons({ addOutline, footballOutline, playOutline, personAddOutline, closeCircleOutline, shuffleOutline });
  }

  ngOnInit() {
    this.carregar();
  }

  refresh(event: any) {
    this.carregar();
    setTimeout(() => event.target.complete(), 500);
  }

  carregar() {
    this.api.getJogadores(true).subscribe((j) => (this.jogadores = j));
    this.api.getSessoes().subscribe((sessoes) => {
      // Get most recent session if it's today
      if (sessoes.length > 0) {
        const hoje = new Date().toISOString().split('T')[0];
        const recente = sessoes[0];
        const dataSessao = new Date(recente.data).toISOString().split('T')[0];
        if (dataSessao === hoje) {
          this.sessaoAtiva = recente;
          this.listaChegada = recente.listaChegada;
          this.atualizarDisponiveis();
          this.carregarPartidas();
          return;
        }
      }
      this.sessaoAtiva = null;
    });
  }

  carregarPartidas() {
    if (!this.sessaoAtiva) return;
    this.api.getPartidasSessao(this.sessaoAtiva.id).subscribe((p) => (this.partidas = p));
  }

  atualizarDisponiveis() {
    const idsNaLista = new Set(this.listaChegada.map((c) => c.jogadorId));
    this.jogadoresDisponiveis = this.jogadores.filter((j) => !idsNaLista.has(j.id));
  }

  criarSessao() {
    this.api.criarSessao(new Date().toISOString(), this.novoLocal || null, this.modoGoleiro).subscribe((s) => {
      this.sessaoAtiva = s;
      this.listaChegada = [];
      this.partidas = [];
      this.atualizarDisponiveis();
    });
  }

  adicionarChegada() {
    if (!this.sessaoAtiva || !this.jogadorSelecionado) return;
    this.api.adicionarChegada(this.sessaoAtiva.id, this.jogadorSelecionado).subscribe((c) => {
      if (c) {
        this.listaChegada.push(c);
        this.jogadorSelecionado = null;
        this.atualizarDisponiveis();
      }
    });
  }

  removerChegada(jogadorId: number) {
    if (!this.sessaoAtiva) return;
    this.api.removerChegada(this.sessaoAtiva.id, jogadorId).subscribe(() => {
      this.listaChegada = this.listaChegada.filter((c) => c.jogadorId !== jogadorId);
      this.atualizarDisponiveis();
    });
  }

  formarTimes() {
    if (!this.sessaoAtiva) return;
    this.api.formarTimes(this.sessaoAtiva.id).subscribe((partida) => {
      this.partidas.push(partida);
    });
  }

  abrirPartida(partidaId: number) {
    this.router.navigate(['/partida', partidaId]);
  }

  statusColor(status: StatusPartida): string {
    switch (status) {
      case StatusPartida.Aguardando: return 'warning';
      case StatusPartida.EmAndamento: return 'success';
      case StatusPartida.Finalizada: return 'medium';
    }
  }

  statusText(status: StatusPartida): string {
    switch (status) {
      case StatusPartida.Aguardando: return 'Aguardando';
      case StatusPartida.EmAndamento: return 'Em Andamento';
      case StatusPartida.Finalizada: return 'Finalizada';
    }
  }
}
