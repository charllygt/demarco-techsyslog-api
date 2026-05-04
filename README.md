# TechsysLog API

Backend .NET 8 com Clean Architecture para o sistema de controle de pedidos e entregas da TechsysLog. Implementa cadastro de usuários, pedidos e entregas com notificações em tempo real via SignalR.

## Stack

- **.NET 8** / **C# 12** (primary constructors, records, file-scoped namespaces, collection expressions)
- **MongoDB 7** (persistência única; `MongoClient` singleton + repos scoped)
- **JWT** (HS256, 4 validações ativas, exp 8h)
- **SignalR** (Hub fortemente tipado, auth via `?access_token=`)
- **FluentValidation** (pipeline behavior)
- **BCrypt.Net-Next** (work factor 12)
- **Polly** via `Microsoft.Extensions.Http.Resilience` (retry + circuit breaker no ViaCEP)
- **Serilog** (Console colorido + File rolling com correlation id)
- **Swashbuckle** + **Scalar** (dois UIs de docs a partir do mesmo OpenAPI)
- **xUnit** + **Shouldly** + **Bogus** + **NSubstitute** (testes)

## Arquitetura

Clean Architecture rigorosa. Regras de dependência:

```
Domain          → (nada — núcleo isolado)
Application     → Domain
Infrastructure  → Domain, Application
Api             → Application, Infrastructure
```

CQRS sem MediatR (dispatcher próprio ~80 linhas). Domain Events disparados após persistência via `IDomainEventDispatcher`. SignalR vive na Infrastructure; abstração `IRealtimeNotifier` mantém Application pura.

Estrutura:

```
src/
  TechsysLog.Domain/          # Aggregates, VOs, Events, Ports
  TechsysLog.Application/     # CQRS handlers, validators, behaviors
  TechsysLog.Infrastructure/  # Mongo, JWT, BCrypt, ViaCEP, SignalR
  TechsysLog.Api/             # Controllers, middleware, composition root
tests/
  TechsysLog.TestUtilities/   # Builders + Doubles
  TechsysLog.Domain.Tests/    # 109 testes unitários (≥90% coverage)
  TechsysLog.Application.Tests/
  TechsysLog.Api.Tests/       # integration (Plano futuro)
```

## Como rodar

### Pré-requisitos
- Docker + Docker Compose
- .NET 8 SDK (apenas se rodar fora de container — pinned em `global.json`)
- `make` (opcional, mas recomendado — Windows: Git Bash ou WSL)

### Modo 1 — Dev local (mais rápido, hot reload)

```bash
cp .env.example .env
make dev
```

Sobe Mongo no Docker, roda API com `dotnet run` (hot reload, breakpoints).

### Modo 2 — Stack completa em containers

```bash
cp .env.example .env
make full
```

Sobe Mongo + Mongo Express + API construindo a imagem.

### Modo 3 — Comandos diretos (sem make)

```bash
docker compose up -d mongo                                  # só Mongo
docker compose --profile tools up -d                        # + Mongo Express
docker compose --profile tools --profile full up -d --build # tudo
docker compose --profile tools --profile full down          # parar tudo
```

## Acesso

| Serviço | URL |
|---|---|
| API | http://localhost:5000 |
| Swagger UI | http://localhost:5000/swagger |
| Scalar (docs) | http://localhost:5000/scalar/v1 |
| Mongo Express | http://localhost:8081 |
| SignalR Hub | http://localhost:5000/hubs/notifications |

## Endpoints

| Método | Path | Auth | Descrição |
|---|---|---|---|
| POST | `/api/v1/users` | público | Cadastra usuário |
| POST | `/api/v1/auth/login` | público | Autentica e retorna JWT |
| POST | `/api/v1/orders` | JWT | Cria pedido |
| GET | `/api/v1/orders` | JWT | Lista pedidos paginados |
| GET | `/api/v1/orders/{id}` | JWT | Busca pedido |
| POST | `/api/v1/orders/{id}/deliveries` | JWT | Registra entrega |
| GET | `/api/v1/notifications` | JWT | Lista minhas notificações |
| PATCH | `/api/v1/notifications/{id}/read` | JWT | Marca como lida |
| GET | `/api/v1/cep/{cep}` | JWT | Lookup ViaCEP (cacheado 24h) |

## Testes

```bash
dotnet test
# ou: make test
```

121 testes unitários cobrindo Domain (109) + Application (12: Dispatcher, ValidationBehavior, LoggingBehavior, CreateUserCommandHandler).

## Frontend

O cliente Angular vive em `../techsyslog-web/` (repo separado) e consome esta API com notificações em tempo real via SignalR. Veja o [README do frontend](../techsyslog-web/README.md) para instruções completas.

## Decisões arquiteturais documentadas

Veja [`docs/specs/`](../docs/specs/) e [`docs/plans/`](../docs/plans/) para a spec original e os planos de implementação.

Decisões-chave registradas como comentário no código:
- **CQRS sem MediatR** (pago em 2025); dispatcher manual via DI.
- **Domain Events + handler dedicado** (não Outbox; documentado o caminho de evolução).
- **Auth customizada** (BCrypt + JWT), Domain `User` puro sem herdar de `IdentityUser`.
- **Notificação broadcast** para todos usuários (com comentário sobre cenário ideal: dono + role Operator).
- **Sem refresh tokens / sem Redis backplane** — escopo do teste; evolução documentada.
