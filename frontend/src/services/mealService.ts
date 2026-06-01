import { apiFetch } from './apiClient'
import type { Meal, MealItem, CreateMeal, UpdateMeal, CreateMealItem, UpdateMealItem } from '../types/meal'
import type { ResourcePermission, ShareRequest } from '../types/shared'

export const mealService = {
  list: () => apiFetch<Meal[]>('/api/meal'),
  sharedWithMe: () => apiFetch<Meal[]>('/api/meal/shared-with-me'),
  get: (id: number) => apiFetch<Meal>(`/api/meal/${id}`),
  create: (data: CreateMeal) => apiFetch<Meal>('/api/meal', { method: 'POST', body: JSON.stringify(data) }),
  update: (id: number, data: UpdateMeal) => apiFetch<Meal>(`/api/meal/${id}`, { method: 'PUT', body: JSON.stringify(data) }),
  delete: (id: number) => apiFetch<void>(`/api/meal/${id}`, { method: 'DELETE' }),
  clone: (id: number) => apiFetch<Meal>(`/api/meal/${id}/clone`, { method: 'POST' }),

  items: {
    list: (mealId: number) => apiFetch<MealItem[]>(`/api/meal/${mealId}/items`),
    create: (mealId: number, data: CreateMealItem) =>
      apiFetch<MealItem>(`/api/meal/${mealId}/items`, { method: 'POST', body: JSON.stringify(data) }),
    update: (data: UpdateMealItem) =>
      apiFetch<MealItem>('/api/meal/items', { method: 'PUT', body: JSON.stringify(data) }),
    delete: (mealItemId: number) => apiFetch<void>(`/api/meal/items/${mealItemId}`, { method: 'DELETE' }),
  },

  shares: {
    list: (mealId: number) => apiFetch<ResourcePermission[]>(`/api/meal/${mealId}/shares`),
    grant: (mealId: number, data: ShareRequest) =>
      apiFetch<void>(`/api/meal/${mealId}/share`, { method: 'POST', body: JSON.stringify(data) }),
    revoke: (mealId: number, shareId: number) =>
      apiFetch<void>(`/api/meal/${mealId}/shares/${shareId}`, { method: 'DELETE' }),
  },
}
