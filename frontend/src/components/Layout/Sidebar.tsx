import { FC } from 'react';
import { NavLink } from 'react-router-dom';
import { List, Download } from 'lucide-react';

interface SidebarProps {
  logo?: string;
}

const HhIcon = () => (
  <span className="font-bold text-lg">HH</span>
);

export const Sidebar: FC<SidebarProps> = ({ logo }) => {
  const navItems = [
    { path: '/vacancies', label: 'Вакансии', icon: List },
    { path: '/hh-search', label: 'Поиск на HH', icon: HhIcon },
    { path: '/import', label: 'Загрузить с HH', icon: Download },
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
      <nav className="flex-1 p-4 space-y-2">
        {navItems.map((item) => {
          const Icon = item.icon;
          return (
            <NavLink
              key={item.path}
              to={item.path}
              className={({ isActive }) =>
                `flex items-center gap-3 px-4 py-3 rounded-lg transition-all duration-200 ${
                  isActive
                    ? 'bg-primary text-dark font-semibold shadow-md'
                    : 'text-gray-300 hover:bg-gray-700 hover:text-white'
                }`
              }
            >
              <Icon size={20} />
              <span>{item.label}</span>
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
