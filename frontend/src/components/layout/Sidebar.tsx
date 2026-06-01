import { NavLink } from 'react-router-dom'
import clsx from 'clsx'

const navItems = [
  { to: '/dashboard', label: 'Dashboard' },
  { to: '/recipes', label: 'Recipes' },
  { to: '/meals', label: 'Meals' },
  { to: '/plans', label: 'Plans' },
]

export default function Sidebar() {
  return (
    <aside className="flex h-full w-56 flex-col border-r border-gray-200 bg-white">
      <div className="flex h-14 items-center px-4 font-semibold text-gray-900">
        Meal Planner
      </div>
      <nav className="flex-1 space-y-1 px-2 py-2">
        {navItems.map(({ to, label }) => (
          <NavLink
            key={to}
            to={to}
            className={({ isActive }) =>
              clsx(
                'block rounded-md px-3 py-2 text-sm font-medium transition-colors',
                isActive
                  ? 'bg-blue-50 text-blue-700'
                  : 'text-gray-700 hover:bg-gray-100 hover:text-gray-900',
              )
            }
          >
            {label}
          </NavLink>
        ))}
      </nav>
    </aside>
  )
}
