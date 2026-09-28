import { useState } from 'react';
import { OrderForm } from './components/order-form';
import { orderApi } from '../../shared/api/order-api';
import type { CreateOrderRequest, CreateOrderResponse } from '../../shared/contracts/order.contract';

const currencyFormatter = new Intl.NumberFormat('pt-BR', { style: 'currency', currency: 'BRL' });

// Feature page: coordena o estado da tela e apresenta a operação em linguagem de negócio.
export function OrderGeneratorPage() {
  const [isSubmitting, setIsSubmitting] = useState(false);
  const [result, setResult] = useState<CreateOrderResponse | null>(null);
  const [submittedOrder, setSubmittedOrder] = useState<CreateOrderRequest | null>(null);
  const [error, setError] = useState<string | null>(null);

  async function handleOrderSubmit(order: CreateOrderRequest) {
    setIsSubmitting(true); setResult(null); setSubmittedOrder(order); setError(null);
    try { setResult(await orderApi.createOrder(order)); }
    catch (requestError) { setError(requestError instanceof Error ? requestError.message : 'Erro inesperado ao enviar a ordem.'); }
    finally { setIsSubmitting(false); }
  }

  const isBuy = submittedOrder?.lado === 'C';
  const operationLabel = isBuy ? 'Compra' : 'Venda';
  const orderValue = submittedOrder ? submittedOrder.quantidade * submittedOrder.preco : 0;
  const formattedOrderValue = currencyFormatter.format(orderValue);
  const formattedExposure = result ? currencyFormatter.format(Math.abs(result.exposicao_atual)) : '';

  return (
    <main className="page-shell">
      <header className="page-header"><div className="brand-mark">OE</div><div><p className="eyebrow">Order Exchange</p><h1>Nova ordem</h1><p className="subtitle">Envie uma ordem e acompanhe a exposição financeira do ativo.</p></div></header>
      <div className="content-grid">
        <section className="card form-card"><div className="card-heading"><div><h2>Detalhes da ordem</h2><p>Preencha os dados para enviar ao acumulador.</p></div><span className="status-dot">API online</span></div><OrderForm isSubmitting={isSubmitting} onSubmit={handleOrderSubmit} /></section>
        <section className={'card result-card ' + (result?.sucesso ? 'success-card' : error || result ? 'error-card' : '')} aria-live="polite">
          <div className="card-heading"><div><h2>Retorno da operação</h2><p>Acompanhe o status da sua ordem.</p></div></div>
          {!result && !error && <div className="empty-result"><span>↗</span><p>A confirmação da ordem aparecerá aqui.</p></div>}
          {result && submittedOrder && <div className="response-content">
            <div className={'response-status ' + (isBuy ? 'buy-status' : 'sell-status')}>{result.sucesso ? '✓' : '×'} {operationLabel} {result.sucesso ? 'realizada' : 'não realizada'}</div>
            <div className="trade-summary"><span>{submittedOrder.ativo}</span><span>{submittedOrder.quantidade.toLocaleString('pt-BR')} ações</span><span>{currencyFormatter.format(submittedOrder.preco)} por ação</span></div>
            {result.sucesso ? <><div className="exposure-label">Exposição líquida de {submittedOrder.ativo}</div><strong className={'exposure-value ' + (isBuy ? 'positive-exposure' : 'negative-exposure')}>{isBuy ? '+' : '−'} {formattedExposure}</strong><p className="exposure-explanation">{isBuy ? 'Sua compra aumentou a exposição em ' + formattedOrderValue + '.' : 'Sua venda reduziu a exposição em ' + formattedOrderValue + '.'}</p></> : <p className="error-message">{result.msg_erro || 'A ordem não foi aceita.'}</p>}
          </div>}
          {error && <div className="response-content"><div className="response-status">× Falha no envio</div><p className="error-message">{error}</p><p className="connection-hint">Verifique se a API está em execução em http://localhost:3333.</p></div>}
        </section>
      </div>
      <footer>Limite de exposição por ativo: <strong>R$ 1.000.000,00</strong></footer>
    </main>
  );
}

