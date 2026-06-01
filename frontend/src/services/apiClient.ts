import type { ApiError } from '../types/api'

let accessToken: string | null = null
let refreshTokenValue: string | null = null
let onUnauthorized: (() => void) | null = null

export function setAccessToken(token: string | null): void {
  accessToken = token
}

export function setRefreshTokenRef(token: string | null): void {
  refreshTokenValue = token
}

export function setUnauthorizedHandler(handler: () => void): void {
  onUnauthorized = handler
}

async function refreshAndRetry(path: string, options: RequestInit): Promise<Response> {
  if (!refreshTokenValue) {
    onUnauthorized?.()
    throw createApiError(401, 'Session expired. Please log in again.')
  }

  try {
    const refreshRes = await fetch('/api/auth/refresh', {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify({ refreshToken: refreshTokenValue }),
    })

    if (!refreshRes.ok) {
      accessToken = null
      refreshTokenValue = null
      onUnauthorized?.()
      throw createApiError(401, 'Session expired. Please log in again.')
    }

    const data = await refreshRes.json() as { accessToken: string }
    accessToken = data.accessToken

    const retryHeaders = new Headers(options.headers)
    retryHeaders.set('Authorization', `Bearer ${accessToken}`)
    const retryResponse = await fetch(path, { ...options, headers: retryHeaders })
    return retryResponse
  } catch (err) {
    if (err instanceof Error && 'status' in err) throw err
    onUnauthorized?.()
    throw createApiError(401, 'Session expired. Please log in again.')
  }
}

function createApiError(status: number, message: string, detail?: string): ApiError & Error {
  const err = new Error(message) as ApiError & Error
  err.status = status
  err.message = message
  err.detail = detail
  return err
}

export async function apiFetch<T>(path: string, options: RequestInit = {}): Promise<T> {
  const headers = new Headers(options.headers)
  headers.set('Content-Type', 'application/json')
  if (accessToken) {
    headers.set('Authorization', `Bearer ${accessToken}`)
  }

  const response = await fetch(path, { ...options, headers })

  if (response.status === 401) {
    const retryResponse = await refreshAndRetry(path, { ...options, headers })
    if (retryResponse.status === 401) {
      accessToken = null
      onUnauthorized?.()
      throw createApiError(401, 'Session expired. Please log in again.')
    }
    if (retryResponse.status === 204) return undefined as T
    return retryResponse.json() as Promise<T>
  }

  if (!response.ok) {
    let message = `Request failed with status ${response.status}`
    let detail: string | undefined
    try {
      const body = await response.json() as { message?: string; detail?: string; title?: string }
      message = body.message ?? body.title ?? message
      detail = body.detail
    } catch {
      // ignore parse errors
    }
    throw createApiError(response.status, message, detail)
  }

  if (response.status === 204) return undefined as T
  return response.json() as Promise<T>
}
