import { FC, useState } from 'react';
import { Download, CheckCircle, AlertCircle } from 'lucide-react';
import { Layout } from '@/components/Layout/Layout';
import { Button } from '@/components/Common/Button';
import { Input } from '@/components/Common/Input';
import { Loading } from '@/components/Common/Loading';
import { useImport } from '@/hooks/useImport';

export const ImportPage: FC = () => {
  const [searchQuery, setSearchQuery] = useState('');
  const { import: importVacancies, isImporting, successMessage } = useImport();

  const handleImport = () => {
    if (searchQuery.trim()) {
      importVacancies(searchQuery);
      setSearchQuery('');
    }
  };

  const handleKeyPress = (e: React.KeyboardEvent<HTMLInputElement>) => {
    if (e.key === 'Enter' && searchQuery.trim()) {
      handleImport();
    }
  };

  return (
    <Layout logo="/logo.svg">
      <div className="max-w-2xl mx-auto animate-fade-in">
        {/* Header */}
        <div className="mb-8">
          <h1 className="text-3xl font-bold text-dark mb-2">Загрузить с HH</h1>
          <p className="text-gray-600">
            Найдите и загрузите вакансии с сайта HeadHunter
          </p>
        </div>

        {/* Import Card */}
        <div className="bg-white rounded-xl shadow-card p-8">
          {/* Icon */}
          <div className="flex justify-center mb-6">
            <div className="w-16 h-16 bg-primary bg-opacity-10 rounded-full flex items-center justify-center">
              <Download className="text-primary" size={32} />
            </div>
          </div>

          {/* Search Section */}
          <div className="space-y-4 mb-8">
            <label className="block">
              <span className="text-sm font-medium text-dark mb-2 block">
                Поиск по названию вакансии
              </span>
              <Input
                placeholder="Например: Frontend Developer, React, Python..."
                value={searchQuery}
                onChange={(e) => setSearchQuery(e.target.value)}
                onKeyPress={handleKeyPress}
                disabled={isImporting}
              />
            </label>
          </div>

          {/* Import Button */}
          <Button
            onClick={handleImport}
            disabled={!searchQuery.trim() || isImporting}
            isLoading={isImporting}
            size="lg"
            className="w-full"
          >
            <Download size={20} />
            {isImporting ? 'Загружаю вакансии...' : 'Загрузить вакансии'}
          </Button>

          {/* Loading State */}
          {isImporting && (
            <div className="mt-8">
              <Loading message="Получаю данные с HeadHunter. Это может занять некоторое время..." />
            </div>
          )}

          {/* Status Message */}
          {successMessage && (
            <div
              className={`mt-6 p-4 rounded-lg flex items-center gap-3 ${
                successMessage.startsWith('Ошибка')
                  ? 'bg-red-50 border border-red-200'
                  : 'bg-green-50 border border-green-200'
              }`}
            >
              {successMessage.startsWith('Ошибка') ? (
                <AlertCircle className="text-red-600 flex-shrink-0" size={20} />
              ) : (
                <CheckCircle className="text-green-600 flex-shrink-0" size={20} />
              )}
              <p
                className={`text-sm font-medium ${
                  successMessage.startsWith('Ошибка')
                    ? 'text-red-700'
                    : 'text-green-700'
                }`}
              >
                {successMessage}
              </p>
            </div>
          )}
        </div>

        {/* Info Section */}
        <div className="mt-12 bg-blue-50 border border-blue-200 rounded-xl p-6">
          <h3 className="font-semibold text-dark mb-3">💡 Как это работает?</h3>
          <ul className="space-y-2 text-sm text-gray-700">
            <li className="flex gap-2">
              <span className="text-primary font-bold">1.</span>
              <span>Введите поисковый запрос (например, название должности или технологию)</span>
            </li>
            <li className="flex gap-2">
              <span className="text-primary font-bold">2.</span>
              <span>Нажмите "Загрузить вакансии"</span>
            </li>
            <li className="flex gap-2">
              <span className="text-primary font-bold">3.</span>
              <span>Система подключится к HeadHunter API и получит все доступные вакансии</span>
            </li>
            <li className="flex gap-2">
              <span className="text-primary font-bold">4.</span>
              <span>Вакансии и их навыки будут добавлены в базу данных</span>
            </li>
            <li className="flex gap-2">
              <span className="text-primary font-bold">5.</span>
              <span>Перейдите на вкладку "Вакансии", чтобы увидеть загруженные данные</span>
            </li>
          </ul>
        </div>
      </div>
    </Layout>
  );
};
