import { http, HttpResponse } from 'msw'
import { mealFixtures } from '../fixtures/meal.fixtures'
import type { CreateMeal, UpdateMeal, CreateMealItem, UpdateMealItem } from '../../types/meal'

export const mealHandlers = [
  // GET /api/meal — not yet implemented in backend; mocked here to unblock frontend development
  // TODO: remove this handler when the real GET /api/meal endpoint ships
  http.get('/api/meal', () => {
    return HttpResponse.json(mealFixtures.list)
  }),

  http.get('/api/meal/shared-with-me', () => {
    return HttpResponse.json([])
  }),

  http.get('/api/meal/:id', ({ params }) => {
    return HttpResponse.json(mealFixtures.detail(Number(params.id)))
  }),

  http.post('/api/meal', async ({ request }) => {
    const body = await request.json() as CreateMeal
    const newMeal = {
      ...mealFixtures.list[0],
      ...body,
      id: 99,
      createdAt: new Date().toISOString(),
      updatedAt: new Date().toISOString(),
    }
    return HttpResponse.json(newMeal, { status: 201 })
  }),

  http.put('/api/meal', async ({ request }) => {
    const body = await request.json() as UpdateMeal
    const existing = mealFixtures.detail(body.id)
    return HttpResponse.json({ ...existing, ...body, updatedAt: new Date().toISOString() })
  }),

  http.delete('/api/meal/:id', () => {
    return new HttpResponse(null, { status: 204 })
  }),

  http.post('/api/meal/:mealId/clone', ({ params }) => {
    const original = mealFixtures.detail(Number(params.mealId))
    return HttpResponse.json({ ...original, id: 100, name: `${original.name} (copy)` }, { status: 201 })
  }),

  http.get('/api/meal/:mealId/shares', () => {
    return HttpResponse.json([])
  }),

  http.post('/api/meal/:mealId/share', () => {
    return new HttpResponse(null, { status: 204 })
  }),

  http.delete('/api/meal/:mealId/shares/:shareId', () => {
    return new HttpResponse(null, { status: 204 })
  }),

  // Meal items
  http.get('/api/meal/:mealId/items', ({ params }) => {
    return HttpResponse.json(mealFixtures.items(Number(params.mealId)))
  }),

  http.post('/api/meal/:mealId/items', async ({ params, request }) => {
    const body = await request.json() as CreateMealItem
    const newItem = {
      id: 200,
      mealId: Number(params.mealId),
      name: body.name ?? null,
      recipeId: body.recipeId ?? null,
      itemType: body.itemType,
      createdAt: new Date().toISOString(),
      updatedAt: new Date().toISOString(),
    }
    return HttpResponse.json(newItem, { status: 201 })
  }),

  http.put('/api/meal/items', async ({ request }) => {
    const body = await request.json() as UpdateMealItem
    const allItems = [1, 2, 3, 4].flatMap(id => mealFixtures.items(id))
    const existing = allItems.find(i => i.id === body.id) ?? allItems[0]
    return HttpResponse.json({ ...existing, ...body, updatedAt: new Date().toISOString() })
  }),

  http.delete('/api/meal/items/:mealItemId', () => {
    return new HttpResponse(null, { status: 204 })
  }),
]
