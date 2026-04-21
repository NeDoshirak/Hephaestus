import { BrowserRouter as Router, Routes, Route, Navigate } from 'react-router-dom';
import { QueryClient, QueryClientProvider } from '@tanstack/react-query';
import { VacanciesPage } from '@/pages/VacanciesPage';
import { ImportPage } from '@/pages/ImportPage';
import { HhSearchPage } from '@/pages/HhSearchPage';

const queryClient = new QueryClient({
  defaultOptions: {
    queries: {
      staleTime: 1000 * 60 * 30, // 30 minutes - список вакансий редко меняется
      gcTime: 1000 * 60 * 60, // 60 minutes (formerly cacheTime)
      retry: 2, // Повторить при ошибке
      retryDelay: (attemptIndex) => Math.min(1000 * 2 ** attemptIndex, 30000),
    },
  },
});

function App() {
  return (
    <QueryClientProvider client={queryClient}>
      <Router>
        <Routes>
          <Route path="/vacancies" element={<VacanciesPage />} />
          <Route path="/import" element={<ImportPage />} />
          <Route path="/hh-search" element={<HhSearchPage />} />
          <Route path="/" element={<Navigate to="/vacancies" replace />} />
        </Routes>
      </Router>
    </QueryClientProvider>
  );
}

export default App;
