import type { AuthUser } from '../types/auth'

interface JwtPayload {
  sub: string
  email: string
  displayName?: string
  exp: number
  iss?: string
  aud?: string
}

export function decodeJwt(token: string): JwtPayload | null {
  try {
    const parts = token.split('.')
    if (parts.length !== 3) return null
    const payload = parts[1]
    const padded = payload + '='.repeat((4 - (payload.length % 4)) % 4)
    const decoded = atob(padded.replace(/-/g, '+').replace(/_/g, '/'))
    return JSON.parse(decoded) as JwtPayload
  } catch {
    return null
  }
}

export function userFromToken(token: string): AuthUser | null {
  const payload = decodeJwt(token)
  if (!payload) return null
  return {
    id: Number(payload.sub),
    email: payload.email,
    displayName: payload.displayName,
  }
}

export function isTokenExpired(token: string): boolean {
  const payload = decodeJwt(token)
  if (!payload) return true
  return Date.now() >= payload.exp * 1000
}
