import { z } from 'zod'

const MEAL_TYPES = ['Breakfast', 'Lunch', 'Dinner', 'Snack'] as const

export const mealSchema = z.object({
  name: z.string().min(1, 'Meal name is required'),
  description: z.string().optional(),
  notes: z.string().optional(),
  mealType: z.enum(MEAL_TYPES, { error: 'Select a meal type' }),
  isMultiDayMeal: z.boolean().optional(),
})

export type MealFormData = z.infer<typeof mealSchema>
