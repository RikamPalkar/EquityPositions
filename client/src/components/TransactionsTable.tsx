import { useEffect, useState } from 'react';
import type { Transaction } from '../types';
import { transactionService } from '../services/api';
import { formatUtcToLocal } from '../utils/dateUtils';

interface TransactionsTableProps {
  refreshTrigger: number;
}

export function TransactionsTable({ refreshTrigger }: TransactionsTableProps) {
  const [transactions, setTransactions] = useState<Transaction[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  const fetchTransactions = async () => {
    try {
      setLoading(true);
      const response = await transactionService.getAll();
      if (response.success && response.data) {
        setTransactions(response.data);
      } else {
        setError(response.message || 'Failed to fetch transactions');
      }
    } catch (err) {
      setError('Network error or API is not running.');
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    fetchTransactions();
  }, [refreshTrigger]);

  const getActionClass = (action: string) => {
    switch (action.toUpperCase()) {
      case 'INSERT': return 'action-insert';
      case 'UPDATE': return 'action-update';
      case 'CANCEL': return 'action-cancel';
      default: return '';
    }
  };

  const getSideClass = (side: string) => {
    return side.toLowerCase() === 'buy' ? 'side-buy' : 'side-sell';
  };

  if (loading) {
    return <div className="table-container"><div className="loading">Loading transactions...</div></div>;
  }

  if (error) {
    return <div className="table-container"><div className="error">{error}</div></div>;
  }

  return (
    <div className="table-container transactions-table">
      <div className="table-header">
        <h2>Transaction History</h2>
        <button onClick={fetchTransactions} className="refresh-btn">Refresh</button>
      </div>
      {transactions.length === 0 ? (
        <div className="empty-state">No transactions yet.</div>
      ) : (
        <table>
          <thead>
            <tr>
              <th>ID</th>
              <th>Trade ID</th>
              <th>Version</th>
              <th>Security</th>
              <th>Quantity</th>
              <th>Action</th>
              <th>Side</th>
              <th>Created At</th>
            </tr>
          </thead>
          <tbody>
            {transactions.map((tx) => (
              <tr key={tx.transactionId}>
                <td>{tx.transactionId}</td>
                <td>{tx.tradeId}</td>
                <td>{tx.version}</td>
                <td className="security-code">{tx.securityCode}</td>
                <td>{tx.quantity}</td>
                <td><span className={`badge ${getActionClass(tx.action)}`}>{tx.action}</span></td>
                <td><span className={`badge ${getSideClass(tx.side)}`}>{tx.side}</span></td>
                <td className="timestamp">{formatUtcToLocal(tx.createdAt)}</td>
              </tr>
            ))}
          </tbody>
        </table>
      )}
    </div>
  );
}

