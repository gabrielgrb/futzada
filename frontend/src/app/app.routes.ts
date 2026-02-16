import { Routes } from '@angular/router';

export const routes: Routes = [
  {
    path: '',
    loadComponent: () => import('./tabs/tabs.page').then(m => m.TabsPage),
    children: [
      {
        path: '',
        redirectTo: 'sessao',
        pathMatch: 'full',
      },
      {
        path: 'sessao',
        loadComponent: () => import('./pages/sessao/sessao.page').then(m => m.SessaoPage),
      },
      {
        path: 'jogadores',
        loadComponent: () => import('./pages/jogadores/jogadores.page').then(m => m.JogadoresPage),
      },
      {
        path: 'ranking',
        loadComponent: () => import('./pages/ranking/ranking.page').then(m => m.RankingPage),
      },
      {
        path: 'historico',
        loadComponent: () => import('./pages/historico/historico.page').then(m => m.HistoricoPage),
      },
    ],
  },
  {
    path: 'partida/:id',
    loadComponent: () => import('./pages/partida/partida.page').then(m => m.PartidaPage),
  },
];
