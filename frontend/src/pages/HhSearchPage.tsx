import { FC, useState } from 'react';
import { Search, ExternalLink } from 'lucide-react';
import { Layout } from '@/components/Layout/Layout';
import { Button } from '@/components/Common/Button';
import { Input } from '@/components/Common/Input';
import { Loading } from '@/components/Common/Loading';
import { Pagination } from '@/components/Common/Pagination';
import { useHhSearch, useHhVacancyDetails } from '@/hooks/useHhSearch';

interface HhVacancy {
  id: string;
  name: string;
  salary?: { from?: number; to?: number; currency: string };
  employer: { name: string };
  area: { name: string };
  url: string;
}

interface HhVacancyDetail {
  id: string;
  name: string;
  description?: string;
  employer: { name: string };
  salary?: { from?: number; to?: number; currency: string };
  key_skills?: Array<{ name: string }>;
  area: { name: string };
  url: string;
}

export const HhSearchPage: FC = () => {
  const [searchQuery, setSearchQuery] = useState('');
  const [currentPage, setCurrentPage] = useState(1);
  const [selectedVacancyId, setSelectedVacancyId] = useState<string | null>(null);

  const pageSize = 20;

  // Используем React Query для кеширования результатов поиска
  const {
    data: searchData,
    isLoading: isSearching,
    error: searchError,
  } = useHhSearch(searchQuery, currentPage - 1, pageSize);

  // Используем React Query для кеширования деталей вакансии
  const {
    data: vacancyDetails,
    isLoading: isLoadingDetails,
    error: detailsError,
  } = useHhVacancyDetails(selectedVacancyId);

  const searchResults = searchData?.items || [];
  const totalResults = searchData?.found || 0;
  const totalPages = Math.ceil(totalResults / pageSize);

  const handleSearch = () => {
    if (searchQuery.trim()) {
      setCurrentPage(1); // Сбросить на первую страницу при новом поиске
    }
  };

  return (
    <Layout logo="/logo.svg">
      <div className="max-w-7xl mx-auto animate-fade-in">
        <h1 className="text-3xl font-bold text-dark mb-2">Поиск на HeadHunter</h1>
        <p className="text-gray-600 mb-8">
          Найдите вакансии и просмотрите детали напрямую с HH API
        </p>

        {/* Search Bar */}
        <div className="bg-white rounded-xl shadow-card p-6 mb-8">
          <div className="flex gap-2">
            <Input
              placeholder="Frontend Developer, React, Python..."
              value={searchQuery}
              onChange={(e) => setSearchQuery(e.target.value)}
              onKeyPress={(e) => e.key === 'Enter' && handleSearch()}
              disabled={isSearching}
              className="flex-1"
            />
            <Button
              onClick={handleSearch}
              disabled={!searchQuery.trim() || isSearching}
              isLoading={isSearching}
              className="px-6"
            >
              <Search size={20} />
            </Button>
          </div>

          {searchError && (
            <div className="p-3 bg-red-50 border border-red-200 rounded-lg text-red-700 text-sm mt-3">
              {searchError instanceof Error ? searchError.message : 'Вакансии не найдены'}
            </div>
          )}
        </div>

        {/* Results Grid */}
        {searchResults.length > 0 && (
          <div className="grid grid-cols-1 lg:grid-cols-3 gap-6">
            {/* Left: Vacancy List */}
            <div className="lg:col-span-1">
              <div className="bg-white rounded-xl shadow-card overflow-hidden">
                <div className="p-4 border-b border-gray-200 bg-primary bg-opacity-5">
                  <h3 className="font-bold text-dark">
                    Найдено: {totalResults}
                  </h3>
                  <p className="text-xs text-gray-600 mt-1">
                    Страница {currentPage} из {totalPages}
                  </p>
                </div>
                <div className="space-y-1 max-h-96 overflow-y-auto">
                  {searchResults.map((vacancy) => (
                    <button
                      key={vacancy.id}
                      onClick={() => handleSelectVacancy(vacancy)}
                      className={`w-full text-left p-3 border-b border-gray-100 hover:bg-blue-50 transition-colors ${
                        selectedVacancyId === vacancy.id
                          ? 'bg-primary bg-opacity-10 border-l-4 border-l-primary'
                          : ''
                      }`}
                    >
                      <p className="font-medium text-dark text-sm line-clamp-2">
                        {vacancy.name}
                      </p>
                      <p className="text-xs text-gray-600 mt-1">
                        {vacancy.employer.name}
                      </p>
                      <p className="text-xs text-gray-500">
                        {vacancy.area.name}
                      </p>
                      {vacancy.salary && (
                        <p className="text-xs text-primary font-semibold mt-1">
                          {vacancy.salary.from?.toLocaleString()}-
                          {vacancy.salary.to?.toLocaleString()} {vacancy.salary.currency}
                        </p>
                      )}
                    </button>
                  ))}
                </div>
                {totalPages > 1 && (
                  <div className="p-4 border-t border-gray-200 bg-gray-50">
                    <Pagination
                      currentPage={currentPage}
                      totalPages={totalPages}
                      onPageChange={(page) => handleSearch(page - 1)}
                    />
                  </div>
                )}
              </div>
            </div>

            {/* Right: Vacancy Details */}
            <div className="lg:col-span-2">
              {isLoadingDetails ? (
                <div className="bg-white rounded-xl shadow-card p-6">
                  <Loading message="Загружаю детали..." />
                </div>
              ) : detailsError ? (
                <div className="bg-white rounded-xl shadow-card p-6">
                  <div className="p-3 bg-red-50 border border-red-200 rounded-lg text-red-700 text-sm">
                    {detailsError instanceof Error ? detailsError.message : 'Ошибка при загрузке деталей'}
                  </div>
                </div>
              ) : vacancyDetails ? (
                <div className="bg-white rounded-xl shadow-card overflow-hidden">
                  <div className="p-6 border-b border-gray-200 bg-primary bg-opacity-5">
                    <h2 className="text-2xl font-bold text-dark">
                      {vacancyDetails.name}
                    </h2>
                    <p className="text-primary font-semibold mt-1">
                      {vacancyDetails.employer.name}
                    </p>
                  </div>

                  <div className="p-6 space-y-4">
                    {/* Basic Info */}
                    <div className="grid grid-cols-2 gap-4">
                      <div>
                        <p className="text-xs text-gray-600 font-medium">Регион</p>
                        <p className="text-dark font-medium">
                          {vacancyDetails.area.name}
                        </p>
                      </div>
                      {vacancyDetails.salary && (
                        <div>
                          <p className="text-xs text-gray-600 font-medium">Зарплата</p>
                          <p className="text-primary font-semibold">
                            {vacancyDetails.salary.from?.toLocaleString()}-
                            {vacancyDetails.salary.to?.toLocaleString()}{' '}
                            {vacancyDetails.salary.currency}
                          </p>
                        </div>
                      )}
                    </div>

                    {/* Description */}
                    {vacancyDetails.description && (
                      <div>
                        <p className="text-xs text-gray-600 font-medium mb-2">
                          Описание
                        </p>
                        <div
                          className="text-sm text-gray-700 leading-relaxed prose prose-sm max-w-none prose-p:m-0 prose-p:mb-2 prose-ul:my-2 prose-li:my-0 prose-strong:font-semibold prose-em:italic"
                          dangerouslySetInnerHTML={{
                            __html: vacancyDetails.description,
                          }}
                        />
                      </div>
                    )}

                    {/* Skills */}
                    {vacancyDetails.key_skills && vacancyDetails.key_skills.length > 0 && (
                      <div>
                        <p className="text-xs text-gray-600 font-medium mb-2">
                          Навыки
                        </p>
                        <div className="flex flex-wrap gap-2">
                          {vacancyDetails.key_skills.map((skill, idx) => (
                            <span
                              key={idx}
                              className="px-3 py-1 bg-primary bg-opacity-10 text-primary text-xs font-medium rounded-full"
                            >
                              {skill.name}
                            </span>
                          ))}
                        </div>
                      </div>
                    )}

                    {/* Link */}
                    <div className="pt-4 border-t border-gray-200">
                      <a
                        href={`https://moscow.hh.ru/vacancy/${vacancyDetails.id}`}
                        target="_blank"
                        rel="noopener noreferrer"
                        className="inline-flex items-center gap-2 px-4 py-2 bg-primary hover:bg-blue-600 text-white rounded-lg font-medium transition-colors"
                      >
                        Открыть на HH
                        <ExternalLink size={16} />
                      </a>
                    </div>
                  </div>
                </div>
              ) : (
                <div className="bg-white rounded-xl shadow-card p-12 text-center">
                  <p className="text-gray-500">
                    Выберите вакансию из списка слева
                  </p>
                </div>
              )}
            </div>
          </div>
        )}
      </div>
    </Layout>
  );
};
