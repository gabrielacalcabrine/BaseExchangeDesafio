import type { CreateOrderRequest, CreateOrderResponse } from '../contracts/order.contract';

// Porta do cliente HTTP: componentes não conhecem fetch, URL ou transporte.
export interface OrderApi { createOrder(input: CreateOrderRequest): Promise<CreateOrderResponse>; }

// Adapter planejado para a API REST; a implementação ficará para a próxima etapa.
export const orderApi: OrderApi = { async createOrder(_input) { throw new Error('Integração ainda não implementada.'); } };
