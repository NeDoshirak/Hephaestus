import { FC } from 'react';
import { NavLink } from 'react-router-dom';
import { Home, List, CheckCircle, Zap, Globe } from 'lucide-react';

interface SidebarProps {
  logo?: string;
}

export const Sidebar: FC<SidebarProps> = ({ logo }) => {
  const navItems = [
    { path: '/', label: 'Главная', icon: Home },
    { path: '/vacancies', label: 'Вакансии', icon: List },
    { path: '/skills-review', label: 'Верификация навыков', icon: CheckCircle },
    { path: '/skills', label: 'Верифицированные навыки', icon: Zap },
    { path: '/hh-search', label: 'Поиск на HH', icon: Globe },
  ];

  return (
    <aside className="w-64 bg-dark text-white h-screen flex flex-col shadow-lg">
      {/* Logo Section */}
      <div className="p-6 border-b border-opacity-20 border-white">
        <div className="flex items-center gap-3">
          {logo && (
            <img
              src={logo}
              alt="Hephaestus"
              className="w-10 h-10 rounded-lg shadow-md"
            />
          )}
          <h1 className="text-xl font-bold tracking-tight">Hephaestus</h1>
        </div>
      </div>

      {/* Navigation */}
      <nav className="flex-1 p-4 space-y-1.5">
        {navItems.map((item) => {
          const Icon = item.icon;
          return (
            <NavLink
              key={item.path}
              to={item.path}
              className={({ isActive }) =>
                `flex items-center gap-3 px-4 py-3.5 rounded-lg transition-all duration-200 font-medium ${
                  isActive
                    ? 'bg-primary text-dark shadow-md'
                    : 'text-gray-200 hover:bg-gray-700 hover:text-white'
                }`
              }
            >
              <Icon size={20} className="flex-shrink-0" />
              <span className="text-sm">{item.label}</span>
            </NavLink>
          );
        })}
      </nav>

      {/* Footer */}
      <div className="p-4 border-t border-opacity-20 border-white">
        <p className="text-xs text-gray-400 text-center">
          v1.0.0
        </p>
      </div>
    </aside>
  );
};
