import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import {
  IonHeader, IonToolbar, IonTitle, IonContent, IonList, IonItem,
  IonLabel, IonButton, IonIcon, IonFab, IonFabButton, IonInput,
  IonAlert, IonItemSliding, IonItemOptions, IonItemOption,
  IonBadge, IonSearchbar, IonRefresher, IonRefresherContent,
} from '@ionic/angular/standalone';
import { addIcons } from 'ionicons';
import { addOutline, createOutline, trashOutline, personOutline } from 'ionicons/icons';
import { ApiService } from '../../services/api.service';
import { Jogador } from '../../models/models';

@Component({
  selector: 'app-jogadores',
  standalone: true,
  imports: [
    CommonModule, FormsModule,
    IonHeader, IonToolbar, IonTitle, IonContent, IonList, IonItem,
    IonLabel, IonButton, IonIcon, IonFab, IonFabButton, IonInput,
    IonAlert, IonItemSliding, IonItemOptions, IonItemOption,
    IonBadge, IonSearchbar, IonRefresher, IonRefresherContent,
  ],
  template: `
    <ion-header>
      <ion-toolbar color="primary">
        <ion-title>Jogadores</ion-title>
      </ion-toolbar>
      <ion-toolbar>
        <ion-searchbar
          placeholder="Buscar jogador..."
          [(ngModel)]="filtro"
          (ionInput)="filtrar()">
        </ion-searchbar>
      </ion-toolbar>
    </ion-header>

    <ion-content>
      <ion-refresher slot="fixed" (ionRefresh)="carregar($event)">
        <ion-refresher-content></ion-refresher-content>
      </ion-refresher>

      <!-- Form to add new player -->
      <div *ngIf="mostrarForm" style="padding: 16px;">
        <ion-item>
          <ion-input label="Nome" labelPlacement="stacked" [(ngModel)]="novoNome" placeholder="Nome completo"></ion-input>
        </ion-item>
        <ion-item>
          <ion-input label="Apelido" labelPlacement="stacked" [(ngModel)]="novoApelido" placeholder="Apelido (opcional)"></ion-input>
        </ion-item>
        <ion-button expand="block" (click)="salvar()" [disabled]="!novoNome">
          {{ editandoId ? 'Atualizar' : 'Cadastrar' }}
        </ion-button>
        <ion-button expand="block" fill="outline" (click)="cancelar()">Cancelar</ion-button>
      </div>

      <ion-list>
        <ion-item-sliding *ngFor="let j of jogadoresFiltrados">
          <ion-item>
            <ion-icon name="person-outline" slot="start"></ion-icon>
            <ion-label>
              <h2>{{ j.apelido || j.nome }}</h2>
              <p *ngIf="j.apelido">{{ j.nome }}</p>
            </ion-label>
            <ion-badge [color]="j.ativo ? 'success' : 'medium'" slot="end">
              {{ j.ativo ? 'Ativo' : 'Inativo' }}
            </ion-badge>
          </ion-item>
          <ion-item-options side="end">
            <ion-item-option color="primary" (click)="editar(j)">
              <ion-icon name="create-outline" slot="icon-only"></ion-icon>
            </ion-item-option>
            <ion-item-option color="danger" (click)="remover(j)">
              <ion-icon name="trash-outline" slot="icon-only"></ion-icon>
            </ion-item-option>
          </ion-item-options>
        </ion-item-sliding>
      </ion-list>

      <ion-fab vertical="bottom" horizontal="end" slot="fixed">
        <ion-fab-button (click)="mostrarForm = true; editandoId = null; novoNome = ''; novoApelido = ''">
          <ion-icon name="add-outline"></ion-icon>
        </ion-fab-button>
      </ion-fab>
    </ion-content>
  `,
})
export class JogadoresPage implements OnInit {
  jogadores: Jogador[] = [];
  jogadoresFiltrados: Jogador[] = [];
  filtro = '';
  mostrarForm = false;
  novoNome = '';
  novoApelido = '';
  editandoId: number | null = null;

  constructor(private api: ApiService) {
    addIcons({ addOutline, createOutline, trashOutline, personOutline });
  }

  ngOnInit() {
    this.carregar();
  }

  carregar(event?: any) {
    this.api.getJogadores().subscribe((data) => {
      this.jogadores = data;
      this.filtrar();
      if (event) event.target.complete();
    });
  }

  filtrar() {
    const f = this.filtro.toLowerCase();
    this.jogadoresFiltrados = this.jogadores.filter(
      (j) => j.nome.toLowerCase().includes(f) || (j.apelido?.toLowerCase().includes(f) ?? false)
    );
  }

  salvar() {
    if (this.editandoId) {
      const j = this.jogadores.find((x) => x.id === this.editandoId)!;
      this.api.atualizarJogador(this.editandoId, this.novoNome, this.novoApelido || null, j.ativo).subscribe(() => {
        this.cancelar();
        this.carregar();
      });
    } else {
      this.api.criarJogador(this.novoNome, this.novoApelido || undefined).subscribe(() => {
        this.cancelar();
        this.carregar();
      });
    }
  }

  editar(j: Jogador) {
    this.editandoId = j.id;
    this.novoNome = j.nome;
    this.novoApelido = j.apelido || '';
    this.mostrarForm = true;
  }

  remover(j: Jogador) {
    if (confirm(`Remover ${j.apelido || j.nome}?`)) {
      this.api.removerJogador(j.id).subscribe(() => this.carregar());
    }
  }

  cancelar() {
    this.mostrarForm = false;
    this.novoNome = '';
    this.novoApelido = '';
    this.editandoId = null;
  }
}
