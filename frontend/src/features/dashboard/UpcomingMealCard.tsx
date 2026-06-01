import { Link } from 'react-router-dom'
import Card from '../../components/ui/Card'
import Badge from '../../components/ui/Badge'
import type { MealPlan } from '../../types/plan'
import type { Meal } from '../../types/meal'
import { formatShortDate, isToday } from '../../utils/dateUtils'

interface UpcomingMealCardProps {
  mealPlan: MealPlan
  meal: Meal | undefined
}

export default function UpcomingMealCard({ mealPlan, meal }: UpcomingMealCardProps) {
  const dateLabel = mealPlan.serveDate
    ? isToday(mealPlan.serveDate)
      ? 'Today'
      : formatShortDate(mealPlan.serveDate)
    : 'Unscheduled'

  return (
    <Card padding="sm" className="flex items-center gap-3">
      <div className="flex w-20 shrink-0 flex-col items-center justify-center rounded-md bg-blue-50 py-2 text-center">
        <span className="text-xs font-medium text-blue-600">{dateLabel}</span>
      </div>
      <div className="flex-1 min-w-0">
        <Link to={`/meals/${mealPlan.mealId}`} className="font-medium text-gray-900 hover:text-blue-600 truncate block">
          {meal?.name ?? `Meal #${mealPlan.mealId}`}
        </Link>
        {meal && (
          <div className="mt-0.5 flex items-center gap-2">
            <Badge label={meal.mealType} variant={meal.mealType} />
          </div>
        )}
      </div>
      <Link to={`/plans/${mealPlan.planId}`} className="shrink-0 text-xs text-gray-400 hover:text-gray-600">
        View plan →
      </Link>
    </Card>
  )
}
