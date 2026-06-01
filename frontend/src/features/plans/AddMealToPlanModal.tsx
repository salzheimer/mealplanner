import { useState } from 'react'
import Modal from '../../components/ui/Modal'
import Button from '../../components/ui/Button'
import Select from '../../components/ui/Select'
import Input from '../../components/ui/Input'
import { useMeals } from '../../hooks/useMeals'
import { useCreateMealPlan } from '../../hooks/useMealPlans'

interface AddMealToPlanModalProps {
  isOpen: boolean
  onClose: () => void
  planId: number
  defaultServeDate?: string
}

export default function AddMealToPlanModal({ isOpen, onClose, planId, defaultServeDate }: AddMealToPlanModalProps) {
  const { data: meals = [] } = useMeals()
  const createMealPlan = useCreateMealPlan()

  const [selectedMealId, setSelectedMealId] = useState('')
  const [serveDate, setServeDate] = useState(defaultServeDate ?? '')
  const [error, setError] = useState<string | null>(null)

  const mealOptions = meals.map(m => ({ value: String(m.id), label: m.name ?? 'Untitled meal' }))

  async function handleSubmit() {
    if (!selectedMealId) {
      setError('Please select a meal.')
      return
    }
    setError(null)
    await createMealPlan.mutateAsync({
      mealId: Number(selectedMealId),
      planId,
      serveDate: serveDate || undefined,
    })
    setSelectedMealId('')
    setServeDate(defaultServeDate ?? '')
    onClose()
  }

  return (
    <Modal isOpen={isOpen} onClose={onClose} title="Add meal to plan" size="sm">
      <div className="space-y-4">
        <Select
          label="Meal *"
          options={mealOptions}
          placeholder="Select a meal"
          value={selectedMealId}
          onChange={e => setSelectedMealId(e.target.value)}
          error={error ?? undefined}
        />

        <Input
          label="Serve date"
          type="date"
          value={serveDate}
          onChange={e => setServeDate(e.target.value)}
        />

        <div className="flex justify-end gap-3">
          <Button variant="secondary" onClick={onClose}>
            Cancel
          </Button>
          <Button onClick={handleSubmit} isLoading={createMealPlan.isPending}>
            Add to plan
          </Button>
        </div>
      </div>
    </Modal>
  )
}
