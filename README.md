# Order Flow Base Exchange

Base inicial para o desafio de processamento de ordens da Coodesh, com um frontend React para geração de ordens e um backend .NET para acumular exposição financeira por ativo.

## Estrutura

- `backend/`: OrderAccumulator, API ASP.NET Core em C# organizada por camadas e conceitos de DDD.
- `frontend/`: OrderGenerator, aplicação React/TypeScript organizada por feature.
- `docs/`: contrato compartilhado da API.

## Tecnologias

- C# / .NET 8 / ASP.NET Core Web API.
- React + TypeScript + Vite.
- Domínio separado de infraestrutura e HTTP, com portas/interfaces para inversão de dependência.
- Repositório inicial em memória, preparado para substituição por persistência real.

## Como executar

Backend:

```bash
cd backend
dotnet restore
dotnet run --project src/OrderAccumulator.Api
```

Frontend:

```bash
cd frontend
npm install
npm run dev
```

Esta entrega cria somente a estrutura base e os contratos. O formulário completo, o caso de uso de recebimento e a integração REST serão implementados na próxima etapa.

## Regra central planejada

A exposição é mantida individualmente por ativo. Compras aumentam a exposição, vendas reduzem, e qualquer operação que leve o valor absoluto acima de R$ 1.000.000 deve ser rejeitada sem alterar o estado.

This is a challenge by [Coodesh](https://coodesh.com/).
