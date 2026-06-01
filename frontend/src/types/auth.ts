export interface AuthUser {
  id: number
  email: string
  displayName?: string
}

export interface LoginRequest {
  email: string
  password: string
}

export interface RegisterRequest {
  email: string
  password: string
  displayName?: string
}

export interface LoginResponse {
  accessToken: string
  refreshToken: string
  tokenType: string
  expiresInSeconds: number
}

export interface RefreshRequest {
  refreshToken: string
}
