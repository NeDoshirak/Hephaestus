import { FC, useState } from 'react';
import { Trash2, Edit2, ExternalLink, Plus, Minus } from 'lucide-react';
import { RawVacancy } from '@/types/vacancy';
import { stripHtmlTags, truncateText } from '@/utils/html';
import { Drawer } from '@/components/Common/Drawer';

interface VacancyCardProps {
  vacancy: RawVacancy;
  onEdit: (vacancy: RawVacancy) => void;
  onDelete: (id: string) => void;
  isDeleting?: boolean;
}

export const VacancyCard: FC<VacancyCardProps> = ({
  vacancy,
  onEdit,
  onDelete,
  isDeleting,
}) => {
  const [isDescriptionModalOpen, setIsDescriptionModalOpen] = useState(false);
  const [areSkillsExpanded, setAreSkillsExpanded] = useState(false);

  const cleanedDescription = stripHtmlTags(vacancy.vacancyDescription);
  const descriptionPreview = truncateText(cleanedDescription, 150);
  const showDescriptionToggle = cleanedDescription.length > 150;

  const visibleSkills = (areSkillsExpanded ? vacancy.keySkills : vacancy.keySkills?.slice(0, 5)) ?? [];
  const hiddenSkillsCount = (vacancy.keySkills?.length ?? 0) - 5;

  return (
    <div className="bg-white rounded-xl shadow-card hover:shadow-card-hover transition-all duration-300 overflow-hidden group animate-slide-up flex flex-col h-full">
      {/* Header with gradient accent */}
      <div className="h-1 bg-gradient-to-r from-primary to-blue-500"></div>

      <div className="p-5 flex-1 flex flex-col overflow-hidden">
        <div className="flex flex-col h-full gap-4">
          {/* Title */}
          <h3 className="text-lg font-semibold text-dark line-clamp-2">
            {vacancy.vacancyName}
          </h3>

          {/* Description and Skills - scrollable content area */}
          <div className="flex-1 overflow-y-auto min-h-0">
            {/* Description */}
            <div className="mb-4">
              <div className="text-gray-600 text-sm prose prose-sm max-w-none">
                <p>{descriptionPreview}</p>
              </div>
              {showDescriptionToggle && (
                <button
                  onClick={() => setIsDescriptionModalOpen(true)}
                  className="mt-2 text-primary hover:text-blue-600 text-sm font-medium transition-colors"
                >
                  Читать полностью →
                </button>
              )}
            </div>

            {/* Skills */}
            {vacancy.keySkills && vacancy.keySkills.length > 0 && (
              <div className="mb-4">
                <div className="flex flex-wrap gap-2">
                  {visibleSkills.map((skill, idx) => (
                    <span
                      key={skill.id || idx}
                      className="inline-block px-3 py-1 bg-primary bg-opacity-10 text-primary text-xs font-medium rounded-full hover:bg-opacity-20 transition-colors cursor-default"
                    >
                      {skill.name || 'Навык'}
                    </span>
                  ))}
                  {hiddenSkillsCount > 0 && !areSkillsExpanded && (
                    <button
                      onClick={() => setAreSkillsExpanded(true)}
                      className="inline-flex items-center justify-center px-2 py-1 bg-primary bg-opacity-10 text-primary text-xs font-medium rounded-full hover:bg-opacity-20 transition-colors"
                      title={`Показать ещё ${hiddenSkillsCount} навыков`}
                    >
                      <Plus size={14} />
                    </button>
                  )}
                  {areSkillsExpanded && hiddenSkillsCount > 0 && (
                    <button
                      onClick={() => setAreSkillsExpanded(false)}
                      className="inline-flex items-center justify-center px-2 py-1 bg-primary bg-opacity-10 text-primary text-xs font-medium rounded-full hover:bg-opacity-20 transition-colors"
                      title="Скрыть"
                    >
                      <Minus size={14} />
                    </button>
                  )}
                </div>
              </div>
            )}

            {/* URL */}
            {vacancy.url && (
              <div className="mb-4">
                <a
                  href={vacancy.url}
                  target="_blank"
                  rel="noopener noreferrer"
                  className="flex items-center gap-1 text-primary hover:text-blue-600 text-sm font-medium transition-colors"
                >
                  <ExternalLink size={16} />
                  Открыть вакансию
                </a>
              </div>
            )}
          </div>

          {/* Status and Actions - pinned to bottom */}
          <div className="pt-4 border-t border-gray-200 space-y-3">
            {/* Status */}
            <div className="flex items-center">
              <span className="text-xs font-medium">
                Статус:{' '}
                <span
                  className={`${
                    vacancy.isProcessed
                      ? 'text-green-600 font-semibold'
                      : 'text-gray-500'
                  }`}
                >
                  {vacancy.isProcessed ? 'Обработана' : 'Не обработана'}
                </span>
              </span>
            </div>

            {/* Actions */}
            <div className="flex gap-2">
              <button
                onClick={() => onEdit(vacancy)}
                className="flex-1 flex items-center justify-center gap-2 px-3 py-2 bg-primary hover:bg-blue-600 text-white rounded-lg transition-colors font-medium text-sm"
              >
                <Edit2 size={16} />
                Редактировать
              </button>
              <button
                onClick={() => onDelete(vacancy.id)}
                disabled={isDeleting}
                className="flex-1 flex items-center justify-center gap-2 px-3 py-2 bg-red-500 hover:bg-red-600 disabled:bg-red-300 text-white rounded-lg transition-colors font-medium text-sm"
              >
                <Trash2 size={16} />
                {isDeleting ? 'Удаляю...' : 'Удалить'}
              </button>
            </div>
          </div>
        </div>
      </div>

      {/* Description Drawer */}
      <Drawer
        isOpen={isDescriptionModalOpen}
        onClose={() => setIsDescriptionModalOpen(false)}
        title={vacancy.vacancyName}
      >
        <div>
          <h3 className="text-sm font-semibold text-gray-600 mb-3 uppercase tracking-wide">
            Описание вакансии
          </h3>
          <div className="prose prose-sm max-w-none">
            <div
              className="text-gray-700 prose-p:m-0 prose-p:mb-3 prose-ul:my-2 prose-li:my-0 prose-strong:font-semibold prose-em:italic prose-a:text-primary hover:prose-a:text-blue-600"
              dangerouslySetInnerHTML={{
                __html: vacancy.vacancyDescription,
              }}
            />
          </div>
        </div>

        {/* Skills in drawer */}
        {vacancy.keySkills && vacancy.keySkills.length > 0 && (
          <div>
            <h3 className="text-sm font-semibold text-gray-600 mb-3 uppercase tracking-wide">
              Требуемые навыки ({vacancy.keySkills.length})
            </h3>
            <div className="flex flex-wrap gap-2">
              {vacancy.keySkills.map((skill, idx) => (
                <span
                  key={skill.id || idx}
                  className="px-3 py-2 bg-gradient-to-r from-primary to-blue-400 text-white text-xs font-medium rounded-full shadow-sm hover:shadow-md transition-shadow"
                >
                  {skill.name || 'Навык'}
                </span>
              ))}
            </div>
          </div>
        )}

        {/* URL in drawer */}
        {vacancy.url && (
          <div className="pt-4">
            <a
              href={vacancy.url}
              target="_blank"
              rel="noopener noreferrer"
              className="flex items-center gap-2 px-4 py-3 bg-primary hover:bg-blue-600 text-white rounded-lg font-medium transition-colors w-full justify-center"
            >
              <ExternalLink size={18} />
              Открыть на HeadHunter
            </a>
          </div>
        )}
      </Drawer>
    </div>
  );
};
