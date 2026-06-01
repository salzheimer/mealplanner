import type { ItemStatus } from './shared'

export interface PlanSummary {
  id: number
  name: string | null
  startDate: string
  endDate: string | null
  ownerUserId: number
  createdAt: string
  updatedAt: string
}

export interface PlanDetail extends PlanSummary {
  mealPlans: MealPlan[]
}

export interface MealPlan {
  id: number
  mealId: number
  planId: number
  serveDate: string | null
  endDate: string | null
  addedByUserId: number
  createdAt: string
  updatedAt: string
}

export interface MealItemPlan {
  id: number
  mealPlanId: number
  mealItemId: number
  assignedToUserId: number | null
  assignedToGuestName: string | null
  status: ItemStatus
  notes: string | null
  createdAt: string
  updatedAt: string
}

export interface PlanShare {
  id: number
  planId: number
  sharedWithUserId: number
  permission: string
  createdAt: string
}

export interface CreatePlan {
  name?: string
  startDate: string
  endDate?: string
}

export interface UpdatePlan extends CreatePlan {
  id: number
}

export interface CreateMealPlan {
  mealId: number
  planId: number
  serveDate?: string
  endDate?: string
}

export interface UpdateMealPlan {
  id: number
  serveDate?: string
  endDate?: string
}

export interface CreateMealItemPlan {
  mealItemId: number
  assignedToUserId?: number
  assignedToGuestName?: string
  status?: ItemStatus
  notes?: string
}

export interface UpdateMealItemPlan {
  id: number
  assignedToUserId?: number
  assignedToGuestName?: string
  status?: ItemStatus
  notes?: string
}

export interface CreatePlanShare {
  planId: number
  sharedWithUserId: number
  permission: string
}

export interface UpdatePlanShare {
  permission: string
}
