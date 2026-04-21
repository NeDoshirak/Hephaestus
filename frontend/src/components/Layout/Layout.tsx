import { FC, ReactNode } from 'react';
import { Sidebar } from './Sidebar';

interface LayoutProps {
  children: ReactNode;
  logo?: string;
}

export const Layout: FC<LayoutProps> = ({ children, logo }) => {
  return (
    <div className="flex h-screen bg-light">
      <Sidebar logo={logo} />
      <main className="flex-1 overflow-auto">
        <div className="p-8">
          {children}
        </div>
      </main>
    </div>
  );
};
