import { FC, useState, useRef, useEffect } from 'react';
import { ChevronDown } from 'lucide-react';
import { Input } from './Input';

interface SkillFilterProps {
  value: string;
  onChange: (value: string) => void;
  skills: string[];
}

export const SkillFilter: FC<SkillFilterProps> = ({ value, onChange, skills }) => {
  const [isOpen, setIsOpen] = useState(false);
  const [searchTerm, setSearchTerm] = useState('');
  const ref = useRef<HTMLDivElement>(null);

  const filteredSkills = skills.filter(skill =>
    skill.toLowerCase().includes(searchTerm.toLowerCase())
  );

  // Close dropdown when clicking outside
  useEffect(() => {
    const handleClickOutside = (event: MouseEvent) => {
      if (ref.current && !ref.current.contains(event.target as Node)) {
        setIsOpen(false);
      }
    };

    document.addEventListener('mousedown', handleClickOutside);
    return () => document.removeEventListener('mousedown', handleClickOutside);
  }, []);

  return (
    <div className="relative w-full max-w-xs" ref={ref}>
      <button
        onClick={() => setIsOpen(!isOpen)}
        className="w-full px-4 py-2 rounded-lg border border-gray-300 bg-white text-gray-700 focus:outline-none focus:border-primary flex items-center justify-between hover:bg-gray-50"
      >
        <span className="truncate">
          {value ? value : 'Все навыки'}
        </span>
        <ChevronDown size={18} className={`transition-transform ${isOpen ? 'rotate-180' : ''}`} />
      </button>

      {isOpen && (
        <div className="absolute top-full left-0 right-0 mt-1 bg-white border border-gray-300 rounded-lg shadow-lg z-10">
          <div className="p-2 border-b border-gray-200">
            <Input
              placeholder="Поиск навыков..."
              value={searchTerm}
              onChange={(e) => setSearchTerm(e.target.value)}
              className="w-full"
            />
          </div>
          <div className="max-h-64 overflow-y-auto">
            <button
              onClick={() => {
                onChange('');
                setIsOpen(false);
                setSearchTerm('');
              }}
              className={`w-full text-left px-4 py-2 hover:bg-blue-50 transition-colors ${
                !value ? 'bg-primary bg-opacity-10 text-primary font-medium' : ''
              }`}
            >
              Все навыки
            </button>
            {filteredSkills.length > 0 ? (
              filteredSkills.map((skill) => (
                <button
                  key={skill}
                  onClick={() => {
                    onChange(skill);
                    setIsOpen(false);
                    setSearchTerm('');
                  }}
                  className={`w-full text-left px-4 py-2 hover:bg-blue-50 transition-colors ${
                    value === skill ? 'bg-primary bg-opacity-10 text-primary font-medium' : ''
                  }`}
                >
                  {skill}
                </button>
              ))
            ) : (
              <div className="px-4 py-2 text-gray-500 text-sm">
                Навыки не найдены
              </div>
            )}
          </div>
        </div>
      )}
    </div>
  );
};
