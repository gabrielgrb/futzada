import { Component } from '@angular/core';
import { IonApp, IonRouterOutlet, IonTabs, IonTabBar, IonTabButton, IonIcon, IonLabel } from '@ionic/angular/standalone';
import { addIcons } from 'ionicons';
import { peopleOutline, footballOutline, listOutline, trophyOutline, timeOutline } from 'ionicons/icons';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [IonApp, IonRouterOutlet, IonTabs, IonTabBar, IonTabButton, IonIcon, IonLabel],
  template: `
    <ion-app>
      <ion-tabs>
        <ion-router-outlet></ion-router-outlet>
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
    </ion-app>
  `,
})
export class AppComponent {
  constructor() {
    addIcons({ peopleOutline, footballOutline, listOutline, trophyOutline, timeOutline });
  }
}
