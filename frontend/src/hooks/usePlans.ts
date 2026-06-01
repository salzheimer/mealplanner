import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query'
import { planService } from '../services/planService'
import { queryKeys } from '../utils/queryKeys'
import type { CreatePlan, UpdatePlan } from '../types/plan'

export function usePlans() {
  return useQuery({
    queryKey: queryKeys.plans.all,
    queryFn: planService.list,
  })
}

export function useSharedPlans() {
  return useQuery({
    queryKey: queryKeys.plans.sharedWithMe,
    queryFn: planService.sharedWithMe,
  })
}

export function usePlan(id: number) {
  return useQuery({
    queryKey: queryKeys.plans.detail(id),
    queryFn: () => planService.get(id),
    enabled: id > 0,
  })
}

export function useCreatePlan() {
  const queryClient = useQueryClient()
  return useMutation({
    mutationFn: (data: CreatePlan) => planService.create(data),
    onSuccess: () => queryClient.invalidateQueries({ queryKey: queryKeys.plans.all }),
  })
}

export function useUpdatePlan(id: number) {
  const queryClient = useQueryClient()
  return useMutation({
    mutationFn: (data: UpdatePlan) => planService.update(id, data),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: queryKeys.plans.all })
      queryClient.invalidateQueries({ queryKey: queryKeys.plans.detail(id) })
    },
  })
}

export function useDeletePlan() {
  const queryClient = useQueryClient()
  return useMutation({
    mutationFn: (id: number) => planService.delete(id),
    onSuccess: () => queryClient.invalidateQueries({ queryKey: queryKeys.plans.all }),
  })
}
