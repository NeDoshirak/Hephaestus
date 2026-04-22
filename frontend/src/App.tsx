import { BrowserRouter as Router, Routes, Route } from 'react-router-dom';
import { QueryClient, QueryClientProvider } from '@tanstack/react-query';
import { HomePage } from '@/pages/HomePage';
import { VacanciesPage } from '@/pages/VacanciesPage';
import { SkillsReviewPage } from '@/pages/SkillsReviewPage';
import { CleanSkillsPage } from '@/pages/CleanSkillsPage';
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
          <Route path="/" element={<HomePage />} />
          <Route path="/vacancies" element={<VacanciesPage />} />
          <Route path="/skills-review" element={<SkillsReviewPage />} />
          <Route path="/skills" element={<CleanSkillsPage />} />
          <Route path="/hh-search" element={<HhSearchPage />} />
        </Routes>
      </Router>
    </QueryClientProvider>
  );
}

export default App;
