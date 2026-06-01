import { apiFetch } from './apiClient'
import type {
  RecipeSummary,
  RecipeDetail,
  CreateRecipe,
  UpdateRecipe,
  RecipeIngredient,
  CreateRecipeIngredient,
  UpdateRecipeIngredient,
  RecipeInstruction,
  CreateRecipeInstruction,
  UpdateRecipeInstruction,
} from '../types/recipe'

export const recipeService = {
  list: () => apiFetch<RecipeSummary[]>('/api/recipes'),
  sharedWithMe: () => apiFetch<RecipeSummary[]>('/api/recipes/shared-with-me'),
  get: (id: number) => apiFetch<RecipeDetail>(`/api/recipes/${id}`),
  create: (data: CreateRecipe) => apiFetch<RecipeDetail>('/api/recipes', { method: 'POST', body: JSON.stringify(data) }),
  update: (id: number, data: UpdateRecipe) => apiFetch<RecipeDetail>(`/api/recipes/${id}`, { method: 'PUT', body: JSON.stringify(data) }),
  delete: (id: number) => apiFetch<void>(`/api/recipes/${id}`, { method: 'DELETE' }),
  clone: (id: number) => apiFetch<RecipeDetail>(`/api/recipes/${id}/clone`, { method: 'POST' }),

  ingredients: {
    list: (recipeId: number) => apiFetch<RecipeIngredient[]>(`/api/recipes/${recipeId}/ingredients`),
    create: (recipeId: number, data: CreateRecipeIngredient) =>
      apiFetch<RecipeIngredient>(`/api/recipes/${recipeId}/ingredients`, { method: 'POST', body: JSON.stringify(data) }),
    update: (recipeId: number, ingredientId: number, data: UpdateRecipeIngredient) =>
      apiFetch<RecipeIngredient>(`/api/recipes/${recipeId}/ingredients/${ingredientId}`, { method: 'PUT', body: JSON.stringify(data) }),
    delete: (recipeId: number, ingredientId: number) =>
      apiFetch<void>(`/api/recipes/${recipeId}/ingredients/${ingredientId}`, { method: 'DELETE' }),
  },

  instructions: {
    list: (recipeId: number) => apiFetch<RecipeInstruction[]>(`/api/recipes/${recipeId}/instructions`),
    create: (recipeId: number, data: CreateRecipeInstruction) =>
      apiFetch<RecipeInstruction>(`/api/recipes/${recipeId}/instructions`, { method: 'POST', body: JSON.stringify(data) }),
    update: (recipeId: number, instructionId: number, data: UpdateRecipeInstruction) =>
      apiFetch<RecipeInstruction>(`/api/recipes/${recipeId}/instructions/${instructionId}`, { method: 'PUT', body: JSON.stringify(data) }),
    delete: (recipeId: number, instructionId: number) =>
      apiFetch<void>(`/api/recipes/${recipeId}/instructions/${instructionId}`, { method: 'DELETE' }),
  },

  shares: {
    list: (recipeId: number) => apiFetch<import('../types/shared').ResourcePermission[]>(`/api/recipes/${recipeId}/shares`),
    grant: (recipeId: number, data: import('../types/shared').ShareRequest) =>
      apiFetch<void>(`/api/recipes/${recipeId}/share`, { method: 'POST', body: JSON.stringify(data) }),
    revoke: (recipeId: number, shareId: number) =>
      apiFetch<void>(`/api/recipes/${recipeId}/shares/${shareId}`, { method: 'DELETE' }),
  },
}
