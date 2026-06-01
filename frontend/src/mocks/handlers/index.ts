import { authHandlers } from './auth.handlers'
import { recipeHandlers } from './recipe.handlers'
import { mealHandlers } from './meal.handlers'
import { planHandlers } from './plan.handlers'

export const handlers = [
  ...authHandlers,
  ...recipeHandlers,
  ...mealHandlers,
  ...planHandlers,
]
