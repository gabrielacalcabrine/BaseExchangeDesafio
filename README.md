# Order Flow Base Exchange

Aplicação para recebimento e acumulação de ordens de compra e venda, calculando a exposição financeira por ativo conforme o desafio técnico da Coodesh.

## Visão geral

O projeto é dividido em duas aplicações:

- **OrderGenerator**: frontend React responsável pelo preenchimento e envio das ordens.
- **OrderAccumulator**: backend REST responsável por validar, registrar as ordens aceitas e calcular a exposição financeira por ativo.

A exposição é calculada da seguinte forma:

```text
Exposição financeira =
somatório das compras - somatório das vendas
```

Cada ativo possui limite absoluto de **R$ 1.000.000,00**. Uma ordem que ultrapasse esse limite é rejeitada e não altera a exposição.

## Tecnologias utilizadas

### Backend

- C#
- .NET 8
- ASP.NET Core Web API
- DDD e princípios SOLID
- FluentValidation para validação dos requests
- Dapper para persistência e SQL manual
- PostgreSQL 16
- Swagger/OpenAPI
- xUnit para testes

### Frontend

- React 18
- TypeScript
- Vite
- CSS responsivo
- Integração REST com a API

## Estrutura do projeto

```text
backend/
├── src/
│   ├── OrderAccumulator.Api/             # Controllers, DTOs e composição da aplicação
│   ├── OrderAccumulator.Application/     # Casos de uso e portas de aplicação
│   ├── OrderAccumulator.Domain/          # Entidades, regras e Value Objects
│   └── OrderAccumulator.Infrastructure/  # Dapper, PostgreSQL e repositórios
├── tests/
│   └── OrderAccumulator.UnitTests/       # Testes unitários e de integração
├── docker/
│   └── initdb/                           # Script de criação do banco
└── OrderAccumulator.sln

frontend/
└── src/
    ├── app/                              # Composição e estilos globais
    ├── features/order-generator/         # Tela e formulário de ordens
    └── shared/                           # Contratos e adapter HTTP
```

## Pré-requisitos

Para executar o projeto localmente, instale:

- .NET SDK 8
- Node.js e npm
- Docker Desktop

## Como executar com Docker

Na raiz do projeto, execute:

```bash
docker compose up --build
```

A API estará disponível em:

- API: http://localhost:3333
- Swagger: http://localhost:3333/swagger
- PostgreSQL: localhost:5432

### Acesso local ao PostgreSQL

Use estes dados no DBeaver ou em outro cliente SQL:

```text
Host: localhost
Porta: 5432
Banco: exchange
Usuário: exchange
Senha: exchange
```

Para encerrar os containers:

```bash
docker compose down
```

Para remover também os dados persistidos do banco:

```bash
docker compose down -v
```

## Como executar o backend sem Docker

O banco PostgreSQL ainda precisa estar disponível e a connection string deve estar configurada.

```bash
cd backend
dotnet restore
dotnet run --project src/OrderAccumulator.Api
```

A API será executada em http://localhost:3333 quando iniciada pelo ambiente Docker. Em execução direta pelo .NET, a porta pode seguir as configurações do ambiente local.

## Como executar o frontend

Em outro terminal:

```bash
cd frontend
npm install
npm run dev
```

A aplicação ficará disponível em:

```text
http://localhost:5173
```

O frontend utiliza, por padrão, a API em:

```text
http://localhost:3333
```

Para configurar outra URL de API, crie um arquivo `.env.local` dentro de `frontend/`:

```env
VITE_API_BASE_URL=http://localhost:3333
```

## Endpoint

### Criar ordem

```http
POST http://localhost:3333/orders
Content-Type: application/json
```

Request:

```json
{
  "ativo": "PETR4",
  "lado": "C",
  "quantidade": 584,
  "preco": 54.87
}
```

Valores aceitos:

- `ativo`: `PETR4`, `VALE3` ou `VIIA4`
- `lado`: `C` para compra ou `V` para venda
- `quantidade`: inteiro positivo menor que 100.000
- `preco`: positivo, múltiplo de 0,01 e menor que 1.000

Resposta de sucesso:

```json
{
  "sucesso": true,
  "exposicao_atual": 32044.08,
  "msg_erro": ""
}
```

Quando uma ordem ultrapassa o limite de exposição, a resposta informa `sucesso: false` e a exposição não é alterada.

## Testes

Para executar todos os testes:

```bash
dotnet test backend/tests/OrderAccumulator.UnitTests/OrderAccumulator.UnitTests.csproj
```

Os testes unitários executam as regras de domínio e os casos de uso em memória. Os testes de integração utilizam o PostgreSQL configurado pelo Docker e limpam os dados usados ao final da execução.

## Build do frontend

```bash
cd frontend
npm run build
```

## Boas práticas adotadas

- Separação entre domínio, aplicação, infraestrutura e API.
- Value Objects para representar e validar ativo, lado, quantidade e preço.
- Contratos HTTP separados do domínio.
- Mapeamento manual entre request e comando.
- Interfaces para inversão de dependência.
- SQL explícito com Dapper.
- Controle transacional para registrar a ordem e atualizar a exposição.
- Testes unitários e de integração.
- Comentários no código explicando responsabilidades e decisões relevantes.

This is a challenge by [Coodesh](https://coodesh.com/).

