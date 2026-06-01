import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query'
import { mealService } from '../services/mealService'
import { queryKeys } from '../utils/queryKeys'
import type { CreateMeal, UpdateMeal, CreateMealItem, UpdateMealItem } from '../types/meal'

export function useMeals() {
  return useQuery({
    queryKey: queryKeys.meals.all,
    queryFn: mealService.list,
  })
}

export function useSharedMeals() {
  return useQuery({
    queryKey: queryKeys.meals.sharedWithMe,
    queryFn: mealService.sharedWithMe,
  })
}

export function useMeal(id: number) {
  return useQuery({
    queryKey: queryKeys.meals.detail(id),
    queryFn: () => mealService.get(id),
    enabled: id > 0,
  })
}

export function useCreateMeal() {
  const queryClient = useQueryClient()
  return useMutation({
    mutationFn: (data: CreateMeal) => mealService.create(data),
    onSuccess: () => queryClient.invalidateQueries({ queryKey: queryKeys.meals.all }),
  })
}

export function useUpdateMeal(id: number) {
  const queryClient = useQueryClient()
  return useMutation({
    mutationFn: (data: UpdateMeal) => mealService.update(id, data),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: queryKeys.meals.all })
      queryClient.invalidateQueries({ queryKey: queryKeys.meals.detail(id) })
    },
  })
}

export function useDeleteMeal() {
  const queryClient = useQueryClient()
  return useMutation({
    mutationFn: (id: number) => mealService.delete(id),
    onSuccess: () => queryClient.invalidateQueries({ queryKey: queryKeys.meals.all }),
  })
}

export function useMealItems(mealId: number) {
  return useQuery({
    queryKey: queryKeys.meals.items(mealId),
    queryFn: () => mealService.items.list(mealId),
    enabled: mealId > 0,
  })
}

export function useCreateMealItem(mealId: number) {
  const queryClient = useQueryClient()
  return useMutation({
    mutationFn: (data: CreateMealItem) => mealService.items.create(mealId, data),
    onSuccess: () => queryClient.invalidateQueries({ queryKey: queryKeys.meals.items(mealId) }),
  })
}

export function useUpdateMealItem(mealId: number) {
  const queryClient = useQueryClient()
  return useMutation({
    mutationFn: (data: UpdateMealItem) => mealService.items.update(data),
    onSuccess: () => queryClient.invalidateQueries({ queryKey: queryKeys.meals.items(mealId) }),
  })
}

export function useDeleteMealItem(mealId: number) {
  const queryClient = useQueryClient()
  return useMutation({
    mutationFn: (mealItemId: number) => mealService.items.delete(mealItemId),
    onSuccess: () => queryClient.invalidateQueries({ queryKey: queryKeys.meals.items(mealId) }),
  })
}
