import { useEffect } from 'react'
import { useNavigate, useParams } from 'react-router-dom'
import { useForm } from 'react-hook-form'
import { zodResolver } from '@hookform/resolvers/zod'
import { useMeal, useCreateMeal, useUpdateMeal } from '../../hooks/useMeals'
import Button from '../../components/ui/Button'
import Input from '../../components/ui/Input'
import TextArea from '../../components/ui/TextArea'
import Select from '../../components/ui/Select'
import PageHeader from '../../components/layout/PageHeader'
import LoadingSpinner from '../../components/ui/LoadingSpinner'
import { mealSchema, type MealFormData } from './mealSchema'

const MEAL_TYPE_OPTIONS = [
  { value: 'Breakfast', label: 'Breakfast' },
  { value: 'Lunch', label: 'Lunch' },
  { value: 'Dinner', label: 'Dinner' },
  { value: 'Snack', label: 'Snack' },
]

export default function MealFormPage() {
  const { id } = useParams<{ id?: string }>()
  const mealId = id ? Number(id) : undefined
  const isEdit = mealId !== undefined
  const navigate = useNavigate()

  const { data: meal, isLoading: isLoadingMeal } = useMeal(mealId ?? 0)
  const createMeal = useCreateMeal()
  const updateMeal = useUpdateMeal(mealId ?? 0)

  const {
    register,
    handleSubmit,
    reset,
    formState: { errors, isSubmitting },
  } = useForm<MealFormData>({
    resolver: zodResolver(mealSchema),
    defaultValues: { mealType: 'Dinner', isMultiDayMeal: false },
  })

  useEffect(() => {
    if (meal) {
      reset({
        name: meal.name ?? '',
        description: meal.description ?? '',
        notes: meal.notes ?? '',
        mealType: meal.mealType,
        isMultiDayMeal: meal.isMultiDayMeal,
      })
    }
  }, [meal, reset])

  const onSubmit = async (data: MealFormData) => {
    const payload = {
      name: data.name,
      description: data.description || undefined,
      notes: data.notes || undefined,
      mealType: data.mealType,
      isMultiDayMeal: data.isMultiDayMeal ?? false,
    }
    if (isEdit && mealId) {
      await updateMeal.mutateAsync({ ...payload, id: mealId })
      navigate(`/meals/${mealId}`)
    } else {
      const created = await createMeal.mutateAsync(payload)
      navigate(`/meals/${created.id}`)
    }
  }

  if (isEdit && isLoadingMeal) {
    return (
      <div className="flex justify-center py-16">
        <LoadingSpinner size="lg" />
      </div>
    )
  }

  return (
    <div className="max-w-2xl">
      <PageHeader title={isEdit ? 'Edit meal' : 'New meal'} />

      <form onSubmit={handleSubmit(onSubmit)} noValidate className="mt-6 space-y-5">
        <Input
          label="Meal name *"
          error={errors.name?.message}
          {...register('name')}
        />

        <Select
          label="Meal type *"
          options={MEAL_TYPE_OPTIONS}
          error={errors.mealType?.message}
          {...register('mealType')}
        />

        <TextArea
          label="Description"
          rows={2}
          placeholder="A brief description of the meal"
          error={errors.description?.message}
          {...register('description')}
        />

        <TextArea
          label="Notes"
          rows={3}
          placeholder="Private notes for yourself"
          error={errors.notes?.message}
          {...register('notes')}
        />

        <label className="flex items-center gap-3">
          <input type="checkbox" className="h-4 w-4 rounded" {...register('isMultiDayMeal')} />
          <span className="text-sm font-medium text-gray-700">This is a multi-day meal</span>
        </label>

        <div className="flex gap-3 pt-2">
          <Button type="submit" isLoading={isSubmitting}>
            {isEdit ? 'Save changes' : 'Create meal'}
          </Button>
          <Button
            type="button"
            variant="secondary"
            onClick={() => navigate(isEdit && mealId ? `/meals/${mealId}` : '/meals')}
          >
            Cancel
          </Button>
        </div>
      </form>
    </div>
  )
}
