import type { AuthUser, LoginResponse } from '../../types/auth'

export const authFixtures = {
  user: {
    id: 1,
    email: 'test@example.com',
    displayName: 'Test User',
  } satisfies AuthUser,

  loginResponse: {
    accessToken: 'eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiIxIiwiZW1haWwiOiJ0ZXN0QGV4YW1wbGUuY29tIiwianRpIjoiYWJjMTIzIiwiaXNzIjoibWVhbHBsYW5uZXIiLCJhdWQiOiJtZWFscGxhbm5lci1jbGllbnQiLCJleHAiOjk5OTk5OTk5OTl9.signature',
    refreshToken: 'mock-refresh-token-abc123',
    tokenType: 'Bearer',
    expiresInSeconds: 900,
  } satisfies LoginResponse,
}
