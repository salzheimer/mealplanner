import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query'
import { recipeService } from '../services/recipeService'
import { queryKeys } from '../utils/queryKeys'
import type { CreateRecipe, UpdateRecipe, CreateRecipeIngredient, UpdateRecipeIngredient, CreateRecipeInstruction, UpdateRecipeInstruction } from '../types/recipe'

export function useRecipes() {
  return useQuery({
    queryKey: queryKeys.recipes.all,
    queryFn: recipeService.list,
  })
}

export function useSharedRecipes() {
  return useQuery({
    queryKey: queryKeys.recipes.sharedWithMe,
    queryFn: recipeService.sharedWithMe,
  })
}

export function useRecipe(id: number) {
  return useQuery({
    queryKey: queryKeys.recipes.detail(id),
    queryFn: () => recipeService.get(id),
    enabled: id > 0,
  })
}

export function useCreateRecipe() {
  const queryClient = useQueryClient()
  return useMutation({
    mutationFn: (data: CreateRecipe) => recipeService.create(data),
    onSuccess: () => queryClient.invalidateQueries({ queryKey: queryKeys.recipes.all }),
  })
}

export function useUpdateRecipe(id: number) {
  const queryClient = useQueryClient()
  return useMutation({
    mutationFn: (data: UpdateRecipe) => recipeService.update(id, data),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: queryKeys.recipes.all })
      queryClient.invalidateQueries({ queryKey: queryKeys.recipes.detail(id) })
    },
  })
}

export function useDeleteRecipe() {
  const queryClient = useQueryClient()
  return useMutation({
    mutationFn: (id: number) => recipeService.delete(id),
    onSuccess: () => queryClient.invalidateQueries({ queryKey: queryKeys.recipes.all }),
  })
}

export function useRecipeIngredients(recipeId: number) {
  return useQuery({
    queryKey: queryKeys.recipes.ingredients(recipeId),
    queryFn: () => recipeService.ingredients.list(recipeId),
    enabled: recipeId > 0,
  })
}

export function useCreateIngredient(recipeId: number) {
  const queryClient = useQueryClient()
  return useMutation({
    mutationFn: (data: CreateRecipeIngredient) => recipeService.ingredients.create(recipeId, data),
    onSuccess: () => queryClient.invalidateQueries({ queryKey: queryKeys.recipes.detail(recipeId) }),
  })
}

export function useUpdateIngredient(recipeId: number) {
  const queryClient = useQueryClient()
  return useMutation({
    mutationFn: ({ id, data }: { id: number; data: UpdateRecipeIngredient }) =>
      recipeService.ingredients.update(recipeId, id, data),
    onSuccess: () => queryClient.invalidateQueries({ queryKey: queryKeys.recipes.detail(recipeId) }),
  })
}

export function useDeleteIngredient(recipeId: number) {
  const queryClient = useQueryClient()
  return useMutation({
    mutationFn: (ingredientId: number) => recipeService.ingredients.delete(recipeId, ingredientId),
    onSuccess: () => queryClient.invalidateQueries({ queryKey: queryKeys.recipes.detail(recipeId) }),
  })
}

export function useRecipeInstructions(recipeId: number) {
  return useQuery({
    queryKey: queryKeys.recipes.instructions(recipeId),
    queryFn: () => recipeService.instructions.list(recipeId),
    enabled: recipeId > 0,
  })
}

export function useCreateInstruction(recipeId: number) {
  const queryClient = useQueryClient()
  return useMutation({
    mutationFn: (data: CreateRecipeInstruction) => recipeService.instructions.create(recipeId, data),
    onSuccess: () => queryClient.invalidateQueries({ queryKey: queryKeys.recipes.detail(recipeId) }),
  })
}

export function useUpdateInstruction(recipeId: number) {
  const queryClient = useQueryClient()
  return useMutation({
    mutationFn: ({ id, data }: { id: number; data: UpdateRecipeInstruction }) =>
      recipeService.instructions.update(recipeId, id, data),
    onSuccess: () => queryClient.invalidateQueries({ queryKey: queryKeys.recipes.detail(recipeId) }),
  })
}

export function useDeleteInstruction(recipeId: number) {
  const queryClient = useQueryClient()
  return useMutation({
    mutationFn: (instructionId: number) => recipeService.instructions.delete(recipeId, instructionId),
    onSuccess: () => queryClient.invalidateQueries({ queryKey: queryKeys.recipes.detail(recipeId) }),
  })
}
