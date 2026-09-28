import type { CreateOrderRequest, CreateOrderResponse } from '../contracts/order.contract';

export interface OrderApi { createOrder(input: CreateOrderRequest): Promise<CreateOrderResponse>; }

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

