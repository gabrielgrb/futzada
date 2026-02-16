import { Component } from '@angular/core';
import { IonTabs, IonTabBar, IonTabButton, IonIcon, IonLabel } from '@ionic/angular/standalone';
import { addIcons } from 'ionicons';
import { peopleOutline, footballOutline, trophyOutline, timeOutline } from 'ionicons/icons';

@Component({
  selector: 'app-tabs',
  standalone: true,
  imports: [IonTabs, IonTabBar, IonTabButton, IonIcon, IonLabel],
  template: `
    <ion-tabs>
      <ion-tab-bar slot="bottom" color="primary">
        <ion-tab-button tab="sessao">
          <ion-icon name="football-outline"></ion-icon>
          <ion-label>Jogar</ion-label>
        </ion-tab-button>
        <ion-tab-button tab="jogadores">
          <ion-icon name="people-outline"></ion-icon>
          <ion-label>Jogadores</ion-label>
        </ion-tab-button>
        <ion-tab-button tab="ranking">
          <ion-icon name="trophy-outline"></ion-icon>
          <ion-label>Ranking</ion-label>
        </ion-tab-button>
        <ion-tab-button tab="historico">
          <ion-icon name="time-outline"></ion-icon>
          <ion-label>Historico</ion-label>
        </ion-tab-button>
      </ion-tab-bar>
    </ion-tabs>
  `,
})
export class TabsPage {
  constructor() {
    addIcons({ peopleOutline, footballOutline, trophyOutline, timeOutline });
  }
}
