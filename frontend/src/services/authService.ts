import type { LoginRequest, LoginResponse, RegisterRequest, RefreshRequest } from '../types/auth'

export async function login(data: LoginRequest): Promise<LoginResponse> {
  const response = await fetch('/api/auth/login', {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify(data),
  })

  if (!response.ok) {
    const body = await response.json().catch(() => ({})) as { message?: string }
    throw new Error(body.message ?? 'Login failed.')
  }

  return response.json() as Promise<LoginResponse>
}

export async function register(data: RegisterRequest): Promise<LoginResponse> {
  const response = await fetch('/api/auth/register', {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify(data),
  })

  if (!response.ok) {
    const body = await response.json().catch(() => ({})) as { message?: string }
    throw new Error(body.message ?? 'Registration failed.')
  }

  return response.json() as Promise<LoginResponse>
}

export async function refreshToken(data: RefreshRequest): Promise<LoginResponse> {
  const response = await fetch('/api/auth/refresh', {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify(data),
  })

  if (!response.ok) {
    throw new Error('Token refresh failed.')
  }

  return response.json() as Promise<LoginResponse>
}

export async function logout(refreshTkn: string): Promise<void> {
  await fetch('/api/auth/logout', {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify({ refreshToken: refreshTkn }),
  })
}
