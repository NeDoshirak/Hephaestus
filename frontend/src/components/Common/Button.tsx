import { FC, ReactNode, ButtonHTMLAttributes } from 'react';
import clsx from 'clsx';

interface ButtonProps extends ButtonHTMLAttributes<HTMLButtonElement> {
  variant?: 'primary' | 'secondary' | 'danger' | 'outline';
  size?: 'sm' | 'md' | 'lg';
  isLoading?: boolean;
  children: ReactNode;
}

export const Button: FC<ButtonProps> = ({
  variant = 'primary',
  size = 'md',
  isLoading,
  className,
  children,
  disabled,
  ...props
}) => {
  const baseClasses = 'font-medium rounded-lg transition-all duration-200 flex items-center justify-center gap-2';

  const variantClasses = {
    primary: 'bg-primary hover:bg-blue-600 text-white disabled:bg-gray-400',
    secondary: 'bg-gray-200 hover:bg-gray-300 text-dark disabled:bg-gray-400',
    danger: 'bg-red-500 hover:bg-red-600 text-white disabled:bg-red-300',
    outline: 'border border-primary text-primary hover:bg-primary hover:text-white disabled:bg-gray-400',
  };

  const sizeClasses = {
    sm: 'px-3 py-2 text-sm',
    md: 'px-4 py-2 text-base',
    lg: 'px-6 py-3 text-lg',
  };

  return (
    <button
      className={clsx(
        baseClasses,
        variantClasses[variant],
        sizeClasses[size],
        className
      )}
      disabled={disabled || isLoading}
      {...props}
    >
      {isLoading && <span className="animate-spin">⏳</span>}
      {children}
    </button>
  );
};
