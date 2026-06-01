import { http, HttpResponse } from 'msw'
import { recipeFixtures } from '../fixtures/recipe.fixtures'
import type { CreateRecipe, UpdateRecipe, CreateRecipeIngredient, UpdateRecipeIngredient, CreateRecipeInstruction, UpdateRecipeInstruction } from '../../types/recipe'

export const recipeHandlers = [
  http.get('/api/recipes', () => {
    return HttpResponse.json(recipeFixtures.list)
  }),

  http.get('/api/recipes/shared-with-me', () => {
    return HttpResponse.json([])
  }),

  http.get('/api/recipes/:id', ({ params }) => {
    return HttpResponse.json(recipeFixtures.detail(Number(params.id)))
  }),

  http.post('/api/recipes', async ({ request }) => {
    const body = await request.json() as CreateRecipe
    const newRecipe = {
      ...recipeFixtures.list[0],
      ...body,
      id: 99,
      createdAt: new Date().toISOString(),
      updatedAt: new Date().toISOString(),
    }
    return HttpResponse.json(newRecipe, { status: 201 })
  }),

  http.post('/api/recipes/:recipeId/clone', ({ params }) => {
    const original = recipeFixtures.detail(Number(params.recipeId))
    return HttpResponse.json({ ...original, id: 100, name: `${original.name} (copy)` }, { status: 201 })
  }),

  http.put('/api/recipes/:recipeId', async ({ params, request }) => {
    const body = await request.json() as UpdateRecipe
    const existing = recipeFixtures.detail(Number(params.recipeId))
    return HttpResponse.json({ ...existing, ...body, updatedAt: new Date().toISOString() })
  }),

  http.delete('/api/recipes/:id', () => {
    return new HttpResponse(null, { status: 204 })
  }),

  // Ingredients
  http.get('/api/recipes/:recipeId/ingredients', ({ params }) => {
    const detail = recipeFixtures.detail(Number(params.recipeId))
    return HttpResponse.json(detail.ingredients)
  }),

  http.post('/api/recipes/:recipeId/ingredients', async ({ params, request }) => {
    const body = await request.json() as CreateRecipeIngredient
    const newIngredient = {
      id: 200,
      recipeId: Number(params.recipeId),
      name: body.name ?? null,
      amount: body.amount ?? null,
      measurementType: body.measurementType ?? null,
      note: body.note ?? null,
      createdAt: new Date().toISOString(),
      updatedAt: new Date().toISOString(),
    }
    return HttpResponse.json(newIngredient, { status: 201 })
  }),

  http.put('/api/recipes/:recipeId/ingredients/:ingredientId', async ({ params, request }) => {
    const body = await request.json() as UpdateRecipeIngredient
    const detail = recipeFixtures.detail(Number(params.recipeId))
    const existing = detail.ingredients.find(i => i.id === Number(params.ingredientId))
    return HttpResponse.json({ ...existing, ...body, updatedAt: new Date().toISOString() })
  }),

  http.delete('/api/recipes/:recipeId/ingredients/:ingredientId', () => {
    return new HttpResponse(null, { status: 204 })
  }),

  // Instructions
  http.get('/api/recipes/:recipeId/instructions', ({ params }) => {
    const detail = recipeFixtures.detail(Number(params.recipeId))
    return HttpResponse.json(detail.instructions)
  }),

  http.post('/api/recipes/:recipeId/instructions', async ({ params, request }) => {
    const body = await request.json() as CreateRecipeInstruction
    const newInstruction = {
      id: 300,
      recipeId: Number(params.recipeId),
      stepNumber: body.stepNumber ?? null,
      description: body.description ?? null,
      note: body.note ?? null,
      createdAt: new Date().toISOString(),
      updatedAt: new Date().toISOString(),
    }
    return HttpResponse.json(newInstruction, { status: 201 })
  }),

  http.put('/api/recipes/:recipeId/instructions/:instructionId', async ({ params, request }) => {
    const body = await request.json() as UpdateRecipeInstruction
    const detail = recipeFixtures.detail(Number(params.recipeId))
    const existing = detail.instructions.find(i => i.id === Number(params.instructionId))
    return HttpResponse.json({ ...existing, ...body, updatedAt: new Date().toISOString() })
  }),

  http.delete('/api/recipes/:recipeId/instructions/:instructionId', () => {
    return new HttpResponse(null, { status: 204 })
  }),

  // Sharing
  http.get('/api/recipes/:recipeId/shares', () => {
    return HttpResponse.json([])
  }),

  http.post('/api/recipes/:recipeId/share', () => {
    return new HttpResponse(null, { status: 204 })
  }),

  http.delete('/api/recipes/:recipeId/shares/:shareId', () => {
    return new HttpResponse(null, { status: 204 })
  }),
]
