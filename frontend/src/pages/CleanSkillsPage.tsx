import { FC, useState } from 'react';
import { Search, Tag, Hash, FileText, Zap, GitBranch, Plus } from 'lucide-react';
import { Layout } from '@/components/Layout/Layout';
import { Button } from '@/components/Common/Button';
import { Input } from '@/components/Common/Input';
import { Loading } from '@/components/Common/Loading';
import { Modal } from '@/components/Common/Modal';
import { useCleanSkills, useAddCleanSkill } from '@/hooks/useSkills';

type RelationshipFilter = 'all' | 'dependent' | 'parent';

export const CleanSkillsPage: FC = () => {
  const { skills, isLoading } = useCleanSkills();
  const { add, isAdding, successMessage } = useAddCleanSkill();
  const [searchQuery, setSearchQuery] = useState('');
  const [isModalOpen, setIsModalOpen] = useState(false);
  const [displayName, setDisplayName] = useState('');
  const [description, setDescription] = useState('');
  const [normalizedName, setNormalizedName] = useState('');
  const [relationshipFilter, setRelationshipFilter] = useState<RelationshipFilter>('all');

  const filteredSkills = skills.filter((skill) => {
    const matchesSearch =
      skill.displayName.toLowerCase().includes(searchQuery.toLowerCase()) ||
      skill.normalizedName.toLowerCase().includes(searchQuery.toLowerCase());

    if (!matchesSearch) return false;

    if (relationshipFilter === 'all') return true;
    if (relationshipFilter === 'dependent') return skill.dependentSkills && skill.dependentSkills.length > 0;
    if (relationshipFilter === 'parent') return skill.parentSkills && skill.parentSkills.length > 0;

    return true;
  });

  const handleAddSkill = () => {
    if (!displayName.trim()) return;
    add({
      displayName: displayName.trim(),
      description: description.trim() || undefined,
      normalizedName: normalizedName.trim() || undefined,
    });
    setDisplayName('');
    setDescription('');
    setNormalizedName('');
    setIsModalOpen(false);
  };

  if (isLoading) {
    return (
      <Layout logo="/logo.svg">
        <Loading message="Загружаю верифицированные навыки..." />
      </Layout>
    );
  }

  return (
    <Layout logo="/logo.svg">
      <div className="max-w-7xl mx-auto animate-fade-in">
        <div className="mb-8">
          <h1 className="text-4xl font-bold text-dark mb-2">Верифицированные навыки</h1>
          <p className="text-gray-600">
            Здесь находятся все одобренные и верифицированные навыки
          </p>
        </div>

        {successMessage && (
          <div className={`mb-6 p-4 rounded-lg flex items-center gap-3 ${
            successMessage.startsWith('Ошибка')
              ? 'bg-red-50 border border-red-200'
              : 'bg-green-50 border border-green-200'
          }`}>
            <p className={`text-sm font-medium ${
              successMessage.startsWith('Ошибка') ? 'text-red-700' : 'text-green-700'
            }`}>
              {successMessage}
            </p>
          </div>
        )}

        <div className="mb-8 space-y-4">
          <div className="flex gap-3">
            <div className="relative flex-1">
              <Search className="absolute left-4 top-3 text-gray-400" size={20} />
              <Input
                placeholder="Поиск по названию или нормализованному имени..."
                value={searchQuery}
                onChange={(e) => setSearchQuery(e.target.value)}
                className="pl-12"
              />
            </div>
            <Button
              onClick={() => setIsModalOpen(true)}
              className="px-6"
            >
              <Plus size={20} />
              Добавить навык
            </Button>
          </div>

          <div className="flex gap-2">
            <label className="flex items-center gap-2 px-4 py-2 border border-gray-300 rounded-lg cursor-pointer hover:bg-gray-50 transition-colors" style={{ borderColor: relationshipFilter === 'all' ? '#5CBBF1' : undefined, backgroundColor: relationshipFilter === 'all' ? '#E0F4FF' : undefined }}>
              <input
                type="radio"
                name="filter"
                value="all"
                checked={relationshipFilter === 'all'}
                onChange={() => setRelationshipFilter('all')}
                className="w-4 h-4"
              />
              <span className="text-sm font-medium">Все</span>
            </label>
            <label className="flex items-center gap-2 px-4 py-2 border border-gray-300 rounded-lg cursor-pointer hover:bg-gray-50 transition-colors" style={{ borderColor: relationshipFilter === 'dependent' ? '#5CBBF1' : undefined, backgroundColor: relationshipFilter === 'dependent' ? '#E0F4FF' : undefined }}>
              <input
                type="radio"
                name="filter"
                value="dependent"
                checked={relationshipFilter === 'dependent'}
                onChange={() => setRelationshipFilter('dependent')}
                className="w-4 h-4"
              />
              <span className="text-sm font-medium">Только с зависимыми</span>
            </label>
            <label className="flex items-center gap-2 px-4 py-2 border border-gray-300 rounded-lg cursor-pointer hover:bg-gray-50 transition-colors" style={{ borderColor: relationshipFilter === 'parent' ? '#5CBBF1' : undefined, backgroundColor: relationshipFilter === 'parent' ? '#E0F4FF' : undefined }}>
              <input
                type="radio"
                name="filter"
                value="parent"
                checked={relationshipFilter === 'parent'}
                onChange={() => setRelationshipFilter('parent')}
                className="w-4 h-4"
              />
              <span className="text-sm font-medium">Только с родительскими</span>
            </label>
          </div>
        </div>

        {filteredSkills.length === 0 ? (
          <div className="bg-white rounded-xl shadow-card p-12 text-center">
            <div className="text-6xl mb-4">📚</div>
            <h2 className="text-2xl font-bold text-dark mb-2">
              {skills.length === 0 ? 'Нет верифицированных навыков' : 'Ничего не найдено'}
            </h2>
            <p className="text-gray-600">
              {skills.length === 0
                ? 'Начните с верификации навыков на вкладке "Верификация навыков"'
                : 'Попробуйте изменить поисковый запрос'}
            </p>
          </div>
        ) : (
          <div>
            <div className="mb-6 text-sm text-gray-600">
              Показано: <span className="font-bold">{filteredSkills.length}</span> из{' '}
              <span className="font-bold">{skills.length}</span> навыков
            </div>

            <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
              {filteredSkills.map((skill) => (
                <div
                  key={skill.id}
                  className="bg-white rounded-xl shadow-card hover:shadow-lg transition-all duration-300 overflow-hidden group"
                >
                  {/* Header with gradient */}
                  <div className="h-1 bg-gradient-to-r from-primary to-blue-500"></div>

                  <div className="p-6">
                    {/* Title */}
                    <h3 className="text-xl font-bold text-dark mb-1 line-clamp-2 group-hover:text-primary transition-colors">
                      {skill.displayName}
                    </h3>

                    {/* Normalized Name */}
                    <div className="flex items-center gap-2 mb-4">
                      <Tag size={14} className="text-gray-400" />
                      <span className="text-xs font-mono bg-gray-100 text-gray-700 px-2 py-1 rounded">
                        {skill.normalizedName}
                      </span>
                    </div>

                    {/* Description */}
                    {skill.description && (
                      <div className="mb-4 p-3 bg-blue-50 border border-blue-100 rounded-lg">
                        <div className="flex items-start gap-2">
                          <FileText size={14} className="text-blue-600 flex-shrink-0 mt-0.5" />
                          <p className="text-sm text-gray-700 line-clamp-3">
                            {skill.description}
                          </p>
                        </div>
                      </div>
                    )}

                    {/* Counter */}
                    <div className="mb-4 flex items-center gap-2 p-3 bg-green-50 border border-green-100 rounded-lg">
                      <Hash size={16} className="text-green-600" />
                      <span className="text-sm font-medium text-green-700">
                        Встречалось {skill.counter} раз(а)
                      </span>
                    </div>

                    {/* Synonyms */}
                    {skill.synonyms && skill.synonyms.length > 0 && (
                      <div className="mb-4">
                        <div className="flex items-center gap-2 mb-2">
                          <Zap size={14} className="text-purple-600" />
                          <span className="text-xs font-semibold text-purple-700 uppercase">
                            Синонимы
                          </span>
                        </div>
                        <div className="flex flex-wrap gap-2">
                          {skill.synonyms.map((synonym) => (
                            <span
                              key={synonym.id}
                              className="px-2 py-1 bg-purple-100 text-purple-700 text-xs rounded-full font-medium hover:bg-purple-200 transition-colors"
                              title={synonym.isFromNormalization ? 'Из нормализации' : 'Добавлено вручную'}
                            >
                              {synonym.synonymName}
                            </span>
                          ))}
                        </div>
                      </div>
                    )}

                    {/* Dependent Skills */}
                    {skill.dependentSkills && skill.dependentSkills.length > 0 && (relationshipFilter === 'all' || relationshipFilter === 'dependent') && (
                      <div className="mb-4">
                        <div className="flex items-center gap-2 mb-2">
                          <GitBranch size={14} className="text-orange-600" />
                          <span className="text-xs font-semibold text-orange-700 uppercase">
                            Зависимые навыки
                          </span>
                        </div>
                        <div className="space-y-2">
                          {skill.dependentSkills.map((dep) => (
                            <div
                              key={dep.id}
                              className="p-2 bg-orange-50 border border-orange-100 rounded-lg hover:bg-orange-100 transition-colors"
                            >
                              <div className="font-medium text-sm text-orange-900">
                                {dep.displayName}
                              </div>
                              <div className="text-xs text-orange-700 font-mono">
                                {dep.normalizedName}
                              </div>
                            </div>
                          ))}
                        </div>
                      </div>
                    )}

                    {/* Parent Skills */}
                    {skill.parentSkills && skill.parentSkills.length > 0 && (relationshipFilter === 'all' || relationshipFilter === 'parent') && (
                      <div className="mb-4">
                        <div className="flex items-center gap-2 mb-2">
                          <GitBranch size={14} className="text-blue-600" />
                          <span className="text-xs font-semibold text-blue-700 uppercase">
                            Зависит от
                          </span>
                        </div>
                        <div className="space-y-2">
                          {skill.parentSkills.map((parent) => (
                            <div
                              key={parent.id}
                              className="p-2 bg-blue-50 border border-blue-100 rounded-lg hover:bg-blue-100 transition-colors"
                            >
                              <div className="font-medium text-sm text-blue-900">
                                {parent.displayName}
                              </div>
                              <div className="text-xs text-blue-700 font-mono">
                                {parent.normalizedName}
                              </div>
                            </div>
                          ))}
                        </div>
                      </div>
                    )}

                    {/* Footer - Metadata */}
                    <div className="pt-4 border-t border-gray-100 text-xs text-gray-500">
                      <p>
                        Создано:{' '}
                        <span className="font-medium">
                          {new Date(skill.createdAt).toLocaleDateString('ru-RU')}
                        </span>
                      </p>
                    </div>
                  </div>
                </div>
              ))}
            </div>
          </div>
        )}

        {/* Add Skill Modal */}
        <Modal
          title="Добавить новый навык"
          isOpen={isModalOpen}
          onClose={() => {
            setIsModalOpen(false);
            setDisplayName('');
            setDescription('');
            setNormalizedName('');
          }}
        >
          <div className="space-y-4">
            <div>
              <label className="block text-sm font-medium text-dark mb-2">
                Название навыка *
              </label>
              <Input
                value={displayName}
                onChange={(e) => setDisplayName(e.target.value)}
                placeholder="Например: TypeScript"
              />
              <p className="text-xs text-gray-500 mt-1">
                Нормализованное имя будет сгенерировано автоматически
              </p>
            </div>

            <div>
              <label className="block text-sm font-medium text-dark mb-2">
                Описание (опционально)
              </label>
              <textarea
                value={description}
                onChange={(e) => setDescription(e.target.value)}
                placeholder="Добавьте описание этого навыка..."
                rows={3}
                className="w-full px-4 py-2 border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-primary focus:border-transparent"
              />
            </div>

            <div className="flex gap-3 pt-4">
              <Button
                onClick={handleAddSkill}
                disabled={isAdding || !displayName.trim()}
                isLoading={isAdding}
                className="flex-1"
              >
                <Plus size={20} />
                Добавить
              </Button>
              <Button
                onClick={() => {
                  setIsModalOpen(false);
                  setDisplayName('');
                  setDescription('');
                  setNormalizedName('');
                }}
                disabled={isAdding}
                variant="secondary"
                className="flex-1"
              >
                Отмена
              </Button>
            </div>
          </div>
        </Modal>
      </div>
    </Layout>
  );
};
