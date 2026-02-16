import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import {
  IonHeader, IonToolbar, IonTitle, IonContent, IonSegment,
  IonSegmentButton, IonLabel, IonList, IonItem, IonBadge,
  IonGrid, IonRow, IonCol, IonSelect, IonSelectOption,
  IonCard, IonCardContent, IonRefresher, IonRefresherContent,
  IonNote,
} from '@ionic/angular/standalone';
import { ApiService } from '../../services/api.service';
import { RankingJogador } from '../../models/models';

@Component({
  selector: 'app-ranking',
  standalone: true,
  imports: [
    CommonModule, FormsModule,
    IonHeader, IonToolbar, IonTitle, IonContent, IonSegment,
    IonSegmentButton, IonLabel, IonList, IonItem, IonBadge,
    IonGrid, IonRow, IonCol, IonSelect, IonSelectOption,
    IonCard, IonCardContent, IonRefresher, IonRefresherContent,
    IonNote,
  ],
  template: `
    <ion-header>
      <ion-toolbar color="primary">
        <ion-title>Ranking</ion-title>
      </ion-toolbar>
      <ion-toolbar>
        <ion-segment [(ngModel)]="tipoRanking" (ionChange)="carregar()">
          <ion-segment-button value="geral"><ion-label>Geral</ion-label></ion-segment-button>
          <ion-segment-button value="artilharia"><ion-label>Gols</ion-label></ion-segment-button>
          <ion-segment-button value="assistencias"><ion-label>Assist.</ion-label></ion-segment-button>
          <ion-segment-button value="participacao"><ion-label>G+A</ion-label></ion-segment-button>
        </ion-segment>
      </ion-toolbar>
    </ion-header>

    <ion-content>
      <ion-refresher slot="fixed" (ionRefresh)="refresh($event)">
        <ion-refresher-content></ion-refresher-content>
      </ion-refresher>

      <!-- Filters -->
      <ion-card>
        <ion-card-content>
          <ion-grid>
            <ion-row>
              <ion-col>
                <ion-select label="Ano" labelPlacement="stacked" [(ngModel)]="filtroAno" (ionChange)="carregar()">
                  <ion-select-option [value]="null">Todos</ion-select-option>
                  <ion-select-option *ngFor="let a of anos" [value]="a">{{ a }}</ion-select-option>
                </ion-select>
              </ion-col>
              <ion-col>
                <ion-select label="Mes" labelPlacement="stacked" [(ngModel)]="filtroMes" (ionChange)="carregar()">
                  <ion-select-option [value]="null">Todos</ion-select-option>
                  <ion-select-option *ngFor="let m of meses; let i = index" [value]="i + 1">{{ m }}</ion-select-option>
                </ion-select>
              </ion-col>
            </ion-row>
          </ion-grid>
        </ion-card-content>
      </ion-card>

      <!-- Ranking table -->
      <ion-list>
        <ion-item *ngFor="let r of ranking" [color]="r.posicao <= 3 ? '' : ''">
          <span slot="start" class="ranking-badge"
            [ngClass]="{
              'gold': r.posicao === 1,
              'silver': r.posicao === 2,
              'bronze': r.posicao === 3,
              'default': r.posicao > 3
            }">
            {{ r.posicao }}
          </span>
          <ion-label>
            <h2><strong>{{ r.apelido || r.nome }}</strong></h2>
            <p>
              {{ r.partidas }}J &middot;
              {{ r.vitorias }}V {{ r.empates }}E {{ r.derrotas }}D &middot;
              {{ r.gols }}G {{ r.assistencias }}A
              <span *ngIf="r.golsContra > 0"> &middot; {{ r.golsContra }}GC</span>
            </p>
          </ion-label>
          <ion-note slot="end" style="font-size: 1.2rem; font-weight: bold;">
            <span *ngIf="tipoRanking === 'geral'">{{ r.pontuacaoTotal }} pts</span>
            <span *ngIf="tipoRanking === 'artilharia'">{{ r.gols }} gols</span>
            <span *ngIf="tipoRanking === 'assistencias'">{{ r.assistencias }} ast</span>
            <span *ngIf="tipoRanking === 'participacao'">{{ r.participacaoGols }} G+A</span>
          </ion-note>
        </ion-item>
      </ion-list>

      <div *ngIf="ranking.length === 0" class="ion-text-center ion-padding">
        <p>Nenhum dado encontrado para o filtro selecionado.</p>
      </div>
    </ion-content>
  `,
})
export class RankingPage implements OnInit {
  ranking: RankingJogador[] = [];
  tipoRanking = 'geral';
  filtroAno: number | null = null;
  filtroMes: number | null = null;

  anos: number[] = [];
  meses = ['Jan', 'Fev', 'Mar', 'Abr', 'Mai', 'Jun', 'Jul', 'Ago', 'Set', 'Out', 'Nov', 'Dez'];

  constructor(private api: ApiService) {
    const anoAtual = new Date().getFullYear();
    for (let a = anoAtual; a >= anoAtual - 5; a--) {
      this.anos.push(a);
    }
  }

  ngOnInit() {
    this.carregar();
  }

  refresh(event: any) {
    this.carregar();
    setTimeout(() => event.target.complete(), 500);
  }

  carregar() {
    const ano = this.filtroAno ?? undefined;
    const mes = this.filtroMes ?? undefined;

    switch (this.tipoRanking) {
      case 'geral':
        this.api.getRankingGeral(ano, mes).subscribe((r) => (this.ranking = r));
        break;
      case 'artilharia':
        this.api.getArtilharia(ano, mes).subscribe((r) => (this.ranking = r));
        break;
      case 'assistencias':
        this.api.getAssistencias(ano, mes).subscribe((r) => (this.ranking = r));
        break;
      case 'participacao':
        this.api.getParticipacaoGols(ano, mes).subscribe((r) => (this.ranking = r));
        break;
    }
  }
}
