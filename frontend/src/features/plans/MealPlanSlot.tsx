import { useState } from 'react'
import { useDeleteMealPlan } from '../../hooks/useMealPlans'
import { useMeals } from '../../hooks/useMeals'
import Button from '../../components/ui/Button'
import ConfirmDialog from '../../components/ui/ConfirmDialog'
import AddMealToPlanModal from './AddMealToPlanModal'
import type { MealPlan } from '../../types/plan'

interface MealPlanSlotProps {
  date: string
  dateLabel: string
  mealPlans: MealPlan[]
  planId: number
  isToday: boolean
  canEdit: boolean
}

export default function MealPlanSlot({ date, dateLabel, mealPlans, planId, isToday, canEdit }: MealPlanSlotProps) {
  const { data: meals = [] } = useMeals()
  const deleteMealPlan = useDeleteMealPlan()

  const [addOpen, setAddOpen] = useState(false)
  const [confirmDeleteId, setConfirmDeleteId] = useState<number | null>(null)

  const getMealName = (mealId: number) => meals.find(m => m.id === mealId)?.name ?? `Meal #${mealId}`

  return (
    <div
      className={`rounded-lg border p-3 ${
        isToday ? 'border-blue-400 bg-blue-50' : 'border-gray-200 bg-white'
      }`}
    >
      <div className="mb-2 flex items-center justify-between">
        <span className={`text-xs font-semibold ${isToday ? 'text-blue-700' : 'text-gray-500'}`}>
          {dateLabel}
        </span>
        {canEdit && (
          <Button variant="ghost" size="sm" onClick={() => setAddOpen(true)}>
            +
          </Button>
        )}
      </div>

      {mealPlans.length === 0 ? (
        <p className="text-xs text-gray-400 italic">No meals</p>
      ) : (
        <ul className="space-y-1">
          {mealPlans.map(mp => (
            <li
              key={mp.id}
              className="group flex items-center justify-between gap-1 rounded bg-white px-2 py-1 text-xs shadow-sm"
            >
              <span className="truncate text-gray-800">{getMealName(mp.mealId)}</span>
              {canEdit && (
                <button
                  type="button"
                  onClick={() => setConfirmDeleteId(mp.id)}
                  aria-label={`Remove ${getMealName(mp.mealId)}`}
                  className="hidden shrink-0 text-gray-400 hover:text-red-500 group-hover:block"
                >
                  ×
                </button>
              )}
            </li>
          ))}
        </ul>
      )}

      <AddMealToPlanModal
        isOpen={addOpen}
        onClose={() => setAddOpen(false)}
        planId={planId}
        defaultServeDate={date}
      />

      <ConfirmDialog
        isOpen={confirmDeleteId !== null}
        onClose={() => setConfirmDeleteId(null)}
        onConfirm={() => {
          if (confirmDeleteId != null) {
            deleteMealPlan.mutate(confirmDeleteId)
            setConfirmDeleteId(null)
          }
        }}
        title="Remove meal"
        message="Remove this meal from the plan for this day?"
        confirmLabel="Remove"
        isDestructive
        isLoading={deleteMealPlan.isPending}
      />
    </div>
  )
}
