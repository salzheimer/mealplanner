import { createContext, useContext, useEffect, useState, type ReactNode } from 'react'
import type { AuthUser } from '../types/auth'
import * as authService from '../services/authService'
import { setAccessToken, setRefreshTokenRef, setUnauthorizedHandler } from '../services/apiClient'
import { userFromToken } from '../utils/tokenUtils'

const REFRESH_TOKEN_KEY = 'meal_planner_refresh_token'

interface AuthState {
  user: AuthUser | null
  isLoading: boolean
  isAuthenticated: boolean
}

interface AuthContextValue extends AuthState {
  login: (email: string, password: string) => Promise<void>
  register: (email: string, password: string, displayName?: string) => Promise<void>
  logout: () => Promise<void>
}

const AuthContext = createContext<AuthContextValue | null>(null)

export function AuthProvider({ children }: { children: ReactNode }) {
  const [state, setState] = useState<AuthState>({
    user: null,
    isLoading: true,
    isAuthenticated: false,
  })

  useEffect(() => {
    setUnauthorizedHandler(() => {
      localStorage.removeItem(REFRESH_TOKEN_KEY)
      setAccessToken(null)
      setRefreshTokenRef(null)
      setState({ user: null, isLoading: false, isAuthenticated: false })
    })

    const storedRefresh = localStorage.getItem(REFRESH_TOKEN_KEY)
    if (!storedRefresh) {
      setState(s => ({ ...s, isLoading: false }))
      return
    }

    authService
      .refreshToken({ refreshToken: storedRefresh })
      .then(data => {
        setAccessToken(data.accessToken)
        setRefreshTokenRef(data.refreshToken)
        localStorage.setItem(REFRESH_TOKEN_KEY, data.refreshToken)
        const user = userFromToken(data.accessToken)
        setState({ user, isLoading: false, isAuthenticated: true })
      })
      .catch(() => {
        localStorage.removeItem(REFRESH_TOKEN_KEY)
        setState({ user: null, isLoading: false, isAuthenticated: false })
      })
  }, [])

  async function login(email: string, password: string): Promise<void> {
    const data = await authService.login({ email, password })
    setAccessToken(data.accessToken)
    setRefreshTokenRef(data.refreshToken)
    localStorage.setItem(REFRESH_TOKEN_KEY, data.refreshToken)
    const user = userFromToken(data.accessToken)
    setState({ user, isLoading: false, isAuthenticated: true })
  }

  async function register(email: string, password: string, displayName?: string): Promise<void> {
    const data = await authService.register({ email, password, displayName })
    setAccessToken(data.accessToken)
    setRefreshTokenRef(data.refreshToken)
    localStorage.setItem(REFRESH_TOKEN_KEY, data.refreshToken)
    const user = userFromToken(data.accessToken)
    setState({ user, isLoading: false, isAuthenticated: true })
  }

  async function logout(): Promise<void> {
    const storedRefresh = localStorage.getItem(REFRESH_TOKEN_KEY)
    if (storedRefresh) {
      await authService.logout(storedRefresh).catch(() => undefined)
    }
    setAccessToken(null)
    setRefreshTokenRef(null)
    localStorage.removeItem(REFRESH_TOKEN_KEY)
    setState({ user: null, isLoading: false, isAuthenticated: false })
  }

  return (
    <AuthContext.Provider value={{ ...state, login, register, logout }}>
      {children}
    </AuthContext.Provider>
  )
}

export function useAuth(): AuthContextValue {
  const ctx = useContext(AuthContext)
  if (!ctx) throw new Error('useAuth must be used within AuthProvider')
  return ctx
}
