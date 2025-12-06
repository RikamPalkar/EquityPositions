import { useState } from 'react';
import type { CreateTransactionRequest } from '../types';
import { transactionService } from '../services/api';

interface TransactionFormProps { // notify the App.tsx to refresh the tables
  onTransactionCreated: () => void;
}

export function TransactionForm({ onTransactionCreated }: TransactionFormProps) {
  const [formData, setFormData] = useState<CreateTransactionRequest>({
    tradeId: 1,
    version: 1,
    securityCode: '',
    quantity: 0,
    action: 'INSERT',
    side: 'Buy',
  });
  const [loading, setLoading] = useState(false); 
  const [error, setError] = useState<string | null>(null);
  const [success, setSuccess] = useState<string | null>(null); 

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setLoading(true);
    setError(null);
    setSuccess(null);

    try {
      const response = await transactionService.create(formData);
      if (response.success) {
        setSuccess(response.message);
        onTransactionCreated();
        setFormData({ // reset form data
          tradeId: formData.tradeId,
          version: formData.version + 1,
          securityCode: '',
          quantity: 0,
          action: 'INSERT',
          side: 'Buy',
        });
      } else {
        setError(response.message || 'Failed to create transaction');
      }
    } catch (err) {
      setError('Network error or API is not running.');
    } finally {
      setLoading(false);
    }
  };

  const handleChange = (
    e: React.ChangeEvent<HTMLInputElement | HTMLSelectElement>
  ) => {
    const { name, value } = e.target;
    setFormData((prev) => ({
      ...prev,
      [name]: name === 'tradeId' || name === 'version' || name === 'quantity'
        ? parseInt(value) || 0
        : value,
    }));
  };

  return (
    <div className="form-container">
      <h2>New Transaction</h2>
      <form onSubmit={handleSubmit}>
        <div className="form-row">
          <div className="form-group">
            <label htmlFor="tradeId">Trade ID</label>
            <input
              type="number"
              id="tradeId"
              name="tradeId"
              value={formData.tradeId}
              onChange={handleChange}
              min="1"
              required
            />
          </div>
          <div className="form-group">
            <label htmlFor="version">Version</label>
            <input
              type="number"
              id="version"
              name="version"
              value={formData.version}
              onChange={handleChange}
              min="1"
              required
            />
          </div>
        </div>

        <div className="form-row">
          <div className="form-group">
            <label htmlFor="securityCode">Security Code</label>
            <input
              type="text"
              id="securityCode"
              name="securityCode"
              value={formData.securityCode}
              onChange={handleChange}
              placeholder="e.g., REL, ITC, INF"
              maxLength={20}
              required
            />
          </div>
          <div className="form-group">
            <label htmlFor="quantity">Quantity</label>
            <input
              type="number"
              id="quantity"
              name="quantity"
              value={formData.quantity}
              onChange={handleChange}
              min="1"
              required
            />
          </div>
        </div>

        <div className="form-row">
          <div className="form-group">
            <label htmlFor="action">Action</label>
            <select
              id="action"
              name="action"
              value={formData.action}
              onChange={handleChange}
              required
            >
              <option value="INSERT">INSERT</option>
              <option value="UPDATE">UPDATE</option>
              <option value="CANCEL">CANCEL</option>
            </select>
          </div>
          <div className="form-group">
            <label htmlFor="side">Side</label>
            <select
              id="side"
              name="side"
              value={formData.side}
              onChange={handleChange}
              required
            >
              <option value="Buy">Buy</option>
              <option value="Sell">Sell</option>
            </select>
          </div>
        </div>

        <button type="submit" className="submit-btn" disabled={loading}>
          {loading ? 'Processing...' : 'Submit Transaction'}
        </button>

        {error && <div className="message error">{error}</div>}
        {success && <div className="message success">{success}</div>}
      </form>
    </div>
  );
}

