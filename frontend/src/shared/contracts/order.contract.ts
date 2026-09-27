// Contrato alinhado ao backend e ao enunciado do desafio.
export type Asset = 'PETR4' | 'VALE3' | 'VIIA4';
export type Side = 'C' | 'V';
export interface CreateOrderRequest { ativo: Asset; lado: Side; quantidade: number; preco: number; }
export interface CreateOrderResponse { sucesso: boolean; exposicao_atual: number; msg_erro: string; }
