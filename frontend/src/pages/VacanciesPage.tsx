import { FC, useState } from 'react';
import { Search, Plus } from 'lucide-react';
import { useQuery } from '@tanstack/react-query';
import { Layout } from '@/components/Layout/Layout';
import { VacancyCard } from '@/components/Vacancies/VacancyCard';
import { Button } from '@/components/Common/Button';
import { Input } from '@/components/Common/Input';
import { Modal } from '@/components/Common/Modal';
import { Pagination } from '@/components/Common/Pagination';
import { Loading } from '@/components/Common/Loading';
import { SkillFilter } from '@/components/Common/SkillFilter';
import { VacancyForm } from '@/components/Vacancies/VacancyForm';
import { useVacancies } from '@/hooks/useVacancies';
import { vacancyAPI } from '@/services/api';
import { RawVacancy, CreateVacancyRequest, UpdateVacancyRequest } from '@/types/vacancy';

export const VacanciesPage: FC = () => {
  const [page, setPage] = useState(1);
  const [pageSize] = useState(10);
  const [search, setSearch] = useState('');
  const [searchInput, setSearchInput] = useState('');
  const [skillFilter, setSkillFilter] = useState('');
  const [isModalOpen, setIsModalOpen] = useState(false);
  const [selectedVacancy, setSelectedVacancy] = useState<RawVacancy | undefined>();

  // Fetch available skills
  const { data: skillsData } = useQuery({
    queryKey: ['skills'],
    queryFn: () => vacancyAPI.getSkills(),
    staleTime: 1000 * 60 * 30, // Cache for 30 minutes
  });

  const {
    vacancies,
    total,
    isLoading,
    isError,
    error,
    createVacancy,
    updateVacancy,
    deleteVacancy,
    isCreating,
    isUpdating,
    isDeleting,
  } = useVacancies(page, pageSize, search, skillFilter);

  const totalPages = Math.ceil(total / pageSize);

  const handleSearch = () => {
    setSearch(searchInput);
    setPage(1);
  };

  const handleCreateClick = () => {
    setSelectedVacancy(undefined);
    setIsModalOpen(true);
  };

  const handleEditClick = (vacancy: RawVacancy) => {
    setSelectedVacancy(vacancy);
    setIsModalOpen(true);
  };

  const handleModalClose = () => {
    setIsModalOpen(false);
    setSelectedVacancy(undefined);
  };

  const handleFormSubmit = (data: CreateVacancyRequest | UpdateVacancyRequest) => {
    if (selectedVacancy) {
      updateVacancy(
        { id: selectedVacancy.id, data: data as UpdateVacancyRequest },
        {
          onSuccess: () => {
            handleModalClose();
          },
        }
      );
    } else {
      createVacancy(data as CreateVacancyRequest, {
        onSuccess: () => {
          handleModalClose();
        },
      });
    }
  };

  const handleDeleteClick = (id: string) => {
    if (confirm('Вы уверены, что хотите удалить эту вакансию?')) {
      deleteVacancy(id);
    }
  };

  return (
    <Layout logo="/logo.svg">
      <div className="max-w-7xl animate-fade-in">
        {/* Header */}
        <div className="mb-8">
          <h1 className="text-3xl font-bold text-dark mb-2">Вакансии</h1>
          <p className="text-gray-600">Управление вакансиями и навыками</p>
        </div>

        {/* Search and Create */}
        <div className="flex gap-4 mb-6 flex-wrap items-center">
          <div className="flex-1 flex gap-2 min-w-64">
            <Input
              placeholder="Поиск по названию или описанию..."
              value={searchInput}
              onChange={(e) => setSearchInput(e.target.value)}
              onKeyPress={(e) => e.key === 'Enter' && handleSearch()}
            />
            <Button
              onClick={handleSearch}
              className="px-6"
            >
              <Search size={20} />
            </Button>
          </div>
          <SkillFilter
            value={skillFilter}
            onChange={(newSkill) => {
              setSkillFilter(newSkill);
              setPage(1);
            }}
            skills={skillsData?.items || []}
          />
          <Button
            onClick={handleCreateClick}
            className="px-6"
          >
            <Plus size={20} />
            Создать
          </Button>
        </div>

        {/* Content */}
        {isLoading ? (
          <Loading message="Загружаю вакансии..." />
        ) : isError ? (
          <div className="bg-red-50 border border-red-200 rounded-lg p-4 text-red-700">
            Ошибка: {error}
          </div>
        ) : vacancies.length === 0 ? (
          <div className="text-center py-12">
            <p className="text-gray-600 text-lg">Вакансий не найдено</p>
          </div>
        ) : (
          <>
            <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6 mb-8 auto-rows-max md:auto-rows-fr">
              {vacancies.map((vacancy) => (
                <VacancyCard
                  key={vacancy.id}
                  vacancy={vacancy}
                  onEdit={handleEditClick}
                  onDelete={handleDeleteClick}
                  isDeleting={isDeleting}
                />
              ))}
            </div>

            <Pagination
              currentPage={page}
              totalPages={totalPages}
              onPageChange={setPage}
            />
          </>
        )}
      </div>

      {/* Modal */}
      <Modal
        isOpen={isModalOpen}
        onClose={handleModalClose}
        title={selectedVacancy ? 'Редактировать вакансию' : 'Создать вакансию'}
      >
        <VacancyForm
          vacancy={selectedVacancy}
          onSubmit={handleFormSubmit}
          isLoading={isCreating || isUpdating}
        />
      </Modal>
    </Layout>
  );
};
