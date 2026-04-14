import { useState } from 'react';
import { TransactionForm } from './components/TransactionForm';
import { PositionsTable } from './components/PositionsTable';
import { TransactionsTable } from './components/TransactionsTable';
import './App.css';

function App() {
  const [refreshTrigger, setRefreshTrigger] = useState(0);

  const handleTransactionCreated = () => {
    setRefreshTrigger((prev) => prev + 1);
  };

  return (
    <div className="app">
      <header className="app-header">
        <h1>Equity Positions</h1>
      </header>

      <main className="app-main">
        <div className="left-panel">
          <TransactionForm onTransactionCreated={handleTransactionCreated} />
        </div>

        <div className="right-panel">
          <PositionsTable refreshTrigger={refreshTrigger} />
          <TransactionsTable refreshTrigger={refreshTrigger} />
        </div>
      </main>

      <footer className="app-footer">
        <p>Equity Positions Demo App - Rikam Palkar</p>
      </footer>
    </div>
  );
}

export default App;
