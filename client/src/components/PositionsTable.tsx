import { useEffect, useState } from 'react';
import type { Position } from '../types';
import { positionService } from '../services/api';
import { formatUtcToLocal } from '../utils/dateUtils';

interface PositionsTableProps {
  refreshTrigger: number; // triggers a refresh of the new transaction added
}

export function PositionsTable({ refreshTrigger }: PositionsTableProps) {
  const [positions, setPositions] = useState<Position[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  const fetchPositions = async () => {
    try {
      setLoading(true);
      const response = await positionService.getAll();
      if (response.success && response.data) {
        setPositions(response.data);
      } else {
        setError(response.message || 'Failed to fetch positions');
      }
    } catch (err) {
      setError('Network error or API is not running.');
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => { //fetch data on mount and when refresh
    fetchPositions();
  }, [refreshTrigger]);

  const formatQuantity = (quantity: number) => { // format to show + or -
    if (quantity > 0) return `+${quantity}`;
    if (quantity < 0) return `${quantity}`; // backend returns -ve quantity on sell 
    return '0';
  };

  const getQuantityClass = (quantity: number) => {
    if (quantity > 0) return 'positive';
    if (quantity < 0) return 'negative';
    return 'neutral';
  };

  if (loading) {
    return <div className="table-container"><div className="loading">Loading positions...</div></div>;
  }

  if (error) {
    return <div className="table-container"><div className="error">{error}</div></div>;
  }

  return (
    <div className="table-container">
      <div className="table-header">
        <h2>Current Positions</h2>
        <button onClick={fetchPositions} className="refresh-btn">Refresh</button>
      </div>
      {positions.length === 0 ? (
        <div className="empty-state">No positions yet. Submit a transaction to get started.</div>
      ) : (
        <table>
          <thead>
            <tr>
              <th>Security Code</th>
              <th>Position</th>
              <th>Last Updated</th>
            </tr>
          </thead>
          <tbody>
            {positions.map((position) => (
              <tr key={position.securityCode}>
                <td className="security-code">{position.securityCode}</td>
                <td className={`quantity ${getQuantityClass(position.quantity)}`}>
                  {formatQuantity(position.quantity)}
                </td>
                <td className="timestamp">
                  {formatUtcToLocal(position.lastUpdatedAt)}
                </td>
              </tr>
            ))}
          </tbody>
        </table>
      )}
    </div>
  );
}

