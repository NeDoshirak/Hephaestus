import { FC, useState } from 'react';
import { CheckCircle, AlertCircle, Trash2, Edit2, Search, ChevronDown, ChevronUp } from 'lucide-react';
import { Layout } from '@/components/Layout/Layout';
import { Button } from '@/components/Common/Button';
import { Input } from '@/components/Common/Input';
import { Loading } from '@/components/Common/Loading';
import { useSkillsOnReview, useApproveSkill, useRejectSkill, useCleanSkills } from '@/hooks/useSkills';

type ApprovalMode = 'new' | 'synonym' | 'dependency';

export const SkillsReviewPage: FC = () => {
  const { skills, isLoading: isLoadingSkills } = useSkillsOnReview();
  const { skills: cleanSkills, isLoading: isLoadingCleanSkills } = useCleanSkills();
  const { approve, isApproving, successMessage: approveMessage } = useApproveSkill();
  const { reject, isRejecting, successMessage: rejectMessage } = useRejectSkill();

  const [searchQuery, setSearchQuery] = useState('');
  const [selectedSkill, setSelectedSkill] = useState<any | null>(null);
  const [approvalMode, setApprovalMode] = useState<ApprovalMode>('new');
  const [displayName, setDisplayName] = useState('');
  const [description, setDescription] = useState('');
  const [selectedExistingSkill, setSelectedExistingSkill] = useState<string | null>(null);
  const [existingSkillSearch, setExistingSkillSearch] = useState('');
  const [selectedDependency, setSelectedDependency] = useState<string | null>(null);
  const [dependencySearch, setDependencySearch] = useState('');

  const filteredSkills = skills.filter((skill) =>
    skill.originalName.toLowerCase().includes(searchQuery.toLowerCase()) ||
    skill.normalizedName.toLowerCase().includes(searchQuery.toLowerCase())
  );

  const filteredExistingSkills = cleanSkills.filter((skill) =>
    skill.displayName.toLowerCase().includes(existingSkillSearch.toLowerCase()) ||
    skill.normalizedName.toLowerCase().includes(existingSkillSearch.toLowerCase())
  );

  const successMessage = approveMessage || rejectMessage;

  const handleOpenModal = (skill: any) => {
    setSelectedSkill(skill);
    setDisplayName(skill.suggestedDisplayName);
    setDescription('');
    setApprovalMode('new');
    setSelectedExistingSkill(null);
    setExistingSkillSearch('');
    setSelectedDependency(null);
    setDependencySearch('');
  };

  const handleCloseModal = () => {
    setSelectedSkill(null);
    setDisplayName('');
    setDescription('');
    setApprovalMode('new');
    setSelectedExistingSkill(null);
    setExistingSkillSearch('');
    setSelectedDependency(null);
    setDependencySearch('');
  };

  const handleApprove = () => {
    if (!selectedSkill) return;

    if (approvalMode === 'new') {
      if (!displayName.trim()) return;
      approve({
        skillId: selectedSkill.id,
        request: {
          displayName: displayName.trim(),
          description: description.trim() || undefined,
          synonyms: [],
          children: [],
        },
      });
    } else if (approvalMode === 'synonym' && selectedExistingSkill) {
      approve({
        skillId: selectedSkill.id,
        request: {
          displayName: selectedSkill.suggestedDisplayName,
          description: undefined,
          synonyms: [],
          children: [],
          existingCleanSkillId: selectedExistingSkill,
        },
      });
    } else if (approvalMode === 'dependency') {
      if (!displayName.trim() || !selectedDependency) return;
      const dep = cleanSkills.find(s => s.id === selectedDependency);
      if (!dep) return;
      approve({
        skillId: selectedSkill.id,
        request: {
          displayName: displayName.trim(),
          description: description.trim() || undefined,
          synonyms: [],
          children: [
            {
              parentNormalizedName: dep.normalizedName,
              relationType: 'is_variant',
            },
          ],
        },
      });
    }
    handleCloseModal();
  };

  const handleReject = (skillId: string) => {
    if (window.confirm('Вы уверены что хотите отклонить этот навык?')) {
      reject(skillId);
    }
  };

  if (isLoadingSkills) {
    return (
      <Layout logo="/logo.svg">
        <Loading message="Загружаю навыки на проверке..." />
      </Layout>
    );
  }

  return (
    <Layout logo="/logo.svg">
      <div className="max-w-6xl mx-auto animate-fade-in">
        <div className="mb-8">
          <h1 className="text-4xl font-bold text-dark mb-2">Верификация навыков</h1>
          <p className="text-gray-600">
            Проверьте и верифицируйте навыки, извлечённые из вакансий
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

        {skills.length === 0 ? (
          <div className="bg-white rounded-xl shadow-card p-12 text-center">
            <div className="text-6xl mb-4">✨</div>
            <h2 className="text-2xl font-bold text-dark mb-2">Нет навыков на проверке</h2>
            <p className="text-gray-600">
              Загрузите вакансии и импортируйте навыки, чтобы начать верификацию
            </p>
          </div>
        ) : (
          <div className="space-y-4">
            <div className="mb-6">
              <div className="relative">
                <Search className="absolute left-4 top-3 text-gray-400" size={20} />
                <Input
                  placeholder="Поиск по названию или нормализованному имени..."
                  value={searchQuery}
                  onChange={(e) => setSearchQuery(e.target.value)}
                  className="pl-12"
                />
              </div>
            </div>

            <div className="text-sm text-gray-600 mb-4">
              Показано: <span className="font-bold">{filteredSkills.length}</span> из{' '}
              <span className="font-bold">{skills.length}</span>
            </div>

            {filteredSkills.map((skill) => (
              <div key={skill.id} className="bg-white rounded-lg shadow-card overflow-hidden">
                <div className="p-6 flex items-center justify-between hover:bg-gray-50 transition-colors">
                  <div className="flex-1">
                    <div className="flex items-center gap-3 mb-2">
                      <h3 className="text-lg font-semibold text-dark">{skill.originalName}</h3>
                      <span className="px-2 py-1 bg-blue-100 text-blue-700 text-xs rounded-full">
                        {skill.normalizedName}
                      </span>
                    </div>
                    <div className="text-sm text-gray-600">
                      Встречалось: <span className="font-bold">{skill.counter}</span> раз(а)
                    </div>
                  </div>

                  <div className="flex gap-2">
                    <Button
                      onClick={() => handleOpenModal(skill)}
                      disabled={isApproving || isRejecting}
                      size="sm"
                      className="gap-2"
                    >
                      {selectedSkill?.id === skill.id ? (
                        <>
                          <ChevronUp size={16} />
                          Скрыть
                        </>
                      ) : (
                        <>
                          <ChevronDown size={16} />
                          Развернуть
                        </>
                      )}
                    </Button>
                    <Button
                      onClick={() => handleReject(skill.id)}
                      disabled={isApproving || isRejecting}
                      size="sm"
                      variant="secondary"
                      className="gap-2"
                    >
                      <Trash2 size={16} />
                      Отклонить
                    </Button>
                  </div>
                </div>

                {selectedSkill?.id === skill.id && (
                  <div className="border-t border-gray-200 p-6 space-y-4 bg-gray-50 max-h-[60vh] overflow-y-auto">
                    {/* Mode Selection */}
                    <div className="bg-white p-4 rounded-lg space-y-3">
                      <label className="block text-sm font-medium text-dark mb-3">Выберите способ одобрения:</label>

                      <label className="flex items-center gap-3 p-3 border border-gray-200 rounded-lg cursor-pointer hover:bg-white transition-colors" style={{ borderColor: approvalMode === 'new' ? '#5CBBF1' : undefined, backgroundColor: approvalMode === 'new' ? '#E0F4FF' : undefined }}>
                        <input
                          type="radio"
                          name="mode"
                          value="new"
                          checked={approvalMode === 'new'}
                          onChange={() => {
                            setApprovalMode('new');
                            setSelectedExistingSkill(null);
                          }}
                          className="w-4 h-4"
                        />
                        <div>
                          <div className="font-medium text-dark">Создать новый навык</div>
                          <div className="text-xs text-gray-600">Создаст новый независимый навык в системе</div>
                        </div>
                      </label>

                      <label className="flex items-center gap-3 p-3 border border-gray-200 rounded-lg cursor-pointer hover:bg-white transition-colors" style={{ borderColor: approvalMode === 'synonym' ? '#5CBBF1' : undefined, backgroundColor: approvalMode === 'synonym' ? '#E0F4FF' : undefined }}>
                        <input
                          type="radio"
                          name="mode"
                          value="synonym"
                          checked={approvalMode === 'synonym'}
                          onChange={() => setApprovalMode('synonym')}
                          className="w-4 h-4"
                        />
                        <div>
                          <div className="font-medium text-dark">Добавить как синоним</div>
                          <div className="text-xs text-gray-600">Добавит к существующему навыку в качестве синонима</div>
                        </div>
                      </label>

                      <label className="flex items-center gap-3 p-3 border border-gray-200 rounded-lg cursor-pointer hover:bg-white transition-colors" style={{ borderColor: approvalMode === 'dependency' ? '#5CBBF1' : undefined, backgroundColor: approvalMode === 'dependency' ? '#E0F4FF' : undefined }}>
                        <input
                          type="radio"
                          name="mode"
                          value="dependency"
                          checked={approvalMode === 'dependency'}
                          onChange={() => setSelectedDependency(null)}
                          onClick={() => setApprovalMode('dependency')}
                          className="w-4 h-4"
                        />
                        <div>
                          <div className="font-medium text-dark">Создать навык с зависимостями</div>
                          <div className="text-xs text-gray-600">Создаст новый навык и свяжет его с другими</div>
                        </div>
                      </label>
                    </div>

                    {/* Mode-specific content */}
                    {approvalMode === 'new' || approvalMode === 'dependency' ? (
                      <div className="space-y-4 bg-white p-4 rounded-lg">
                        <div>
                          <label className="block text-sm font-medium text-dark mb-2">
                            Название навыка
                          </label>
                          <Input
                            value={displayName}
                            onChange={(e) => setDisplayName(e.target.value)}
                            placeholder="Например: C#"
                          />
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

                        {approvalMode === 'dependency' && (
                          <div>
                            <label className="block text-sm font-medium text-dark mb-2">
                              Выберите одну зависимость
                            </label>
                            <Input
                              value={dependencySearch}
                              onChange={(e) => setDependencySearch(e.target.value)}
                              placeholder="Поиск по названию или нормализованному имени..."
                            />
                            <div className="max-h-48 overflow-y-auto border border-gray-200 rounded-lg mt-2">
                              {isLoadingCleanSkills ? (
                                <div className="p-4 text-center text-gray-500 text-sm">Загружаю навыки...</div>
                              ) : cleanSkills
                                  .filter(skill =>
                                    skill.displayName.toLowerCase().includes(dependencySearch.toLowerCase()) ||
                                    skill.normalizedName.toLowerCase().includes(dependencySearch.toLowerCase())
                                  )
                                  .filter(skill => skill.normalizedName !== selectedSkill?.normalizedName)
                                  .length === 0 ? (
                                <div className="p-4 text-center text-gray-500 text-sm">Навыки не найдены</div>
                              ) : (
                                <div className="divide-y">
                                  {cleanSkills
                                    .filter(skill =>
                                      skill.displayName.toLowerCase().includes(dependencySearch.toLowerCase()) ||
                                      skill.normalizedName.toLowerCase().includes(dependencySearch.toLowerCase())
                                    )
                                    .filter(skill => skill.normalizedName !== selectedSkill?.normalizedName)
                                    .map((s) => (
                                      <label
                                        key={s.id}
                                        className="flex items-center gap-3 p-3 hover:bg-gray-50 transition-colors cursor-pointer"
                                      >
                                        <input
                                          type="radio"
                                          name="dependency"
                                          checked={selectedDependency === s.id}
                                          onChange={() => setSelectedDependency(s.id)}
                                          className="w-4 h-4"
                                        />
                                        <div className="flex-1">
                                          <div className="font-medium text-dark text-sm">{s.displayName}</div>
                                          <div className="text-xs text-gray-600">
                                            <span className="bg-gray-100 px-2 py-0.5 rounded inline-block">
                                              {s.normalizedName}
                                            </span>
                                          </div>
                                        </div>
                                      </label>
                                    ))}
                                </div>
                              )}
                            </div>
                            {selectedDependency && (
                              <div className="p-3 bg-blue-50 border border-blue-200 rounded-lg text-sm mt-2">
                                <div className="text-blue-700">
                                  ✓ Выбран родительский навык
                                </div>
                              </div>
                            )}
                          </div>
                        )}
                      </div>
                    ) : (
                      <div className="space-y-4 bg-white p-4 rounded-lg">
                        <div>
                          <label className="block text-sm font-medium text-dark mb-2">
                            Поиск существующего навыка
                          </label>
                          <Input
                            value={existingSkillSearch}
                            onChange={(e) => setExistingSkillSearch(e.target.value)}
                            placeholder="Поиск по названию или нормализованному имени..."
                          />
                        </div>

                        <div>
                          <label className="block text-sm font-medium text-dark mb-2">
                            Выберите навык из списка
                          </label>
                          <div className="max-h-64 overflow-y-auto border border-gray-200 rounded-lg">
                            {isLoadingCleanSkills ? (
                              <div className="p-4 text-center text-gray-500 text-sm">Загружаю навыки...</div>
                            ) : cleanSkills
                                .filter(s =>
                                  s.displayName.toLowerCase().includes(existingSkillSearch.toLowerCase()) ||
                                  s.normalizedName.toLowerCase().includes(existingSkillSearch.toLowerCase())
                                )
                                .length === 0 ? (
                              <div className="p-4 text-center text-gray-500 text-sm">Навыки не найдены</div>
                            ) : (
                              <div className="divide-y">
                                {cleanSkills
                                  .filter(s =>
                                    s.displayName.toLowerCase().includes(existingSkillSearch.toLowerCase()) ||
                                    s.normalizedName.toLowerCase().includes(existingSkillSearch.toLowerCase())
                                  )
                                  .map((s) => (
                                    <button
                                      key={s.id}
                                      onClick={() => setSelectedExistingSkill(s.id)}
                                      className={`w-full text-left p-3 hover:bg-gray-50 transition-colors ${
                                        selectedExistingSkill === s.id ? 'bg-primary bg-opacity-10 border-l-4 border-l-primary' : ''
                                      }`}
                                    >
                                      <p className="font-medium text-dark text-sm">{s.displayName}</p>
                                      <p className="text-xs text-gray-600">
                                        <span className="bg-gray-100 px-2 py-0.5 rounded inline-block mt-1">
                                          {s.normalizedName}
                                        </span>
                                      </p>
                                    </button>
                                  ))}
                              </div>
                            )}
                          </div>
                        </div>
                      </div>
                    )}

                    {/* Action Buttons */}
                    <div className="flex gap-3">
                      <Button
                        onClick={handleApprove}
                        disabled={
                          isApproving ||
                          (approvalMode === 'new' && !displayName.trim()) ||
                          (approvalMode === 'synonym' && !selectedExistingSkill) ||
                          (approvalMode === 'dependency' && (!displayName.trim() || !selectedDependency))
                        }
                        isLoading={isApproving}
                        className="flex-1"
                      >
                        <CheckCircle size={20} />
                        Одобрить
                      </Button>
                      <Button
                        onClick={handleCloseModal}
                        disabled={isApproving}
                        variant="secondary"
                        className="flex-1"
                      >
                        Отмена
                      </Button>
                    </div>
                  </div>
                )}
              </div>
            ))}
          </div>
        )}
      </div>
    </Layout>
  );
};
