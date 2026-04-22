import { FC, useState } from 'react';
import { Download, CheckCircle, AlertCircle, Zap } from 'lucide-react';
import { Layout } from '@/components/Layout/Layout';
import { Button } from '@/components/Common/Button';
import { Input } from '@/components/Common/Input';
import { Loading } from '@/components/Common/Loading';
import { useImport } from '@/hooks/useImport';
import { useImportSkills } from '@/hooks/useSkills';

export const HomePage: FC = () => {
  const [vacancySearch, setVacancySearch] = useState('');
  const [skillsFilter, setSkillsFilter] = useState('');
  const { import: importVacancies, isImporting, successMessage: vacancyMessage } = useImport();
  const { import: importSkills, isImporting: isImportingSkills, successMessage: skillsMessage } = useImportSkills();

  const handleImportVacancies = () => {
    if (vacancySearch.trim()) {
      importVacancies(vacancySearch);
      setVacancySearch('');
    }
  };

  const handleImportSkills = () => {
    importSkills(skillsFilter ? { vacancyNameFilter: skillsFilter } : undefined);
    setSkillsFilter('');
  };

  const handleKeyPress = (e: React.KeyboardEvent<HTMLInputElement>, action: () => void) => {
    if (e.key === 'Enter') {
      action();
    }
  };

  return (
    <Layout logo="/logo.svg">
      <div className="max-w-5xl mx-auto animate-fade-in">
        <div className="mb-8">
          <h1 className="text-4xl font-bold text-dark mb-2">Управление вакансиями и навыками</h1>
          <p className="text-gray-600">
            Загружайте вакансии с HeadHunter и верифицируйте извлечённые навыки
          </p>
        </div>

        <div className="grid grid-cols-1 lg:grid-cols-2 gap-8">
          {/* Vacancies Import Card */}
          <div className="bg-white rounded-xl shadow-card p-8">
            <div className="flex justify-center mb-6">
              <div className="w-16 h-16 bg-primary bg-opacity-10 rounded-full flex items-center justify-center">
                <Download className="text-primary" size={32} />
              </div>
            </div>

            <h2 className="text-2xl font-bold text-dark mb-4">Загрузить вакансии</h2>
            <p className="text-gray-600 mb-6 text-sm">
              Получайте вакансии с сайта HeadHunter и добавляйте их в базу данных
            </p>

            <div className="space-y-4 mb-6">
              <label className="block">
                <span className="text-sm font-medium text-dark mb-2 block">
                  Поиск по названию вакансии
                </span>
                <Input
                  placeholder="Например: C# Developer, React, Python..."
                  value={vacancySearch}
                  onChange={(e) => setVacancySearch(e.target.value)}
                  onKeyPress={(e) => handleKeyPress(e, handleImportVacancies)}
                  disabled={isImporting}
                />
              </label>
            </div>

            <Button
              onClick={handleImportVacancies}
              disabled={!vacancySearch.trim() || isImporting}
              isLoading={isImporting}
              size="lg"
              className="w-full"
            >
              <Download size={20} />
              {isImporting ? 'Загружаю...' : 'Загрузить вакансии'}
            </Button>

            {isImporting && (
              <div className="mt-6">
                <Loading message="Получаю данные с HeadHunter..." />
              </div>
            )}

            {vacancyMessage && (
              <div
                className={`mt-4 p-4 rounded-lg flex items-center gap-3 ${
                  vacancyMessage.startsWith('Ошибка')
                    ? 'bg-red-50 border border-red-200'
                    : 'bg-green-50 border border-green-200'
                }`}
              >
                {vacancyMessage.startsWith('Ошибка') ? (
                  <AlertCircle className="text-red-600 flex-shrink-0" size={20} />
                ) : (
                  <CheckCircle className="text-green-600 flex-shrink-0" size={20} />
                )}
                <p
                  className={`text-sm font-medium ${
                    vacancyMessage.startsWith('Ошибка') ? 'text-red-700' : 'text-green-700'
                  }`}
                >
                  {vacancyMessage}
                </p>
              </div>
            )}
          </div>

          {/* Skills Import Card */}
          <div className="bg-white rounded-xl shadow-card p-8">
            <div className="flex justify-center mb-6">
              <div className="w-16 h-16 bg-purple-100 rounded-full flex items-center justify-center">
                <Zap className="text-purple-600" size={32} />
              </div>
            </div>

            <h2 className="text-2xl font-bold text-dark mb-4">Верифицировать навыки</h2>
            <p className="text-gray-600 mb-6 text-sm">
              Извлеките навыки из загруженных вакансий и верифицируйте их
            </p>

            <div className="space-y-4 mb-6">
              <label className="block">
                <span className="text-sm font-medium text-dark mb-2 block">
                  Фильтр по названию вакансии (опционально)
                </span>
                <Input
                  placeholder="Например: C# разработчик (для фильтра при импорте)"
                  value={skillsFilter}
                  onChange={(e) => setSkillsFilter(e.target.value)}
                  onKeyPress={(e) => handleKeyPress(e, handleImportSkills)}
                  disabled={isImportingSkills}
                />
              </label>
            </div>

            <Button
              onClick={handleImportSkills}
              disabled={isImportingSkills}
              isLoading={isImportingSkills}
              size="lg"
              className="w-full bg-purple-600 hover:bg-purple-700"
            >
              <Zap size={20} />
              {isImportingSkills ? 'Импортирую навыки...' : 'Импортировать навыки'}
            </Button>

            {isImportingSkills && (
              <div className="mt-6">
                <Loading message="Извлекаю навыки из вакансий..." />
              </div>
            )}

            {skillsMessage && (
              <div
                className={`mt-4 p-4 rounded-lg flex items-center gap-3 ${
                  skillsMessage.startsWith('Ошибка')
                    ? 'bg-red-50 border border-red-200'
                    : 'bg-green-50 border border-green-200'
                }`}
              >
                {skillsMessage.startsWith('Ошибка') ? (
                  <AlertCircle className="text-red-600 flex-shrink-0" size={20} />
                ) : (
                  <CheckCircle className="text-green-600 flex-shrink-0" size={20} />
                )}
                <p
                  className={`text-sm font-medium ${
                    skillsMessage.startsWith('Ошибка') ? 'text-red-700' : 'text-green-700'
                  }`}
                >
                  {skillsMessage}
                </p>
              </div>
            )}
          </div>
        </div>

        {/* Info Section */}
        <div className="mt-12 bg-blue-50 border border-blue-200 rounded-xl p-6">
          <h3 className="font-semibold text-dark mb-3">💡 Как это работает?</h3>
          <div className="grid grid-cols-1 lg:grid-cols-2 gap-8">
            <div>
              <h4 className="font-semibold text-dark mb-2">Загрузка вакансий</h4>
              <ul className="space-y-2 text-sm text-gray-700">
                <li className="flex gap-2">
                  <span className="text-primary font-bold">1.</span>
                  <span>Введите название должности или технологию</span>
                </li>
                <li className="flex gap-2">
                  <span className="text-primary font-bold">2.</span>
                  <span>Нажмите "Загрузить вакансии"</span>
                </li>
                <li className="flex gap-2">
                  <span className="text-primary font-bold">3.</span>
                  <span>Система подключится к HeadHunter API</span>
                </li>
                <li className="flex gap-2">
                  <span className="text-primary font-bold">4.</span>
                  <span>Вакансии будут добавлены в базу данных</span>
                </li>
              </ul>
            </div>
            <div>
              <h4 className="font-semibold text-dark mb-2">Верификация навыков</h4>
              <ul className="space-y-2 text-sm text-gray-700">
                <li className="flex gap-2">
                  <span className="text-purple-600 font-bold">1.</span>
                  <span>Нажмите "Импортировать навыки"</span>
                </li>
                <li className="flex gap-2">
                  <span className="text-purple-600 font-bold">2.</span>
                  <span>Система извлечёт навыки из вакансий</span>
                </li>
                <li className="flex gap-2">
                  <span className="text-purple-600 font-bold">3.</span>
                  <span>Перейдите на вкладку "Верификация навыков"</span>
                </li>
                <li className="flex gap-2">
                  <span className="text-purple-600 font-bold">4.</span>
                  <span>Проверьте и одобрите навыки</span>
                </li>
              </ul>
            </div>
          </div>
        </div>
      </div>
    </Layout>
  );
};
