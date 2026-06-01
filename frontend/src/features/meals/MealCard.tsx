import { Link } from 'react-router-dom'
import Card from '../../components/ui/Card'
import Badge from '../../components/ui/Badge'
import type { Meal } from '../../types/meal'

interface MealCardProps {
  meal: Meal
}

export default function MealCard({ meal }: MealCardProps) {
  return (
    <Card className="hover:shadow-md transition-shadow">
      <Link to={`/meals/${meal.id}`} className="block">
        <div className="flex items-start justify-between gap-2">
          <h3 className="font-semibold text-gray-900">{meal.name ?? 'Untitled meal'}</h3>
          <Badge label={meal.mealType} variant={meal.mealType} />
        </div>
        {meal.description && (
          <p className="mt-1 text-sm text-gray-500 line-clamp-2">{meal.description}</p>
        )}
        {meal.isMultiDayMeal && (
          <p className="mt-2 text-xs text-blue-600">Multi-day meal</p>
        )}
      </Link>
    </Card>
  )
}
