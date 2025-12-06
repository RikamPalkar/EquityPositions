import type { ApiResponse, Transaction, Position, CreateTransactionRequest } from '../types';

const API_BASE_URL = 'http://localhost:5277/api';

async function handleResponse<T>(response: Response): Promise<ApiResponse<T>> {
  const data = await response.json();
  return data as ApiResponse<T>;
}

export const transactionService = {
  async getAll(): Promise<ApiResponse<Transaction[]>> {
    const response = await fetch(`${API_BASE_URL}/transactions`);
    return handleResponse<Transaction[]>(response);
  },

  async create(request: CreateTransactionRequest): Promise<ApiResponse<Transaction>> {
    const response = await fetch(`${API_BASE_URL}/transactions`, {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json',
      },
      body: JSON.stringify(request),
    });
    return handleResponse<Transaction>(response);
  },
};

export const positionService = {
  async getAll(): Promise<ApiResponse<Position[]>> {
    const response = await fetch(`${API_BASE_URL}/positions`);
    return handleResponse<Position[]>(response);
  },
};
