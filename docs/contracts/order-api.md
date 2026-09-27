# Contrato da API de ordens

## Endpoint planejado

`POST /orders`

### Requisição

```json
{
  "ativo": "PETR4",
  "lado": "C",
  "quantidade": 584,
  "preco": 54.87
}
```

Valores válidos:

- `ativo`: `PETR4`, `VALE3` ou `VIIA4`.
- `lado`: `C` para compra ou `V` para venda.
- `quantidade`: inteiro positivo menor que `100000`.
- `preco`: positivo, menor que `1000` e múltiplo de `0.01`.

### Resposta

```json
{
  "sucesso": true,
  "exposicao_atual": 32044.08,
  "msg_erro": ""
}
```

Ordens rejeitadas não alteram a exposição do ativo. A exposição é calculada por ativo e deve permanecer no intervalo `[-1000000, 1000000]`.

