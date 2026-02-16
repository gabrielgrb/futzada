# Fut de Quarta

Sistema web/mobile para organizar futebol semanal com lista de chegada, formacao automatica de times, cronometro de partidas, estatisticas individuais e ranking.

## Stack

| Camada   | Tecnologia              |
|----------|-------------------------|
| Frontend | Angular 17 + Ionic 7    |
| Backend  | .NET 8 Web API          |
| ORM      | Entity Framework Core 8 |
| Banco    | SQLite (dev) / PostgreSQL (prod) |

## Estrutura do Projeto

```
futzada/
├── backend/
│   ├── FutDeQuarta.sln
│   └── FutDeQuarta.Api/
│       ├── Models/          # Entidades do banco
│       ├── Data/            # DbContext + Seed
│       ├── DTOs/            # Data Transfer Objects
│       ├── Services/        # Logica de negocio
│       └── Controllers/     # Endpoints da API
└── frontend/
    └── src/
        └── app/
            ├── models/      # Interfaces TypeScript
            ├── services/    # ApiService (HTTP)
            └── pages/       # Paginas Ionic
                ├── jogadores/   # Cadastro de jogadores
                ├── sessao/      # Sessao de jogo + lista de chegada
                ├── partida/     # Tela da partida com cronometro
                ├── ranking/     # Rankings com filtros
                └── historico/   # Historico de partidas
```

## Modelo de Banco (Entidades)

```
Jogador ─────────────── ListaChegada ────── SessaoJogo
  │                                            │
  ├── ParticipacaoPartida ── Time ── Partida ──┘
  │
  ├── Gol ── Assistencia
  │
  └── (stats calculadas via RankingConfiguracao)
```

### Entidades

| Entidade             | Descricao                                       |
|----------------------|-------------------------------------------------|
| Jogador              | Cadastro de jogadores (nome, apelido, ativo)    |
| SessaoJogo           | Um dia de jogo (data, local, modo goleiro)       |
| ListaChegada         | Ordem de chegada por sessao                     |
| Partida              | Uma partida (numero, status, duracao, modo)      |
| Time                 | Um time de 5 em uma partida (nome, cor, gols)   |
| ParticipacaoPartida  | Jogador <-> Time na partida (goleiro, resultado) |
| Gol                  | Gol marcado (jogador, time, minuto, gol contra) |
| Assistencia          | Assistencia para um gol                         |
| RankingConfiguracao  | Pontuacao configuravel                          |

### Regras de Pontuacao (configuravel)

| Evento      | Pontos |
|-------------|--------|
| Vitoria     | 3      |
| Empate      | 1      |
| Derrota     | 0      |
| Gol         | 2      |
| Assistencia | 1      |
| Gol contra  | -1     |

## API Endpoints

### Jogadores
- `GET    /api/jogadores` - Listar todos (query: `apenasAtivos`)
- `GET    /api/jogadores/:id` - Obter por ID
- `POST   /api/jogadores` - Criar jogador
- `PUT    /api/jogadores/:id` - Atualizar jogador
- `DELETE /api/jogadores/:id` - Remover jogador

### Sessoes de Jogo
- `GET    /api/sessoes` - Listar sessoes
- `GET    /api/sessoes/:id` - Obter sessao
- `POST   /api/sessoes` - Criar sessao
- `GET    /api/sessoes/:id/chegadas` - Lista de chegada
- `POST   /api/sessoes/:id/chegadas` - Adicionar jogador
- `DELETE /api/sessoes/:id/chegadas/:jogadorId` - Remover jogador

### Partidas
- `GET    /api/partidas/:id` - Obter partida
- `GET    /api/partidas/sessao/:sessaoId` - Partidas da sessao
- `POST   /api/partidas/formar-times` - Formar times e criar partida
- `POST   /api/partidas/:id/iniciar` - Iniciar cronometro
- `POST   /api/partidas/:id/finalizar` - Finalizar partida
- `POST   /api/partidas/:id/gols` - Registrar gol
- `GET    /api/partidas/historico` - Historico (query: `ano`, `mes`, `dataInicio`, `dataFim`)

### Ranking
- `GET    /api/ranking/geral` - Ranking geral (query: `ano`, `mes`)
- `GET    /api/ranking/artilharia` - Artilharia
- `GET    /api/ranking/assistencias` - Assistencias
- `GET    /api/ranking/participacao-gols` - Participacao em gols (G+A)
- `GET    /api/ranking/configuracoes` - Ver configuracoes de pontuacao
- `PUT    /api/ranking/configuracoes/:id` - Alterar pontuacao

## Como Rodar

### Backend

```bash
cd backend
dotnet restore
dotnet ef migrations add InitialCreate --project FutDeQuarta.Api
dotnet run --project FutDeQuarta.Api
```

A API roda em `http://localhost:5000` com Swagger em `/swagger`.

### Frontend

```bash
cd frontend
npm install
npm start
```

O frontend roda em `http://localhost:4200`.

Altere `src/environments/environment.ts` para apontar para o backend.

## Deploy Gratuito

### Backend (Railway ou Render)
1. Crie um repositorio no GitHub
2. No Railway/Render, conecte o repositorio
3. Configure variavel de ambiente:
   - `ConnectionStrings__DefaultConnection` = sua connection string PostgreSQL
   - `UsePostgres` = `true`
4. Build command: `dotnet publish -c Release -o out`
5. Start command: `dotnet out/FutDeQuarta.Api.dll`

### Banco (Neon ou Supabase)
1. Crie um banco PostgreSQL gratuito
2. Copie a connection string para o backend

### Frontend (Vercel ou Netlify)
1. Conecte o repositorio
2. Build command: `cd frontend && npm install && npm run build`
3. Output directory: `frontend/dist/fut-de-quarta`
4. Configure a variavel de ambiente `apiUrl` no `environment.prod.ts`

## Fluxo de Uso

1. **Cadastrar jogadores** - Tela Jogadores
2. **Iniciar sessao** - Tela Jogar > "Iniciar Sessao"
3. **Check-in** - Jogadores vao sendo adicionados conforme chegam
4. **Formar times** - Com 10+ jogadores, "Formar Times" cria times de 5
5. **Jogar** - Iniciar partida, cronometro roda, registrar gols
6. **Finalizar** - Automatico (7min ou 2 gols) ou manual
7. **Ranking** - Tela Ranking com filtros por ano/mes

## Futuro

- [ ] Estatisticas por temporada
- [ ] Exportar ranking em CSV
- [ ] PWA completo (offline)
- [ ] Times equilibrados por skill rating
