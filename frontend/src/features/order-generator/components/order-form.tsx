import type { FormEvent } from 'react';
import type { Asset, Side } from '../../../shared/contracts/order.contract';

interface OrderFormProps {
  isSubmitting: boolean;
  onSubmit: (data: { ativo: Asset; lado: Side; quantidade: number; preco: number }) => void;
}

export function OrderForm({ isSubmitting, onSubmit }: OrderFormProps) {
  function handleSubmit(event: FormEvent<HTMLFormElement>) {
    event.preventDefault();
    const formData = new FormData(event.currentTarget);
    onSubmit({
      ativo: formData.get('ativo') as Asset,
      lado: formData.get('lado') as Side,
      quantidade: Number(formData.get('quantidade')),
      preco: Number(formData.get('preco')),
    });
  }

  return (
    <form className="order-form" onSubmit={handleSubmit}>
      <div className="field-group">
        <label htmlFor="ativo">Ativo</label>
        <select id="ativo" name="ativo" defaultValue="PETR4">
          <option value="PETR4">PETR4 — Petrobras</option>
          <option value="VALE3">VALE3 — Vale</option>
          <option value="VIIA4">VIIA4 — Via</option>
        </select>
      </div>
      <fieldset className="field-group side-fieldset">
        <legend>Lado</legend>
        <div className="side-options">
          <label className="side-option buy-option"><input type="radio" name="lado" value="C" defaultChecked /><span>Compra</span></label>
          <label className="side-option sell-option"><input type="radio" name="lado" value="V" /><span>Venda</span></label>
        </div>
      </fieldset>
      <div className="form-grid">
        <div className="field-group">
          <label htmlFor="quantidade">Quantidade</label>
          <input id="quantidade" name="quantidade" type="number" min="1" max="99999" step="1" placeholder="Ex.: 584" required />
          <small>Inteiro positivo, menor que 100.000</small>
        </div>
        <div className="field-group">
          <label htmlFor="preco">Preço unitário</label>
          <div className="currency-input"><span>R$</span><input id="preco" name="preco" type="number" min="0.01" max="999.99" step="0.01" placeholder="Ex.: 54,87" required /></div>
          <small>Positivo, em centavos e menor que R$ 1.000</small>
        </div>
      </div>
      <button className="submit-button" type="submit" disabled={isSubmitting}>{isSubmitting ? 'Enviando ordem...' : 'Enviar ordem'}</button>
    </form>
  );
}

