import { recipeFixtures } from './fixtures/recipe.fixtures'
import { mealFixtures } from './fixtures/meal.fixtures'
import { planFixtures } from './fixtures/plan.fixtures'

describe('MSW fixtures type safety', () => {
  it('recipe fixture list contains expected items', () => {
    expect(recipeFixtures.list).toHaveLength(3)
    expect(recipeFixtures.list[0].name).toBe('Pasta Primavera')
  })

  it('recipe fixture detail returns by id', () => {
    const detail = recipeFixtures.detail(1)
    expect(detail.id).toBe(1)
    expect(detail.ingredients.length).toBeGreaterThan(0)
    expect(detail.instructions.length).toBeGreaterThan(0)
  })

  it('recipe fixture detail falls back to first item for unknown id', () => {
    expect(recipeFixtures.detail(9999).id).toBe(1)
  })

  it('meal fixture list covers all MealType values', () => {
    const types = mealFixtures.list.map(m => m.mealType)
    expect(types).toContain('Breakfast')
    expect(types).toContain('Lunch')
    expect(types).toContain('Dinner')
  })

  it('meal fixture items returns correct items for a meal', () => {
    const items = mealFixtures.items(1)
    expect(items.length).toBeGreaterThan(0)
    expect(items.every(i => i.mealId === 1)).toBe(true)
  })

  it('plan fixture date range filter works', () => {
    const results = planFixtures.mealPlansByDateRange('2026-05-20', '2026-05-21')
    expect(results.every(mp => mp.serveDate! >= '2026-05-20' && mp.serveDate! <= '2026-05-21')).toBe(true)
  })
})

describe('MSW server integration', () => {
  it('handles GET /api/recipes via mock server', async () => {
    const response = await fetch('/api/recipes')
    expect(response.ok).toBe(true)
    const data = await response.json() as unknown[]
    expect(Array.isArray(data)).toBe(true)
    expect(data.length).toBe(3)
  })

  it('handles GET /api/meal (mock-only endpoint)', async () => {
    const response = await fetch('/api/meal')
    expect(response.ok).toBe(true)
    const data = await response.json() as unknown[]
    expect(Array.isArray(data)).toBe(true)
    expect(data.length).toBeGreaterThan(0)
  })

  it('handles POST /api/auth/login', async () => {
    const response = await fetch('/api/auth/login', {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify({ email: 'test@example.com', password: 'password' }),
    })
    expect(response.ok).toBe(true)
    const data = await response.json() as { accessToken: string }
    expect(data.accessToken).toBeDefined()
  })

  it('returns 401 for wrong credentials', async () => {
    const response = await fetch('/api/auth/login', {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify({ email: 'wrong@example.com', password: 'bad' }),
    })
    expect(response.status).toBe(401)
  })

  it('handles DELETE returning 204', async () => {
    const response = await fetch('/api/recipes/1', { method: 'DELETE' })
    expect(response.status).toBe(204)
  })
})
