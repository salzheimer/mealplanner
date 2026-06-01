import clsx from 'clsx'
import type { MealType, ItemType, ItemStatus } from '../../types/shared'

type BadgeVariant = MealType | ItemType | ItemStatus | 'default'

interface BadgeProps {
  label: string
  variant?: BadgeVariant
  className?: string
}

const variantClasses: Record<BadgeVariant, string> = {
  // MealType
  Breakfast: 'bg-yellow-100 text-yellow-800',
  Lunch: 'bg-green-100 text-green-800',
  Dinner: 'bg-blue-100 text-blue-800',
  Snack: 'bg-purple-100 text-purple-800',
  // ItemType
  Recipe: 'bg-indigo-100 text-indigo-800',
  Homemade: 'bg-teal-100 text-teal-800',
  StoreBought: 'bg-orange-100 text-orange-800',
  // ItemStatus
  Unknown: 'bg-gray-100 text-gray-600',
  Pending: 'bg-amber-100 text-amber-800',
  Confirmed: 'bg-emerald-100 text-emerald-800',
  // default
  default: 'bg-gray-100 text-gray-700',
}

export default function Badge({ label, variant = 'default', className }: BadgeProps) {
  return (
    <span
      className={clsx(
        'inline-flex items-center rounded-full px-2.5 py-0.5 text-xs font-medium',
        variantClasses[variant],
        className,
      )}
    >
      {label}
    </span>
  )
}
