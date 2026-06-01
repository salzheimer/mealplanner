import { useState } from 'react'
import { Link } from 'react-router-dom'
import { useMeals } from '../../hooks/useMeals'
import Button from '../../components/ui/Button'
import EmptyState from '../../components/ui/EmptyState'
import LoadingSpinner from '../../components/ui/LoadingSpinner'
import PageHeader from '../../components/layout/PageHeader'
import MealCard from './MealCard'
import type { MealType } from '../../types/shared'

const MEAL_TYPES: MealType[] = ['Breakfast', 'Lunch', 'Dinner', 'Snack']

export default function MealListPage() {
  const { data: meals, isLoading, error } = useMeals()
  const [activeFilter, setActiveFilter] = useState<MealType | null>(null)

  const filtered = activeFilter ? meals?.filter(m => m.mealType === activeFilter) : meals

  if (isLoading) {
    return (
      <div className="flex justify-center py-16">
        <LoadingSpinner size="lg" />
      </div>
    )
  }

  if (error) {
    return <p className="text-red-600">Failed to load meals. Please try again.</p>
  }

  return (
    <div>
      <PageHeader
        title="Meals"
        action={
          <Link to="/meals/new">
            <Button>New meal</Button>
          </Link>
        }
      />

      <div className="mt-4 flex flex-wrap gap-2">
        <button
          onClick={() => setActiveFilter(null)}
          className={`rounded-full px-3 py-1 text-sm font-medium transition-colors ${
            activeFilter === null
              ? 'bg-blue-600 text-white'
              : 'bg-gray-100 text-gray-700 hover:bg-gray-200'
          }`}
        >
          All
        </button>
        {MEAL_TYPES.map(type => (
          <button
            key={type}
            onClick={() => setActiveFilter(activeFilter === type ? null : type)}
            className={`rounded-full px-3 py-1 text-sm font-medium transition-colors ${
              activeFilter === type
                ? 'bg-blue-600 text-white'
                : 'bg-gray-100 text-gray-700 hover:bg-gray-200'
            }`}
          >
            {type}
          </button>
        ))}
      </div>

      {filtered && filtered.length === 0 ? (
        <EmptyState
          heading={activeFilter ? `No ${activeFilter} meals` : 'No meals yet'}
          description={activeFilter ? undefined : 'Create your first meal to start planning.'}
          action={
            !activeFilter ? (
              <Link to="/meals/new">
                <Button>Create meal</Button>
              </Link>
            ) : undefined
          }
        />
      ) : (
        <div className="mt-6 grid gap-4 sm:grid-cols-2 lg:grid-cols-3">
          {filtered?.map(meal => (
            <MealCard key={meal.id} meal={meal} />
          ))}
        </div>
      )}
    </div>
  )
}
