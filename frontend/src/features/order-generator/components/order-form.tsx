import type { FormEvent } from 'react';
import type { Asset, Side } from '../../../shared/contracts/order.contract';

interface OrderFormProps { onSubmit: (data: { ativo: Asset; lado: Side; quantidade: number; preco: number }) => void; }

// Componente visual isolado: recebe callback e não decide como persistir a ordem.
export function OrderForm({ onSubmit }: OrderFormProps) {
  function handleSubmit(event: FormEvent<HTMLFormElement>) {
    event.preventDefault();
    // Placeholder intencional para manter a estrutura sem implementação grossa.
    onSubmit({ ativo: 'PETR4', lado: 'C', quantidade: 0, preco: 0 });
  }
  return <form onSubmit={handleSubmit}><p>Campos de ativo, lado, quantidade e preço serão adicionados aqui.</p><button type="submit">Enviar ordem</button></form>;
}
