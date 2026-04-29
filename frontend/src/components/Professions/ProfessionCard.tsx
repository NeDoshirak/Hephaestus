import { FC } from 'react';
import { Edit2, Trash2, Briefcase } from 'lucide-react';
import { Profession, ProfessionDirection } from '@/types/vacancy';

interface ProfessionCardProps {
  profession: Profession;
  onEdit: (profession: Profession) => void;
  onDelete: (id: string) => void;
  isDeleting?: boolean;
}

const DIRECTION_COLORS: Record<ProfessionDirection, { bg: string; text: string; border: string; icon: string }> = {
  [ProfessionDirection.Programming]: {
    bg: 'from-blue-600 to-blue-400',
    text: 'text-blue-700',
    border: 'border-blue-200',
    icon: 'bg-blue-100 text-blue-600',
  },
  [ProfessionDirection.Analytics]: {
    bg: 'from-orange-600 to-orange-400',
    text: 'text-orange-700',
    border: 'border-orange-200',
    icon: 'bg-orange-100 text-orange-600',
  },
  [ProfessionDirection.Testing]: {
    bg: 'from-green-600 to-green-400',
    text: 'text-green-700',
    border: 'border-green-200',
    icon: 'bg-green-100 text-green-600',
  },
  [ProfessionDirection.Design]: {
    bg: 'from-purple-600 to-purple-400',
    text: 'text-purple-700',
    border: 'border-purple-200',
    icon: 'bg-purple-100 text-purple-600',
  },
  [ProfessionDirection.DevOps]: {
    bg: 'from-red-600 to-red-400',
    text: 'text-red-700',
    border: 'border-red-200',
    icon: 'bg-red-100 text-red-600',
  },
  [ProfessionDirection.DataScience]: {
    bg: 'from-amber-600 to-amber-400',
    text: 'text-amber-700',
    border: 'border-amber-200',
    icon: 'bg-amber-100 text-amber-600',
  },
  [ProfessionDirection.Management]: {
    bg: 'from-cyan-600 to-cyan-400',
    text: 'text-cyan-700',
    border: 'border-cyan-200',
    icon: 'bg-cyan-100 text-cyan-600',
  },
};

export const ProfessionCard: FC<ProfessionCardProps> = ({
  profession,
  onEdit,
  onDelete,
  isDeleting,
}) => {
  const colors = DIRECTION_COLORS[profession.direction];

  return (
    <div className="bg-white rounded-xl shadow-md hover:shadow-xl transition-all duration-300 overflow-hidden group h-full flex flex-col">
      {/* Gradient Header */}
      <div className={`h-1 bg-gradient-to-r ${colors.bg}`}></div>

      <div className="p-6 flex-1 flex flex-col">
        {/* Icon and Title */}
        <div className="flex items-start gap-3 mb-4">
          <div className={`p-2 rounded-lg ${colors.icon} flex-shrink-0`}>
            <Briefcase size={20} />
          </div>
          <div className="flex-1 min-w-0">
            <h3 className="text-xl font-bold text-dark group-hover:text-primary transition-colors line-clamp-2">
              {profession.name}
            </h3>
            <p className={`text-sm font-semibold ${colors.text} mt-1`}>
              {profession.direction}
            </p>
          </div>
        </div>

        {/* Description */}
        {profession.description && (
          <p className="text-sm text-gray-600 mb-4 line-clamp-3 flex-grow">
            {profession.description}
          </p>
        )}

        {/* Footer - Metadata and Actions */}
        <div className="pt-4 border-t border-gray-100">
          <div className="flex items-center justify-between">
            <div className="text-xs text-gray-500">
              Создано:{' '}
              <span className="font-medium">
                {new Date(profession.createdAt).toLocaleDateString('ru-RU')}
              </span>
            </div>
            <div className="flex gap-2">
              <button
                onClick={() => onEdit(profession)}
                className="p-2 text-blue-600 hover:bg-blue-50 rounded-lg transition-colors"
                title="Редактировать"
              >
                <Edit2 size={18} />
              </button>
              <button
                onClick={() => onDelete(profession.id)}
                disabled={isDeleting}
                className="p-2 text-red-600 hover:bg-red-50 rounded-lg transition-colors disabled:opacity-50 disabled:cursor-not-allowed"
                title="Удалить"
              >
                <Trash2 size={18} />
              </button>
            </div>
          </div>
        </div>
      </div>
    </div>
  );
};
