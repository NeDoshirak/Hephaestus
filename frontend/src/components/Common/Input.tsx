import { FC, InputHTMLAttributes } from 'react';
import clsx from 'clsx';

interface InputProps extends InputHTMLAttributes<HTMLInputElement> {
  label?: string;
  error?: string;
}

export const Input: FC<InputProps> = ({
  label,
  error,
  className,
  ...props
}) => {
  return (
    <div className="w-full">
      {label && (
        <label className="block text-sm font-medium text-dark mb-2">
          {label}
        </label>
      )}
      <input
        className={clsx(
          'w-full px-4 py-2 rounded-lg border transition-all duration-200',
          'text-dark placeholder-gray-400',
          'focus:outline-none focus:ring-2 focus:ring-primary focus:border-transparent',
          error ? 'border-red-500 focus:ring-red-500' : 'border-gray-200 hover:border-gray-300',
          className
        )}
        {...props}
      />
      {error && <p className="text-red-500 text-sm mt-1">{error}</p>}
    </div>
  );
};
