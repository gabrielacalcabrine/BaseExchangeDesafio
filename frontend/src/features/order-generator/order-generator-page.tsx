import { OrderForm } from './components/order-form';
import type { CreateOrderRequest } from '../../shared/contracts/order.contract';

// Feature page: coordena o formulário e o espaço de resultado da API.
export function OrderGeneratorPage() {
  function handleOrderSubmit(order: CreateOrderRequest) { console.info('Ordem preparada para envio', order); }
  return <main><h1>Order Generator</h1><OrderForm onSubmit={handleOrderSubmit}/><section aria-live="polite"><h2>Resultado</h2><p>A resposta da requisição aparecerá aqui.</p></section></main>;
}
