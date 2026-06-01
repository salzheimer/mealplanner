import { apiFetch } from './apiClient'
import type {
  PlanSummary,
  PlanDetail,
  MealPlan,
  MealItemPlan,
  PlanShare,
  CreatePlan,
  UpdatePlan,
  CreateMealPlan,
  UpdateMealPlan,
  CreateMealItemPlan,
  UpdateMealItemPlan,
  CreatePlanShare,
  UpdatePlanShare,
} from '../types/plan'

export const planService = {
  list: () => apiFetch<PlanSummary[]>('/api/plans'),
  sharedWithMe: () => apiFetch<PlanSummary[]>('/api/plans/shared-with-me'),
  get: (id: number) => apiFetch<PlanDetail>(`/api/plans/${id}`),
  create: (data: CreatePlan) => apiFetch<PlanSummary>('/api/plans', { method: 'POST', body: JSON.stringify(data) }),
  update: (id: number, data: UpdatePlan) => apiFetch<PlanSummary>(`/api/plans/${id}`, { method: 'PUT', body: JSON.stringify(data) }),
  delete: (id: number) => apiFetch<void>(`/api/plans/${id}`, { method: 'DELETE' }),

  shares: {
    list: (planId: number) => apiFetch<PlanShare[]>(`/api/plans/${planId}/shares`),
    grant: (data: CreatePlanShare) =>
      apiFetch<PlanShare>('/api/plans/shares', { method: 'POST', body: JSON.stringify(data) }),
    update: (planId: number, shareId: number, data: UpdatePlanShare) =>
      apiFetch<PlanShare>(`/api/plans/${planId}/shares/${shareId}`, { method: 'PUT', body: JSON.stringify(data) }),
    revoke: (planId: number, shareId: number) =>
      apiFetch<void>(`/api/plans/${planId}/shares/${shareId}`, { method: 'DELETE' }),
  },
}

export const mealPlanService = {
  getById: (id: number) => apiFetch<MealPlan>(`/api/mealplan/${id}`),
  byDateRange: (startDate: string, endDate: string) =>
    apiFetch<MealPlan[]>(`/api/mealplan/date-range?startDate=${startDate}&endDate=${endDate}`),
  create: (data: CreateMealPlan) => apiFetch<MealPlan>('/api/mealplan', { method: 'POST', body: JSON.stringify(data) }),
  update: (data: UpdateMealPlan) => apiFetch<MealPlan>('/api/mealplan', { method: 'PUT', body: JSON.stringify(data) }),
  delete: (id: number) => apiFetch<void>(`/api/mealplan/${id}`, { method: 'DELETE' }),

  items: {
    list: (mealPlanId: number) => apiFetch<MealItemPlan[]>(`/api/mealplan/${mealPlanId}/mealitems`),
    create: (mealPlanId: number, data: CreateMealItemPlan) =>
      apiFetch<MealItemPlan>(`/api/mealplan/${mealPlanId}/mealitems`, { method: 'POST', body: JSON.stringify(data) }),
    update: (mealPlanId: number, mealItemId: number, data: UpdateMealItemPlan) =>
      apiFetch<MealItemPlan>(`/api/mealplan/${mealPlanId}/mealitems/${mealItemId}`, { method: 'PUT', body: JSON.stringify(data) }),
    delete: (mealItemPlanId: number) => apiFetch<void>(`/api/mealplan/mealitem/${mealItemPlanId}`, { method: 'DELETE' }),
  },
}
