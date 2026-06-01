import { http, HttpResponse } from 'msw'
import { planFixtures } from '../fixtures/plan.fixtures'
import type { CreatePlan, UpdatePlan, CreateMealPlan, UpdateMealPlan, CreateMealItemPlan, UpdateMealItemPlan, CreatePlanShare, UpdatePlanShare } from '../../types/plan'

export const planHandlers = [
  // Plans
  http.get('/api/plans', () => {
    return HttpResponse.json(planFixtures.list)
  }),

  http.get('/api/plans/shared-with-me', () => {
    return HttpResponse.json([])
  }),

  http.get('/api/plans/plans-shared-by-user', () => {
    return HttpResponse.json([])
  }),

  http.get('/api/plans/start-date', ({ request }) => {
    const url = new URL(request.url)
    const startDate = url.searchParams.get('startDate') ?? ''
    const filtered = planFixtures.list.filter(p => p.startDate >= startDate)
    return HttpResponse.json(filtered)
  }),

  http.get('/api/plans/end-date', ({ request }) => {
    const url = new URL(request.url)
    const endDate = url.searchParams.get('endDate') ?? ''
    const filtered = planFixtures.list.filter(p => !p.endDate || p.endDate <= endDate)
    return HttpResponse.json(filtered)
  }),

  http.get('/api/plans/date-range', ({ request }) => {
    const url = new URL(request.url)
    const startDate = url.searchParams.get('startDate') ?? ''
    const endDate = url.searchParams.get('endDate') ?? ''
    const filtered = planFixtures.list.filter(p => p.startDate >= startDate && (!p.endDate || p.endDate <= endDate))
    return HttpResponse.json(filtered)
  }),

  http.get('/api/plans/:id', ({ params }) => {
    return HttpResponse.json(planFixtures.detail(Number(params.id)))
  }),

  http.post('/api/plans', async ({ request }) => {
    const body = await request.json() as CreatePlan
    const newPlan = {
      ...planFixtures.list[0],
      ...body,
      id: 99,
      createdAt: new Date().toISOString(),
      updatedAt: new Date().toISOString(),
    }
    return HttpResponse.json(newPlan, { status: 201 })
  }),

  http.put('/api/plans/:planId', async ({ params, request }) => {
    const body = await request.json() as UpdatePlan
    const existing = planFixtures.detail(Number(params.planId))
    return HttpResponse.json({ ...existing, ...body, updatedAt: new Date().toISOString() })
  }),

  http.delete('/api/plans/:id', () => {
    return new HttpResponse(null, { status: 204 })
  }),

  // Plan shares
  http.get('/api/plans/:planId/shares', () => {
    return HttpResponse.json([])
  }),

  http.post('/api/plans/shares', async ({ request }) => {
    const body = await request.json() as CreatePlanShare
    return HttpResponse.json({ id: 1, ...body, createdAt: new Date().toISOString() }, { status: 201 })
  }),

  http.put('/api/plans/:planId/shares/:shareId', async ({ request }) => {
    const body = await request.json() as UpdatePlanShare
    return HttpResponse.json({ id: 1, ...body, updatedAt: new Date().toISOString() })
  }),

  http.delete('/api/plans/:planId/shares/:planShareId', () => {
    return new HttpResponse(null, { status: 204 })
  }),

  // MealPlans
  http.get('/api/mealplan/user-meal-plans', () => {
    return HttpResponse.json(planFixtures.mealPlans)
  }),

  http.get('/api/mealplan/serve-date', ({ request }) => {
    const url = new URL(request.url)
    const serveDate = url.searchParams.get('serveDate') ?? ''
    return HttpResponse.json(planFixtures.mealPlans.filter(mp => mp.serveDate === serveDate))
  }),

  http.get('/api/mealplan/date-range', ({ request }) => {
    const url = new URL(request.url)
    const startDate = url.searchParams.get('startDate') ?? ''
    const endDate = url.searchParams.get('endDate') ?? ''
    return HttpResponse.json(planFixtures.mealPlansByDateRange(startDate, endDate))
  }),

  http.get('/api/mealplan/end-date', ({ request }) => {
    const url = new URL(request.url)
    const endDate = url.searchParams.get('endDate') ?? ''
    return HttpResponse.json(planFixtures.mealPlans.filter(mp => mp.serveDate && mp.serveDate <= endDate))
  }),

  http.get('/api/mealplan/:id', ({ params }) => {
    const found = planFixtures.mealPlans.find(mp => mp.id === Number(params.id))
    if (!found) return HttpResponse.json({ message: 'Not found' }, { status: 404 })
    return HttpResponse.json(found)
  }),

  http.post('/api/mealplan', async ({ request }) => {
    const body = await request.json() as CreateMealPlan
    const newMealPlan = {
      id: 99,
      mealId: body.mealId,
      planId: body.planId,
      serveDate: body.serveDate ?? null,
      endDate: body.endDate ?? null,
      addedByUserId: 1,
      createdAt: new Date().toISOString(),
      updatedAt: new Date().toISOString(),
    }
    return HttpResponse.json(newMealPlan, { status: 201 })
  }),

  http.put('/api/mealplan', async ({ request }) => {
    const body = await request.json() as UpdateMealPlan
    const existing = planFixtures.mealPlans.find(mp => mp.id === body.id) ?? planFixtures.mealPlans[0]
    return HttpResponse.json({ ...existing, ...body, updatedAt: new Date().toISOString() })
  }),

  http.delete('/api/mealplan/:id', () => {
    return new HttpResponse(null, { status: 204 })
  }),

  // Meal item plans
  http.get('/api/mealplan/:mealPlanId/mealitems', ({ params }) => {
    return HttpResponse.json(planFixtures.mealItemPlans(Number(params.mealPlanId)))
  }),

  http.post('/api/mealplan/:mealPlanId/mealitems', async ({ params, request }) => {
    const body = await request.json() as CreateMealItemPlan
    const newItem = {
      id: 200,
      mealPlanId: Number(params.mealPlanId),
      mealItemId: body.mealItemId,
      assignedToUserId: body.assignedToUserId ?? null,
      assignedToGuestName: body.assignedToGuestName ?? null,
      status: body.status ?? 'Unknown',
      notes: body.notes ?? null,
      createdAt: new Date().toISOString(),
      updatedAt: new Date().toISOString(),
    }
    return HttpResponse.json(newItem, { status: 201 })
  }),

  http.put('/api/mealplan/:mealPlanId/mealitems/:mealItemId', async ({ params, request }) => {
    const body = await request.json() as UpdateMealItemPlan
    const existing = planFixtures.mealItemPlans(Number(params.mealPlanId)).find(i => i.id === Number(params.mealItemId))
    return HttpResponse.json({ ...existing, ...body, updatedAt: new Date().toISOString() })
  }),

  http.delete('/api/mealplan/mealitem/:mealItemPlanId', () => {
    return new HttpResponse(null, { status: 204 })
  }),
]
