# FCG - PaymentsAPI

Microsserviço de **Pagamentos** da FIAP Cloud Games (FCG).

Responsável por **processar (simular)** o pagamento de uma compra de jogo. Consome
`OrderPlacedEvent`, registra a transação no PostgreSQL e publica `PaymentProcessedEvent`
com o resultado (`Approved` ou `Rejected`).

## Papel no fluxo de compra

1. Consome **`OrderPlacedEvent`** (publicado pelo CatalogAPI).
2. Simula o processamento do pagamento e persiste a transação (`payments`).
3. Publica **`PaymentProcessedEvent`** com `Status = Approved | Rejected`, consumido por CatalogAPI e NotificationsAPI.

### Regra de simulação

A aprovação é intencionalmente simples e configurável para permitir demonstrar os dois caminhos:

- `amount <= 0` -> **Rejected** (valor inválido)
- `amount > ApprovalCeiling` -> **Rejected** (acima do limite)
- caso contrário -> **Approved**

O teto padrão (`ApprovalCeiling`) é `1000`. Para demonstrar uma rejeição, cadastre um jogo
com preço acima do teto (ou ajuste `Payments__ApprovalCeiling`).

O consumidor é **idempotente**: um mesmo `OrderId` não é cobrado duas vezes; o resultado
armazenado é republicado.

## Tecnologias

- .NET 10 (ASP.NET Core minimal API, apenas para health check)
- PostgreSQL (via EF Core + Npgsql) — registro das transações
- RabbitMQ (via MassTransit) — consumer e publisher

## Variáveis de ambiente

| Variável                        | Descrição                                        | Exemplo                                                                             |
|---------------------------------|--------------------------------------------------|-------------------------------------------------------------------------------------|
| `ConnectionStrings__Postgres`   | String de conexão do PostgreSQL (**secret**)     | `Host=postgres;Port=5432;Database=fcg_payments;Username=postgres;Password=postgres` |
| `Payments__ApprovalCeiling`     | Valor máximo aprovado automaticamente            | `1000`                                                                              |
| `RabbitMq__Host`                | Host do RabbitMQ                                  | `rabbitmq`                                                                          |
| `RabbitMq__Username`            | Usuário do RabbitMQ (**secret**)                 | `guest`                                                                             |
| `RabbitMq__Password`            | Senha do RabbitMQ (**secret**)                   | `guest`                                                                             |
| `ASPNETCORE_URLS`               | URL de escuta do Kestrel                          | `http://+:8080`                                                                     |

## Banco de dados (migrations)

O banco **não** é criado automaticamente. Script SQL idempotente pronto em [`db/init.sql`](db/init.sql):

```bash
psql -h localhost -U postgres -c "CREATE DATABASE fcg_payments;"
psql -h localhost -U postgres -d fcg_payments -f db/init.sql
```

Ou, com o EF Core: `dotnet ef database update --project src/PaymentsAPI`.

## Executando localmente

```bash
dotnet run --project src/PaymentsAPI
```

Requer PostgreSQL e RabbitMQ acessíveis (veja o `docker-compose.yml` na raiz).

## Docker

```bash
docker build -t fcg/payments-api:latest .
```

## Kubernetes

```bash
kubectl apply -f k8s/
kubectl get pods
```

- `configmap.yaml` — configuração não sensível (RabbitMQ host, teto de aprovação)
- `secret.yaml` — dados sensíveis (connection string, credenciais do RabbitMQ)
- `deployment.yaml` — Deployment do serviço
- `service.yaml` — Service ClusterIP (`payments-api:80` -> container `:8080`)
