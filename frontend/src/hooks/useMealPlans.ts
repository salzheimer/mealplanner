import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query'
import { mealPlanService } from '../services/planService'
import { queryKeys } from '../utils/queryKeys'
import type { CreateMealPlan, UpdateMealPlan, CreateMealItemPlan, UpdateMealItemPlan } from '../types/plan'

export function useMealPlansByDateRange(startDate: string, endDate: string) {
  return useQuery({
    queryKey: queryKeys.mealPlans.byDateRange(startDate, endDate),
    queryFn: () => mealPlanService.byDateRange(startDate, endDate),
    enabled: Boolean(startDate && endDate),
  })
}

export function useCreateMealPlan() {
  const queryClient = useQueryClient()
  return useMutation({
    mutationFn: (data: CreateMealPlan) => mealPlanService.create(data),
    onSuccess: (_data, variables) => {
      queryClient.invalidateQueries({ queryKey: queryKeys.plans.detail(variables.planId) })
      queryClient.invalidateQueries({ queryKey: queryKeys.mealPlans.all })
    },
  })
}

export function useUpdateMealPlan() {
  const queryClient = useQueryClient()
  return useMutation({
    mutationFn: (data: UpdateMealPlan) => mealPlanService.update(data),
    onSuccess: () => queryClient.invalidateQueries({ queryKey: queryKeys.mealPlans.all }),
  })
}

export function useDeleteMealPlan() {
  const queryClient = useQueryClient()
  return useMutation({
    mutationFn: (id: number) => mealPlanService.delete(id),
    onSuccess: () => queryClient.invalidateQueries({ queryKey: queryKeys.mealPlans.all }),
  })
}

export function useMealPlanItems(mealPlanId: number) {
  return useQuery({
    queryKey: queryKeys.mealPlans.items(mealPlanId),
    queryFn: () => mealPlanService.items.list(mealPlanId),
    enabled: mealPlanId > 0,
  })
}

export function useCreateMealItemPlan(mealPlanId: number) {
  const queryClient = useQueryClient()
  return useMutation({
    mutationFn: (data: CreateMealItemPlan) => mealPlanService.items.create(mealPlanId, data),
    onSuccess: () => queryClient.invalidateQueries({ queryKey: queryKeys.mealPlans.items(mealPlanId) }),
  })
}

export function useUpdateMealItemPlan(mealPlanId: number) {
  const queryClient = useQueryClient()
  return useMutation({
    mutationFn: ({ id, data }: { id: number; data: UpdateMealItemPlan }) =>
      mealPlanService.items.update(mealPlanId, id, data),
    onSuccess: () => queryClient.invalidateQueries({ queryKey: queryKeys.mealPlans.items(mealPlanId) }),
  })
}

export function useDeleteMealItemPlan() {
  const queryClient = useQueryClient()
  return useMutation({
    mutationFn: (mealItemPlanId: number) => mealPlanService.items.delete(mealItemPlanId),
    onSuccess: () => queryClient.invalidateQueries({ queryKey: queryKeys.mealPlans.all }),
  })
}
