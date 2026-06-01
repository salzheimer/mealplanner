import { useMealPlansByDateRange } from '../../hooks/useMealPlans'
import { useMeals } from '../../hooks/useMeals'
import { useAuth } from '../../contexts/AuthContext'
import LoadingSpinner from '../../components/ui/LoadingSpinner'
import EmptyState from '../../components/ui/EmptyState'
import UpcomingMealCard from './UpcomingMealCard'
import { getCurrentWeekRange, formatDate } from '../../utils/dateUtils'
import { Link } from 'react-router-dom'
import Button from '../../components/ui/Button'

const { startDate, endDate } = getCurrentWeekRange()

export default function DashboardPage() {
  const { user } = useAuth()
  const { data: mealPlans = [], isLoading: isLoadingPlans } = useMealPlansByDateRange(startDate, endDate)
  const { data: meals = [] } = useMeals()

  const sorted = [...mealPlans].sort((a, b) => {
    if (!a.serveDate) return 1
    if (!b.serveDate) return -1
    return a.serveDate.localeCompare(b.serveDate)
  })

  const getMeal = (mealId: number) => meals.find(m => m.id === mealId)

  return (
    <div className="max-w-2xl">
      <div className="mb-6">
        <h1 className="text-2xl font-bold text-gray-900">
          {user?.displayName ? `Hello, ${user.displayName}` : 'Dashboard'}
        </h1>
        <p className="mt-1 text-sm text-gray-500">
          Week of {formatDate(startDate)} – {formatDate(endDate)}
        </p>
      </div>

      <div className="mb-2 flex items-center justify-between">
        <h2 className="text-lg font-semibold text-gray-900">This week's meals</h2>
        <Link to="/plans">
          <Button variant="ghost" size="sm">View all plans</Button>
        </Link>
      </div>

      {isLoadingPlans ? (
        <div className="flex justify-center py-8">
          <LoadingSpinner />
        </div>
      ) : sorted.length === 0 ? (
        <EmptyState
          heading="No meals scheduled this week"
          description="Add meals to a plan to see them here."
          action={
            <Link to="/plans/new">
              <Button>Create a plan</Button>
            </Link>
          }
        />
      ) : (
        <div className="space-y-3">
          {sorted.map(mp => (
            <UpcomingMealCard key={mp.id} mealPlan={mp} meal={getMeal(mp.mealId)} />
          ))}
        </div>
      )}
    </div>
  )
}
