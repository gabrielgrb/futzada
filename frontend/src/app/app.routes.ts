import { Routes } from '@angular/router';

export const routes: Routes = [
  {
    path: '',
    redirectTo: 'sessao',
    pathMatch: 'full',
  },
  {
    path: 'jogadores',
    loadComponent: () => import('./pages/jogadores/jogadores.page').then(m => m.JogadoresPage),
  },
  {
    path: 'sessao',
    loadComponent: () => import('./pages/sessao/sessao.page').then(m => m.SessaoPage),
  },
  {
    path: 'partida/:id',
    loadComponent: () => import('./pages/partida/partida.page').then(m => m.PartidaPage),
  },
  {
    path: 'ranking',
    loadComponent: () => import('./pages/ranking/ranking.page').then(m => m.RankingPage),
  },
  {
    path: 'historico',
    loadComponent: () => import('./pages/historico/historico.page').then(m => m.HistoricoPage),
  },
];
