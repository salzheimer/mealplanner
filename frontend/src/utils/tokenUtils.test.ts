import { decodeJwt, userFromToken, isTokenExpired } from './tokenUtils'
import { authFixtures } from '../mocks/fixtures/auth.fixtures'

describe('tokenUtils', () => {
  describe('decodeJwt', () => {
    it('decodes the mock access token correctly', () => {
      const payload = decodeJwt(authFixtures.loginResponse.accessToken)
      expect(payload).not.toBeNull()
      expect(payload?.sub).toBe('1')
      expect(payload?.email).toBe('test@example.com')
    })

    it('returns null for invalid token', () => {
      expect(decodeJwt('not.a.token')).toBeNull()
      expect(decodeJwt('')).toBeNull()
    })
  })

  describe('userFromToken', () => {
    it('extracts user from valid token', () => {
      const user = userFromToken(authFixtures.loginResponse.accessToken)
      expect(user).toEqual({ id: 1, email: 'test@example.com', displayName: undefined })
    })

    it('returns null for invalid token', () => {
      expect(userFromToken('bad')).toBeNull()
    })
  })

  describe('isTokenExpired', () => {
    it('returns false for the far-future mock token', () => {
      expect(isTokenExpired(authFixtures.loginResponse.accessToken)).toBe(false)
    })

    it('returns true for invalid token', () => {
      expect(isTokenExpired('invalid')).toBe(true)
    })
  })
})
