// Creating same type as defined in Entities
export interface Transaction { 
  transactionId: number;
  tradeId: number;
  version: number;
  securityCode: string;
  quantity: number;
  action: string;
  side: string;
  createdAt: string;
  isProcessed: boolean;
}

export interface Position {
  securityCode: string;
  quantity: number;
  lastUpdatedAt: string;
}

export interface ApiResponse<T> {
  success: boolean;
  message: string;
  data: T | null;
  errors: string[];
}

export interface CreateTransactionRequest {
  tradeId: number;
  version: number;
  securityCode: string;
  quantity: number;
  action: 'INSERT' | 'UPDATE' | 'CANCEL';
  side: 'Buy' | 'Sell';
}

