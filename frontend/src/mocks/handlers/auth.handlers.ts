import { http, HttpResponse } from 'msw'
import { authFixtures } from '../fixtures/auth.fixtures'

export const authHandlers = [
  http.post('/api/auth/register', async ({ request }) => {
    const body = await request.json() as { email: string; password: string; displayName?: string }
    return HttpResponse.json({
      ...authFixtures.loginResponse,
      // reflect display name from registration if provided
      displayName: (body as { displayName?: string }).displayName,
    }, { status: 201 })
  }),

  http.post('/api/auth/login', async ({ request }) => {
    const body = await request.json() as { email: string; password: string }
    if (body.email === 'wrong@example.com') {
      return HttpResponse.json({ message: 'Invalid credentials.' }, { status: 401 })
    }
    return HttpResponse.json(authFixtures.loginResponse)
  }),

  http.post('/api/auth/refresh', () => {
    return HttpResponse.json(authFixtures.loginResponse)
  }),

  http.post('/api/auth/logout', () => {
    return new HttpResponse(null, { status: 204 })
  }),

  http.post('/api/auth/validate', () => {
    return HttpResponse.json({ valid: true })
  }),

  http.get('/api/auth/users/search', ({ request }) => {
    const q = new URL(request.url).searchParams.get('q') ?? ''
    const users = [
      { id: 2, email: 'alice@example.com', displayName: 'Alice' },
      { id: 3, email: 'bob@example.com', displayName: 'Bob' },
    ]
    const results = q ? users.filter(u => u.email.includes(q) || u.displayName?.toLowerCase().includes(q.toLowerCase())) : []
    return HttpResponse.json(results)
  }),
]
