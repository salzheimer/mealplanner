import type { MealType, ItemType } from './shared'

export interface Meal {
  id: number
  name: string | null
  description: string | null
  notes: string | null
  mealType: MealType
  isMultiDayMeal: boolean
  ownerUserId: number
  createdAt: string
  updatedAt: string
}

export interface MealItem {
  id: number
  mealId: number
  name: string | null
  recipeId: number | null
  itemType: ItemType
  createdAt: string
  updatedAt: string
}

export interface CreateMeal {
  name?: string
  description?: string
  notes?: string
  mealType: MealType
  isMultiDayMeal?: boolean
}

export interface UpdateMeal extends CreateMeal {
  id: number
}

export interface CreateMealItem {
  name?: string
  recipeId?: number
  itemType: ItemType
}

export interface UpdateMealItem extends CreateMealItem {
  id: number
}
