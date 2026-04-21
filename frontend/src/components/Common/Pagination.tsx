import { FC } from 'react';
import { ChevronLeft, ChevronRight } from 'lucide-react';

interface PaginationProps {
  currentPage: number;
  totalPages: number;
  onPageChange: (page: number) => void;
}

export const Pagination: FC<PaginationProps> = ({
  currentPage,
  totalPages,
  onPageChange,
}) => {
  const handlePrevious = () => {
    if (currentPage > 1) onPageChange(currentPage - 1);
  };

  const handleNext = () => {
    if (currentPage < totalPages) onPageChange(currentPage + 1);
  };

  // Ограничиваем currentPage максимумом totalPages
  const safePage = Math.min(currentPage, totalPages);

  const pageNumbers = Array.from(
    { length: Math.min(totalPages, 5) },
    (_, i) => {
      const startPage = Math.max(1, safePage - 2);
      return Math.min(startPage + i, totalPages);
    }
  ).filter((page, idx, arr) => idx === 0 || arr[idx - 1] !== page); // Убираем дубли

  return (
    <div className="flex items-center justify-center gap-2 mt-8">
      <button
        onClick={handlePrevious}
        disabled={safePage === 1}
        className="p-2 rounded-lg hover:bg-gray-200 disabled:opacity-50 disabled:cursor-not-allowed transition-colors"
      >
        <ChevronLeft size={20} />
      </button>

      {pageNumbers.map((page) => (
        <button
          key={page}
          onClick={() => onPageChange(page)}
          className={`w-10 h-10 rounded-lg font-medium transition-all ${
            page === safePage
              ? 'bg-primary text-white'
              : 'bg-white text-dark hover:bg-gray-200'
          }`}
        >
          {page}
        </button>
      ))}

      <button
        onClick={handleNext}
        disabled={safePage === totalPages}
        className="p-2 rounded-lg hover:bg-gray-200 disabled:opacity-50 disabled:cursor-not-allowed transition-colors"
      >
        <ChevronRight size={20} />
      </button>

      <span className="ml-4 text-sm text-gray-600">
        Страница {safePage} из {totalPages}
      </span>
    </div>
  );
};
