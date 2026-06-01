import { useEffect } from 'react'
import { useNavigate, useParams } from 'react-router-dom'
import { useForm } from 'react-hook-form'
import { zodResolver } from '@hookform/resolvers/zod'
import { usePlan, useCreatePlan, useUpdatePlan } from '../../hooks/usePlans'
import Button from '../../components/ui/Button'
import Input from '../../components/ui/Input'
import PageHeader from '../../components/layout/PageHeader'
import LoadingSpinner from '../../components/ui/LoadingSpinner'
import { planSchema, type PlanFormData } from './planSchema'

export default function PlanFormPage() {
  const { id } = useParams<{ id?: string }>()
  const planId = id ? Number(id) : undefined
  const isEdit = planId !== undefined
  const navigate = useNavigate()

  const { data: plan, isLoading: isLoadingPlan } = usePlan(planId ?? 0)
  const createPlan = useCreatePlan()
  const updatePlan = useUpdatePlan(planId ?? 0)

  const {
    register,
    handleSubmit,
    reset,
    formState: { errors, isSubmitting },
  } = useForm<PlanFormData>({
    resolver: zodResolver(planSchema),
  })

  useEffect(() => {
    if (plan) {
      reset({
        name: plan.name ?? '',
        startDate: plan.startDate,
        endDate: plan.endDate ?? '',
      })
    }
  }, [plan, reset])

  const onSubmit = async (data: PlanFormData) => {
    const payload = {
      name: data.name || undefined,
      startDate: data.startDate,
      endDate: data.endDate || undefined,
    }
    if (isEdit && planId) {
      await updatePlan.mutateAsync({ ...payload, id: planId })
      navigate(`/plans/${planId}`)
    } else {
      const created = await createPlan.mutateAsync(payload)
      navigate(`/plans/${created.id}`)
    }
  }

  if (isEdit && isLoadingPlan) {
    return (
      <div className="flex justify-center py-16">
        <LoadingSpinner size="lg" />
      </div>
    )
  }

  return (
    <div className="max-w-lg">
      <PageHeader title={isEdit ? 'Edit plan' : 'New plan'} />

      <form onSubmit={handleSubmit(onSubmit)} noValidate className="mt-6 space-y-5">
        <Input
          label="Plan name"
          placeholder="e.g. Weekly meal plan"
          error={errors.name?.message}
          {...register('name')}
        />

        <Input
          label="Start date *"
          type="date"
          error={errors.startDate?.message}
          {...register('startDate')}
        />

        <Input
          label="End date"
          type="date"
          error={errors.endDate?.message}
          {...register('endDate')}
        />

        <div className="flex gap-3 pt-2">
          <Button type="submit" isLoading={isSubmitting}>
            {isEdit ? 'Save changes' : 'Create plan'}
          </Button>
          <Button
            type="button"
            variant="secondary"
            onClick={() => navigate(isEdit && planId ? `/plans/${planId}` : '/plans')}
          >
            Cancel
          </Button>
        </div>
      </form>
    </div>
  )
}
