export default function UnauthorizedPage() {
  return (
    <div className="flex min-h-screen items-center justify-center">
      <div className="text-center">
        <h1 className="text-4xl font-bold text-gray-900">401</h1>
        <p className="mt-2 text-gray-600">You are not authorized to view this page.</p>
        <a href="/login" className="mt-4 inline-block text-blue-600 hover:underline">
          Sign in
        </a>
      </div>
    </div>
  )
}
