import { useState } from 'react';
import { Plus, Search, CheckCircle, AlertCircle } from 'lucide-react';
import { Layout } from '@/components/Layout/Layout';
import { Button } from '@/components/Common/Button';
import { Input } from '@/components/Common/Input';
import { Loading } from '@/components/Common/Loading';
import { ProfessionCard } from '@/components/Professions/ProfessionCard';
import { ProfessionDialog } from '@/components/Professions/ProfessionDialog';
import { useProfessions, useProfessionsCreation, useProfessionsUpdate, useProfessionsDeletion } from '@/hooks/useProfessions';
import { Profession, ProfessionDirection } from '@/types/vacancy';

export function ProfessionsPage() {
  const { data: professions = [], isLoading } = useProfessions();
  const createMutation = useProfessionsCreation();
  const updateMutation = useProfessionsUpdate();
  const deleteMutation = useProfessionsDeletion();

  const [dialogOpen, setDialogOpen] = useState(false);
  const [editingProfession, setEditingProfession] = useState<Profession | undefined>();
  const [searchQuery, setSearchQuery] = useState('');
  const [selectedDirection, setSelectedDirection] = useState<ProfessionDirection | 'all'>('all');
  const [successMessage, setSuccessMessage] = useState('');

  const directions: (ProfessionDirection | 'all')[] = [
    'all',
    ProfessionDirection.Programming,
    ProfessionDirection.Analytics,
    ProfessionDirection.Testing,
    ProfessionDirection.Design,
    ProfessionDirection.DevOps,
    ProfessionDirection.DataScience,
    ProfessionDirection.Management,
  ];

  const DIRECTION_LABELS: Record<ProfessionDirection | 'all', string> = {
    all: 'Все направления',
    [ProfessionDirection.Programming]: 'Программирование',
    [ProfessionDirection.Analytics]: 'Аналитика',
    [ProfessionDirection.Testing]: 'Тестирование',
    [ProfessionDirection.Design]: 'Дизайн',
    [ProfessionDirection.DevOps]: 'DevOps',
    [ProfessionDirection.DataScience]: 'Data Science',
    [ProfessionDirection.Management]: 'Управление',
  };

  const filteredProfessions = professions.filter((prof) => {
    const matchesSearch = prof.name.toLowerCase().includes(searchQuery.toLowerCase()) ||
      prof.description?.toLowerCase().includes(searchQuery.toLowerCase());
    const matchesDirection = selectedDirection === 'all' || prof.direction === selectedDirection;
    return matchesSearch && matchesDirection;
  });

  const handleCreate = (data: { name: string; description?: string; direction: ProfessionDirection }) => {
    createMutation.mutate(data, {
      onSuccess: () => {
        setDialogOpen(false);
        setSuccessMessage('Profession created successfully!');
        setTimeout(() => setSuccessMessage(''), 5000);
      },
      onError: (error) => {
        setSuccessMessage(`Error: ${error instanceof Error ? error.message : 'Unknown error'}`);
        setTimeout(() => setSuccessMessage(''), 5000);
      },
    });
  };

  const handleUpdate = (data: { name: string; description?: string; direction: ProfessionDirection }) => {
    if (!editingProfession) return;
    updateMutation.mutate(
      { id: editingProfession.id, ...data },
      {
        onSuccess: () => {
          setDialogOpen(false);
          setEditingProfession(undefined);
          setSuccessMessage('Profession updated successfully!');
          setTimeout(() => setSuccessMessage(''), 5000);
        },
        onError: (error) => {
          setSuccessMessage(`Error: ${error instanceof Error ? error.message : 'Unknown error'}`);
          setTimeout(() => setSuccessMessage(''), 5000);
        },
      }
    );
  };

  const handleDelete = (id: string) => {
    if (!window.confirm('Are you sure you want to delete this profession?')) return;
    deleteMutation.mutate(id, {
      onSuccess: () => {
        setSuccessMessage('Profession deleted successfully!');
        setTimeout(() => setSuccessMessage(''), 5000);
      },
      onError: (error) => {
        setSuccessMessage(`Error: ${error instanceof Error ? error.message : 'Unknown error'}`);
        setTimeout(() => setSuccessMessage(''), 5000);
      },
    });
  };

  const handleEdit = (profession: Profession) => {
    setEditingProfession(profession);
    setDialogOpen(true);
  };

  if (isLoading) {
    return (
      <Layout logo="/logo.svg">
        <Loading message="Loading professions..." />
      </Layout>
    );
  }

  return (
    <Layout logo="/logo.svg">
      <div className="max-w-7xl mx-auto animate-fade-in">
        <div className="mb-8">
          <h1 className="text-4xl font-bold text-dark mb-2">Профессии</h1>
          <p className="text-gray-600">
            Управляйте профессиональными должностями и карьерными путями в вашей организации
          </p>
        </div>

        {successMessage && (
          <div
            className={`mb-6 p-4 rounded-lg flex items-center gap-3 ${
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
                successMessage.startsWith('Ошибка') ? 'text-red-700' : 'text-green-700'
              }`}
            >
              {successMessage}
            </p>
          </div>
        )}

        <div className="mb-8 space-y-4">
          <div className="flex gap-3">
            <div className="relative flex-1">
              <Search className="absolute left-4 top-3 text-gray-400" size={20} />
              <Input
                placeholder="Поиск по названию или описанию..."
                value={searchQuery}
                onChange={(e) => setSearchQuery(e.target.value)}
                className="pl-12"
              />
            </div>
            <Button
              onClick={() => {
                setEditingProfession(undefined);
                setDialogOpen(true);
              }}
              className="px-6"
            >
              <Plus size={20} />
              Добавить профессию
            </Button>
          </div>

          <div className="flex gap-2 flex-wrap">
            {directions.map((dir) => (
              <button
                key={dir}
                onClick={() => setSelectedDirection(dir)}
                className={`px-4 py-2 rounded-lg transition-all duration-200 text-sm font-medium ${
                  selectedDirection === dir
                    ? 'bg-primary text-white'
                    : 'bg-gray-100 text-gray-700 hover:bg-gray-200'
                }`}
              >
                {DIRECTION_LABELS[dir]}
              </button>
            ))}
          </div>

          <div className="text-sm text-gray-600">
            Показано: <span className="font-bold">{filteredProfessions.length}</span> из{' '}
            <span className="font-bold">{professions.length}</span> профессий
          </div>
        </div>

        <ProfessionDialog
          isOpen={dialogOpen}
          profession={editingProfession}
          onClose={() => {
            setDialogOpen(false);
            setEditingProfession(undefined);
          }}
          onSubmit={editingProfession ? handleUpdate : handleCreate}
          isLoading={createMutation.isPending || updateMutation.isPending}
        />

        {filteredProfessions.length === 0 ? (
          <div className="bg-white rounded-xl shadow-card p-12 text-center">
            <div className="text-6xl mb-4">💼</div>
            <h2 className="text-2xl font-bold text-dark mb-2">
              {professions.length === 0 ? 'Профессии отсутствуют' : 'Профессии не найдены'}
            </h2>
            <p className="text-gray-600">
              {professions.length === 0
                ? 'Создайте первую профессию, чтобы начать'
                : 'Попробуйте изменить параметры поиска или фильтры'}
            </p>
          </div>
        ) : (
          <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
            {filteredProfessions.map((profession) => (
              <ProfessionCard
                key={profession.id}
                profession={profession}
                onEdit={handleEdit}
                onDelete={handleDelete}
                isDeleting={deleteMutation.isPending}
              />
            ))}
          </div>
        )}
      </div>
    </Layout>
  );
}
