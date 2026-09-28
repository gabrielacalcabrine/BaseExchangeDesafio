import type { CreateOrderRequest, CreateOrderResponse } from '../contracts/order.contract';

// Porta HTTP: os componentes dependem desta abstração e não conhecem o transporte.
export interface OrderApi { createOrder(input: CreateOrderRequest): Promise<CreateOrderResponse>; }

// Adapter REST: converte o contrato TypeScript em JSON e traduz a resposta da API.
const apiBaseUrl = import.meta.env.VITE_API_BASE_URL ?? 'http://localhost:3333';

export const orderApi: OrderApi = {
  async createOrder(input) {
    const response = await fetch(`${apiBaseUrl}/orders`, {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify(input),
    });
    const body = (await response.json()) as CreateOrderResponse;
    if (!response.ok) throw new Error(body.msg_erro || 'Não foi possível registrar a ordem.');
    return body;
  },
};

