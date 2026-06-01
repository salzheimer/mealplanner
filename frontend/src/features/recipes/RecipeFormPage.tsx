import { useEffect } from 'react'
import { useNavigate, useParams } from 'react-router-dom'
import { useForm } from 'react-hook-form'
import { zodResolver } from '@hookform/resolvers/zod'
import { useRecipe, useCreateRecipe, useUpdateRecipe } from '../../hooks/useRecipes'
import Button from '../../components/ui/Button'
import Input from '../../components/ui/Input'
import TextArea from '../../components/ui/TextArea'
import PageHeader from '../../components/layout/PageHeader'
import LoadingSpinner from '../../components/ui/LoadingSpinner'
import { recipeSchema, type RecipeFormData, parseDurationToForm, formToDuration } from './recipeSchema'

export default function RecipeFormPage() {
  const { id } = useParams<{ id?: string }>()
  const recipeId = id ? Number(id) : undefined
  const isEdit = recipeId !== undefined
  const navigate = useNavigate()

  const { data: recipe, isLoading: isLoadingRecipe } = useRecipe(recipeId ?? 0)
  const createRecipe = useCreateRecipe()
  const updateRecipe = useUpdateRecipe(recipeId ?? 0)

  const {
    register,
    handleSubmit,
    reset,
    formState: { errors, isSubmitting },
  } = useForm<RecipeFormData>({
    resolver: zodResolver(recipeSchema),
  })

  useEffect(() => {
    if (recipe) {
      const prep = parseDurationToForm(recipe.prepTime)
      const cook = parseDurationToForm(recipe.cookTime)
      reset({
        name: recipe.name,
        description: recipe.description ?? '',
        notes: recipe.notes ?? '',
        originalSource: recipe.originalSource ?? '',
        servings: recipe.servings != null ? String(recipe.servings) : '',
        prepTimeHours: prep.hours,
        prepTimeMinutes: prep.minutes,
        cookTimeHours: cook.hours,
        cookTimeMinutes: cook.minutes,
      })
    }
  }, [recipe, reset])

  const onSubmit = async (data: RecipeFormData) => {
    const prepTime = formToDuration(data.prepTimeHours, data.prepTimeMinutes)
    const cookTime = formToDuration(data.cookTimeHours, data.cookTimeMinutes)
    const servings = data.servings ? parseInt(data.servings, 10) : undefined
    const payload = {
      name: data.name,
      description: data.description || undefined,
      notes: data.notes || undefined,
      originalSource: data.originalSource || undefined,
      servings: Number.isNaN(servings ?? NaN) ? undefined : servings,
      prepTime,
      cookTime,
    }
    if (isEdit && recipeId) {
      const updated = await updateRecipe.mutateAsync({ ...payload, id: recipeId })
      navigate(`/recipes/${updated.id}`)
    } else {
      const created = await createRecipe.mutateAsync(payload)
      navigate(`/recipes/${created.id}`)
    }
  }

  if (isEdit && isLoadingRecipe) {
    return (
      <div className="flex justify-center py-16">
        <LoadingSpinner size="lg" />
      </div>
    )
  }

  return (
    <div className="max-w-2xl">
      <PageHeader title={isEdit ? 'Edit recipe' : 'New recipe'} />

      <form onSubmit={handleSubmit(onSubmit)} noValidate className="mt-6 space-y-5">
        <Input
          label="Recipe name *"
          error={errors.name?.message}
          {...register('name')}
        />

        <TextArea
          label="Description"
          rows={2}
          placeholder="A brief description of the recipe"
          error={errors.description?.message}
          {...register('description')}
        />

        <div className="grid grid-cols-2 gap-4">
          <div>
            <label className="mb-1 block text-sm font-medium text-gray-700">Prep time</label>
            <div className="flex gap-2">
              <Input
                type="number"
                min={0}
                max={23}
                placeholder="h"
                {...register('prepTimeHours')}
              />
              <Input
                type="number"
                min={0}
                max={59}
                placeholder="min"
                {...register('prepTimeMinutes')}
              />
            </div>
          </div>
          <div>
            <label className="mb-1 block text-sm font-medium text-gray-700">Cook time</label>
            <div className="flex gap-2">
              <Input
                type="number"
                min={0}
                max={23}
                placeholder="h"
                {...register('cookTimeHours')}
              />
              <Input
                type="number"
                min={0}
                max={59}
                placeholder="min"
                {...register('cookTimeMinutes')}
              />
            </div>
          </div>
        </div>

        <Input
          label="Servings"
          type="number"
          min={1}
          placeholder="e.g. 4"
          error={errors.servings?.message}
          {...register('servings')}
        />

        <TextArea
          label="Notes"
          rows={3}
          placeholder="Private notes for yourself"
          error={errors.notes?.message}
          {...register('notes')}
        />

        <Input
          label="Original source"
          placeholder="Where did this recipe come from?"
          error={errors.originalSource?.message}
          {...register('originalSource')}
        />

        <div className="flex gap-3 pt-2">
          <Button type="submit" isLoading={isSubmitting}>
            {isEdit ? 'Save changes' : 'Create recipe'}
          </Button>
          <Button
            type="button"
            variant="secondary"
            onClick={() => navigate(isEdit && recipeId ? `/recipes/${recipeId}` : '/recipes')}
          >
            Cancel
          </Button>
        </div>
      </form>
    </div>
  )
}
