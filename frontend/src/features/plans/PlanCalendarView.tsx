import { useState } from 'react'
import { addWeeks, subWeeks, startOfWeek, endOfWeek, eachDayOfInterval, format } from 'date-fns'
import { useMealPlansByDateRange } from '../../hooks/useMealPlans'
import Button from '../../components/ui/Button'
import LoadingSpinner from '../../components/ui/LoadingSpinner'
import MealPlanSlot from './MealPlanSlot'
import { isToday } from '../../utils/dateUtils'
import type { MealPlan } from '../../types/plan'

interface PlanCalendarViewProps {
  planId: number
  allMealPlans?: MealPlan[]
  canEdit: boolean
}

function getWeekRange(base: Date) {
  const start = startOfWeek(base, { weekStartsOn: 1 })
  const end = endOfWeek(base, { weekStartsOn: 1 })
  return { start, end, startStr: format(start, 'yyyy-MM-dd'), endStr: format(end, 'yyyy-MM-dd') }
}

export default function PlanCalendarView({ planId, allMealPlans, canEdit }: PlanCalendarViewProps) {
  const [baseDate, setBaseDate] = useState(() => new Date())
  const { start, end, startStr, endStr } = getWeekRange(baseDate)

  const { data: weekMealPlans, isLoading } = useMealPlansByDateRange(startStr, endStr)

  const days = eachDayOfInterval({ start, end })

  const mealPlansForDate = (dateStr: string): MealPlan[] => {
    const source = weekMealPlans ?? allMealPlans ?? []
    return source.filter(mp => {
      if (mp.serveDate === dateStr) return true
      if (mp.planId !== planId) return false
      return false
    })
  }

  return (
    <div>
      <div className="mb-4 flex items-center gap-3">
        <Button variant="secondary" size="sm" onClick={() => setBaseDate(d => subWeeks(d, 1))}>
          ← Prev
        </Button>
        <span className="text-sm font-medium text-gray-700">
          {format(start, 'MMM d')} – {format(end, 'MMM d, yyyy')}
        </span>
        <Button variant="secondary" size="sm" onClick={() => setBaseDate(d => addWeeks(d, 1))}>
          Next →
        </Button>
        <Button variant="ghost" size="sm" onClick={() => setBaseDate(new Date())}>
          Today
        </Button>
      </div>

      {isLoading ? (
        <div className="flex justify-center py-8">
          <LoadingSpinner />
        </div>
      ) : (
        <div className="grid grid-cols-7 gap-2">
          {days.map(day => {
            const dateStr = format(day, 'yyyy-MM-dd')
            const dayMealPlans = mealPlansForDate(dateStr)
            return (
              <MealPlanSlot
                key={dateStr}
                date={dateStr}
                dateLabel={format(day, 'EEE d')}
                mealPlans={dayMealPlans}
                planId={planId}
                isToday={isToday(dateStr)}
                canEdit={canEdit}
              />
            )
          })}
        </div>
      )}
    </div>
  )
}
