import { useState } from 'react'
import Modal from './Modal'
import Button from './Button'
import Select from './Select'
import type { Permission, ResourcePermission } from '../../types/shared'

interface UserSearchResult {
  id: number
  email: string
  displayName?: string
}

interface ShareModalProps {
  isOpen: boolean
  onClose: () => void
  resourceName: string
  shares: ResourcePermission[]
  isLoadingShares: boolean
  onGrantShare: (subjectId: number, permission: Permission) => Promise<void>
  onRevokeShare: (shareId: number) => Promise<void>
  onSearchUsers: (query: string) => Promise<UserSearchResult[]>
}

const PERMISSION_OPTIONS: { value: Permission; label: string }[] = [
  { value: 'View', label: 'View' },
  { value: 'Comment', label: 'Comment' },
  { value: 'Edit', label: 'Edit' },
  { value: 'Manage', label: 'Manage' },
]

export default function ShareModal({
  isOpen,
  onClose,
  resourceName,
  shares,
  isLoadingShares,
  onGrantShare,
  onRevokeShare,
  onSearchUsers,
}: ShareModalProps) {
  const [searchQuery, setSearchQuery] = useState('')
  const [searchResults, setSearchResults] = useState<UserSearchResult[]>([])
  const [selectedUser, setSelectedUser] = useState<UserSearchResult | null>(null)
  const [permission, setPermission] = useState<Permission>('View')
  const [isSearching, setIsSearching] = useState(false)
  const [isGranting, setIsGranting] = useState(false)
  const [revokingId, setRevokingId] = useState<number | null>(null)
  const [error, setError] = useState<string | null>(null)

  async function handleSearch() {
    if (!searchQuery.trim()) return
    setIsSearching(true)
    setError(null)
    try {
      const results = await onSearchUsers(searchQuery.trim())
      setSearchResults(results)
      if (results.length === 0) setError('No users found with that email.')
    } catch {
      setError('User search failed.')
    } finally {
      setIsSearching(false)
    }
  }

  async function handleGrant() {
    if (!selectedUser) return
    setIsGranting(true)
    setError(null)
    try {
      await onGrantShare(selectedUser.id, permission)
      setSearchQuery('')
      setSearchResults([])
      setSelectedUser(null)
      setPermission('View')
    } catch {
      setError('Failed to share. Please try again.')
    } finally {
      setIsGranting(false)
    }
  }

  async function handleRevoke(shareId: number) {
    setRevokingId(shareId)
    try {
      await onRevokeShare(shareId)
    } finally {
      setRevokingId(null)
    }
  }

  return (
    <Modal isOpen={isOpen} onClose={onClose} title={`Share "${resourceName}"`} size="md">
      <div className="space-y-6">
        {/* Grant access section */}
        <div>
          <p className="mb-3 text-sm font-medium text-gray-700">Add people</p>
          <div className="flex gap-2">
            <input
              type="text"
              value={searchQuery}
              onChange={e => setSearchQuery(e.target.value)}
              onKeyDown={e => e.key === 'Enter' && handleSearch()}
              placeholder="Search by email"
              className="flex-1 rounded-md border border-gray-300 px-3 py-2 text-sm shadow-sm outline-none focus:border-blue-500 focus:ring-2 focus:ring-blue-500"
            />
            <Button variant="secondary" onClick={handleSearch} isLoading={isSearching}>
              Search
            </Button>
          </div>

          {searchResults.length > 0 && (
            <ul className="mt-2 rounded-md border border-gray-200 bg-white shadow-sm">
              {searchResults.map(user => (
                <li key={user.id}>
                  <button
                    type="button"
                    onClick={() => { setSelectedUser(user); setSearchResults([]) }}
                    className="w-full px-3 py-2 text-left text-sm hover:bg-gray-50"
                  >
                    <span className="font-medium">{user.displayName ?? user.email}</span>
                    {user.displayName && (
                      <span className="ml-2 text-gray-500">{user.email}</span>
                    )}
                  </button>
                </li>
              ))}
            </ul>
          )}

          {selectedUser && (
            <div className="mt-3 flex items-center gap-3 rounded-md bg-blue-50 px-3 py-2">
              <span className="flex-1 text-sm">
                <span className="font-medium">{selectedUser.displayName ?? selectedUser.email}</span>
                {selectedUser.displayName && (
                  <span className="ml-2 text-gray-500 text-xs">{selectedUser.email}</span>
                )}
              </span>
              <Select
                options={PERMISSION_OPTIONS}
                value={permission}
                onChange={e => setPermission(e.target.value as Permission)}
                className="w-28 py-1"
                aria-label="Permission level"
              />
              <Button onClick={handleGrant} isLoading={isGranting} size="sm">
                Share
              </Button>
              <button
                type="button"
                onClick={() => setSelectedUser(null)}
                aria-label="Remove selected user"
                className="text-gray-400 hover:text-gray-600"
              >
                ×
              </button>
            </div>
          )}

          {error && <p role="alert" className="mt-2 text-xs text-red-600">{error}</p>}
        </div>

        {/* Existing shares */}
        <div>
          <p className="mb-2 text-sm font-medium text-gray-700">Shared with</p>
          {isLoadingShares ? (
            <p className="text-sm text-gray-500">Loading…</p>
          ) : shares.length === 0 ? (
            <p className="text-sm text-gray-500">Not shared with anyone yet.</p>
          ) : (
            <ul className="space-y-2">
              {shares.map(share => (
                <li key={share.id} className="flex items-center justify-between text-sm">
                  <span className="text-gray-700">User #{share.subjectId}</span>
                  <div className="flex items-center gap-2">
                    <span className="text-xs text-gray-500">{share.permission}</span>
                    <Button
                      variant="ghost"
                      size="sm"
                      onClick={() => handleRevoke(share.id)}
                      isLoading={revokingId === share.id}
                    >
                      Revoke
                    </Button>
                  </div>
                </li>
              ))}
            </ul>
          )}
        </div>
      </div>
    </Modal>
  )
}
