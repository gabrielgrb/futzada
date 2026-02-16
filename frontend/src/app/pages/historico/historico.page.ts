import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import {
  IonHeader, IonToolbar, IonTitle, IonContent, IonList, IonItem,
  IonLabel, IonBadge, IonCard, IonCardHeader, IonCardTitle,
  IonCardContent, IonGrid, IonRow, IonCol, IonSelect,
  IonSelectOption, IonRefresher, IonRefresherContent, IonNote,
  IonIcon,
} from '@ionic/angular/standalone';
import { addIcons } from 'ionicons';
import { footballOutline } from 'ionicons/icons';
import { ApiService } from '../../services/api.service';
import { Partida, StatusPartida } from '../../models/models';

@Component({
  selector: 'app-historico',
  standalone: true,
  imports: [
    CommonModule, FormsModule,
    IonHeader, IonToolbar, IonTitle, IonContent, IonList, IonItem,
    IonLabel, IonBadge, IonCard, IonCardHeader, IonCardTitle,
    IonCardContent, IonGrid, IonRow, IonCol, IonSelect,
    IonSelectOption, IonRefresher, IonRefresherContent, IonNote,
    IonIcon,
  ],
  template: `
    <ion-header>
      <ion-toolbar color="primary">
        <ion-title>Historico</ion-title>
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

      <!-- Match list -->
      <ion-card *ngFor="let p of partidas">
        <ion-card-content>
          <ion-grid>
            <ion-row class="ion-align-items-center ion-text-center">
              <ion-col>
                <strong>{{ p.timeA?.nome }}</strong>
                <div class="score-highlight" style="font-size: 1.5rem;">{{ p.timeA?.golsMarcados || 0 }}</div>
                <ion-badge *ngIf="p.timeA?.vencedor" color="success">V</ion-badge>
              </ion-col>
              <ion-col size="2">
                <strong>X</strong>
              </ion-col>
              <ion-col>
                <strong>{{ p.timeB?.nome }}</strong>
                <div class="score-highlight" style="font-size: 1.5rem;">{{ p.timeB?.golsMarcados || 0 }}</div>
                <ion-badge *ngIf="p.timeB?.vencedor" color="success">V</ion-badge>
              </ion-col>
            </ion-row>
          </ion-grid>
          <div class="ion-text-center" *ngIf="p.empate">
            <ion-badge color="warning">Empate</ion-badge>
          </div>
          <!-- Goals summary -->
          <ion-list *ngIf="p.gols.length > 0" lines="none">
            <ion-item *ngFor="let g of p.gols" class="ion-no-padding">
              <ion-icon name="football-outline" slot="start" style="font-size: 14px;"></ion-icon>
              <ion-label>
                <p style="margin: 2px 0;">
                  {{ g.nomeJogador }} {{ g.golContra ? '(GC)' : '' }}
                  <span *ngIf="g.assistencia"> - Assist: {{ g.assistencia.nomeJogador }}</span>
                </p>
              </ion-label>
            </ion-item>
          </ion-list>
        </ion-card-content>
      </ion-card>

      <div *ngIf="partidas.length === 0" class="ion-text-center ion-padding">
        <p>Nenhuma partida encontrada.</p>
      </div>
    </ion-content>
  `,
})
export class HistoricoPage implements OnInit {
  partidas: Partida[] = [];
  filtroAno: number | null = null;
  filtroMes: number | null = null;

  anos: number[] = [];
  meses = ['Jan', 'Fev', 'Mar', 'Abr', 'Mai', 'Jun', 'Jul', 'Ago', 'Set', 'Out', 'Nov', 'Dez'];

  constructor(private api: ApiService) {
    addIcons({ footballOutline });
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
    this.api.getHistorico(
      this.filtroAno ?? undefined,
      this.filtroMes ?? undefined,
    ).subscribe((p) => (this.partidas = p));
  }
}
