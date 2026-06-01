import { useAuth } from '../../contexts/AuthContext'

export default function TopBar() {
  const { user, logout } = useAuth()

  return (
    <header className="flex h-14 items-center justify-between border-b border-gray-200 bg-white px-6">
      <div />
      <div className="flex items-center gap-4">
        {user && (
          <span className="text-sm text-gray-700">
            {user.displayName ?? user.email}
          </span>
        )}
        <button
          onClick={() => void logout()}
          className="text-sm text-gray-500 hover:text-gray-900"
        >
          Sign out
        </button>
      </div>
    </header>
  )
}
