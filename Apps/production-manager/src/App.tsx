import { useState } from 'react';
import HealthPage from './pages/HealthPage';
import PendingIntrosPage from './pages/PendingIntrosPage';
import UserIntrosPage from './pages/UserIntrosPage';

type Page = 'health' | 'user-intros' | 'pending-intros';

export default function App() {
  const [page, setPage] = useState<Page>('health');

  return (
    <div className="min-h-screen bg-gray-950">
      <nav className="bg-gray-900 border-b border-gray-800 px-6 py-3 flex gap-4">
        <button
          onClick={() => setPage('health')}
          className={`text-sm font-medium px-3 py-1.5 rounded-md transition-colors ${
            page === 'health'
              ? 'bg-gray-800 text-gray-50'
              : 'text-gray-400 hover:text-gray-200'
          }`}
        >
          Health
        </button>
        <button
          onClick={() => setPage('user-intros')}
          className={`text-sm font-medium px-3 py-1.5 rounded-md transition-colors ${
            page === 'user-intros'
              ? 'bg-gray-800 text-gray-50'
              : 'text-gray-400 hover:text-gray-200'
          }`}
        >
          User Intros
        </button>
        <button
          onClick={() => setPage('pending-intros')}
          className={`text-sm font-medium px-3 py-1.5 rounded-md transition-colors ${
            page === 'pending-intros'
              ? 'bg-gray-800 text-gray-50'
              : 'text-gray-400 hover:text-gray-200'
          }`}
        >
          Pending Intros
        </button>
      </nav>
      {page === 'health' && <HealthPage />}
      {page === 'user-intros' && <UserIntrosPage />}
      {page === 'pending-intros' && <PendingIntrosPage />}
    </div>
  );
}
