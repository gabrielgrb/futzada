import { Component, OnInit, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import {
  IonHeader, IonToolbar, IonTitle, IonContent, IonButton, IonIcon,
  IonCard, IonCardHeader, IonCardTitle, IonCardContent, IonGrid,
  IonRow, IonCol, IonBadge, IonList, IonItem, IonLabel, IonSelect,
  IonSelectOption, IonToggle, IonBackButton, IonButtons, IonNote,
} from '@ionic/angular/standalone';
import { addIcons } from 'ionicons';
import {
  playOutline, stopOutline, footballOutline, arrowBackOutline,
} from 'ionicons/icons';
import { ApiService } from '../../services/api.service';
import { Partida, StatusPartida, Participacao } from '../../models/models';

@Component({
  selector: 'app-partida',
  standalone: true,
  imports: [
    CommonModule, FormsModule,
    IonHeader, IonToolbar, IonTitle, IonContent, IonButton, IonIcon,
    IonCard, IonCardHeader, IonCardTitle, IonCardContent, IonGrid,
    IonRow, IonCol, IonBadge, IonList, IonItem, IonLabel, IonSelect,
    IonSelectOption, IonToggle, IonBackButton, IonButtons, IonNote,
  ],
  template: `
    <ion-header>
      <ion-toolbar color="primary">
        <ion-buttons slot="start">
          <ion-back-button defaultHref="/sessao"></ion-back-button>
        </ion-buttons>
        <ion-title>Partida {{ partida?.numero }}</ion-title>
      </ion-toolbar>
    </ion-header>

    <ion-content class="ion-padding" *ngIf="partida">
      <!-- Timer -->
      <ion-card>
        <ion-card-content class="ion-text-center">
          <div style="font-size: 3rem; font-weight: bold; font-family: monospace;">
            {{ timerDisplay }}
          </div>
          <ion-badge [color]="statusColor()">{{ statusText() }}</ion-badge>
        </ion-card-content>
      </ion-card>

      <!-- Scoreboard -->
      <ion-card>
        <ion-card-content>
          <ion-grid>
            <ion-row class="ion-align-items-center ion-text-center">
              <ion-col>
                <strong>{{ partida.timeA?.nome }}</strong>
                <ion-badge color="primary" style="margin-left: 4px">{{ partida.timeA?.cor }}</ion-badge>
                <div class="score-highlight" style="color: var(--ion-color-primary);">
                  {{ partida.timeA?.golsMarcados || 0 }}
                </div>
              </ion-col>
              <ion-col size="2">
                <strong style="font-size: 1.5rem;">X</strong>
              </ion-col>
              <ion-col>
                <strong>{{ partida.timeB?.nome }}</strong>
                <ion-badge color="danger" style="margin-left: 4px">{{ partida.timeB?.cor }}</ion-badge>
                <div class="score-highlight" style="color: var(--ion-color-danger);">
                  {{ partida.timeB?.golsMarcados || 0 }}
                </div>
              </ion-col>
            </ion-row>
          </ion-grid>
        </ion-card-content>
      </ion-card>

      <!-- Team rosters -->
      <ion-grid>
        <ion-row>
          <ion-col>
            <ion-list>
              <ion-item *ngFor="let j of partida.timeA?.jogadores">
                <ion-label>{{ j.nomeJogador }}</ion-label>
                <ion-badge *ngIf="j.ehGoleiro" color="tertiary" slot="end">GK</ion-badge>
              </ion-item>
            </ion-list>
          </ion-col>
          <ion-col>
            <ion-list>
              <ion-item *ngFor="let j of partida.timeB?.jogadores">
                <ion-label>{{ j.nomeJogador }}</ion-label>
                <ion-badge *ngIf="j.ehGoleiro" color="tertiary" slot="end">GK</ion-badge>
              </ion-item>
            </ion-list>
          </ion-col>
        </ion-row>
      </ion-grid>

      <!-- Controls -->
      <ion-button
        *ngIf="partida.status === 0"
        expand="block" color="success" (click)="iniciar()">
        <ion-icon name="play-outline" slot="start"></ion-icon>
        Iniciar Partida
      </ion-button>

      <!-- Register goal (only during match) -->
      <ion-card *ngIf="partida.status === 1">
        <ion-card-header>
          <ion-card-title>Registrar Gol</ion-card-title>
        </ion-card-header>
        <ion-card-content>
          <ion-item>
            <ion-select label="Time" labelPlacement="stacked" [(ngModel)]="golTimeId">
              <ion-select-option *ngIf="partida.timeA" [value]="partida.timeA.id">
                {{ partida.timeA.nome }} ({{ partida.timeA.cor }})
              </ion-select-option>
              <ion-select-option *ngIf="partida.timeB" [value]="partida.timeB.id">
                {{ partida.timeB.nome }} ({{ partida.timeB.cor }})
              </ion-select-option>
            </ion-select>
          </ion-item>
          <ion-item>
            <ion-select label="Jogador (gol)" labelPlacement="stacked" [(ngModel)]="golJogadorId">
              <ion-select-option *ngFor="let j of jogadoresDoTime(golTimeId)" [value]="j.jogadorId">
                {{ j.nomeJogador }}
              </ion-select-option>
            </ion-select>
          </ion-item>
          <ion-item>
            <ion-select label="Assistencia (opcional)" labelPlacement="stacked" [(ngModel)]="assistenciaJogadorId">
              <ion-select-option [value]="null">Nenhuma</ion-select-option>
              <ion-select-option
                *ngFor="let j of jogadoresDoTime(golTimeId)"
                [value]="j.jogadorId"
                [disabled]="j.jogadorId === golJogadorId">
                {{ j.nomeJogador }}
              </ion-select-option>
            </ion-select>
          </ion-item>
          <ion-item>
            <ion-toggle [(ngModel)]="golContra">Gol contra</ion-toggle>
          </ion-item>
          <ion-button expand="block" (click)="registrarGol()" [disabled]="!golTimeId || !golJogadorId" class="ion-margin-top">
            <ion-icon name="football-outline" slot="start"></ion-icon>
            Registrar Gol
          </ion-button>
        </ion-card-content>
      </ion-card>

      <!-- Goal list -->
      <ion-card *ngIf="partida.gols.length > 0">
        <ion-card-header>
          <ion-card-title>Gols</ion-card-title>
        </ion-card-header>
        <ion-card-content>
          <ion-list>
            <ion-item *ngFor="let g of partida.gols">
              <ion-icon name="football-outline" slot="start"></ion-icon>
              <ion-label>
                <h3>{{ g.nomeJogador }} ({{ g.nomeTime }}) {{ g.golContra ? '(GC)' : '' }}</h3>
                <p *ngIf="g.assistencia">Assist: {{ g.assistencia.nomeJogador }}</p>
              </ion-label>
            </ion-item>
          </ion-list>
        </ion-card-content>
      </ion-card>

      <ion-button
        *ngIf="partida.status === 1"
        expand="block" color="danger" (click)="finalizar()">
        <ion-icon name="stop-outline" slot="start"></ion-icon>
        Finalizar Partida
      </ion-button>

      <!-- Result -->
      <ion-card *ngIf="partida.status === 2">
        <ion-card-header>
          <ion-card-title>Resultado</ion-card-title>
        </ion-card-header>
        <ion-card-content class="ion-text-center">
          <div *ngIf="partida.empate" style="font-size: 1.5rem;">Empate!</div>
          <div *ngIf="!partida.empate && partida.timeA?.vencedor" style="font-size: 1.5rem; color: var(--ion-color-success);">
            {{ partida.timeA?.nome }} venceu!
          </div>
          <div *ngIf="!partida.empate && partida.timeB?.vencedor" style="font-size: 1.5rem; color: var(--ion-color-success);">
            {{ partida.timeB?.nome }} venceu!
          </div>
          <ion-button expand="block" fill="outline" routerLink="/sessao" class="ion-margin-top">
            Voltar para Sessao
          </ion-button>
        </ion-card-content>
      </ion-card>
    </ion-content>
  `,
})
export class PartidaPage implements OnInit, OnDestroy {
  partida: Partida | null = null;
  timerSeconds = 0;
  timerInterval: any = null;
  timerDisplay = '00:00';

  golTimeId: number | null = null;
  golJogadorId: number | null = null;
  assistenciaJogadorId: number | null = null;
  golContra = false;

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private api: ApiService,
  ) {
    addIcons({ playOutline, stopOutline, footballOutline, arrowBackOutline });
  }

  ngOnInit() {
    const id = Number(this.route.snapshot.paramMap.get('id'));
    this.carregar(id);
  }

  ngOnDestroy() {
    this.pararTimer();
  }

  carregar(id: number) {
    this.api.getPartida(id).subscribe((p) => {
      this.partida = p;
      if (p.status === StatusPartida.EmAndamento && p.iniciadaEm) {
        const elapsed = Math.floor((Date.now() - new Date(p.iniciadaEm).getTime()) / 1000);
        this.timerSeconds = elapsed;
        this.iniciarTimer();
      }
    });
  }

  iniciar() {
    if (!this.partida) return;
    this.api.iniciarPartida(this.partida.id).subscribe((p) => {
      this.partida = p;
      this.timerSeconds = 0;
      this.iniciarTimer();
    });
  }

  finalizar() {
    if (!this.partida) return;
    this.pararTimer();
    this.api.finalizarPartida(this.partida.id).subscribe((p) => {
      this.partida = p;
    });
  }

  registrarGol() {
    if (!this.partida || !this.golTimeId || !this.golJogadorId) return;
    const minuto = Math.floor(this.timerSeconds / 60);
    this.api.registrarGol(
      this.partida.id,
      this.golJogadorId,
      this.golTimeId,
      minuto,
      this.golContra,
      this.assistenciaJogadorId,
    ).subscribe((gol) => {
      if (gol && this.partida) {
        this.partida.gols.push(gol);
        // Update score locally
        if (this.partida.timeA && gol.timeId === this.partida.timeA.id && !gol.golContra) {
          this.partida.timeA.golsMarcados++;
        } else if (this.partida.timeB && gol.timeId === this.partida.timeB.id && !gol.golContra) {
          this.partida.timeB.golsMarcados++;
        }
        // Reset form
        this.golJogadorId = null;
        this.assistenciaJogadorId = null;
        this.golContra = false;

        // Auto-finish if 2 goals reached
        const maxGols = Math.max(
          this.partida.timeA?.golsMarcados || 0,
          this.partida.timeB?.golsMarcados || 0
        );
        if (maxGols >= 2) {
          this.finalizar();
        }
      }
    });
  }

  jogadoresDoTime(timeId: number | null): Participacao[] {
    if (!this.partida || !timeId) return [];
    if (this.partida.timeA?.id === timeId) return this.partida.timeA.jogadores;
    if (this.partida.timeB?.id === timeId) return this.partida.timeB.jogadores;
    return [];
  }

  private iniciarTimer() {
    this.pararTimer();
    this.atualizarDisplay();
    this.timerInterval = setInterval(() => {
      this.timerSeconds++;
      this.atualizarDisplay();
      // Auto-finish at 7 minutes (420 seconds)
      if (this.timerSeconds >= 420) {
        this.finalizar();
      }
    }, 1000);
  }

  private pararTimer() {
    if (this.timerInterval) {
      clearInterval(this.timerInterval);
      this.timerInterval = null;
    }
  }

  private atualizarDisplay() {
    const min = Math.floor(this.timerSeconds / 60);
    const sec = this.timerSeconds % 60;
    this.timerDisplay = `${min.toString().padStart(2, '0')}:${sec.toString().padStart(2, '0')}`;
  }

  statusColor(): string {
    if (!this.partida) return 'medium';
    switch (this.partida.status) {
      case StatusPartida.Aguardando: return 'warning';
      case StatusPartida.EmAndamento: return 'success';
      case StatusPartida.Finalizada: return 'medium';
    }
  }

  statusText(): string {
    if (!this.partida) return '';
    switch (this.partida.status) {
      case StatusPartida.Aguardando: return 'Aguardando';
      case StatusPartida.EmAndamento: return 'Em Andamento';
      case StatusPartida.Finalizada: return 'Finalizada';
    }
  }
}
