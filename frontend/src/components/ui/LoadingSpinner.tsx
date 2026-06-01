interface LoadingSpinnerProps {
  size?: 'sm' | 'md' | 'lg'
  className?: string
}

const sizeClasses = { sm: 'h-4 w-4', md: 'h-6 w-6', lg: 'h-10 w-10' }

export default function LoadingSpinner({ size = 'md', className = '' }: LoadingSpinnerProps) {
  return (
    <span
      role="status"
      aria-label="Loading"
      className={`inline-block animate-spin rounded-full border-2 border-current border-t-transparent text-blue-600 ${sizeClasses[size]} ${className}`}
    />
  )
}

export function FullPageSpinner() {
  return (
    <div className="flex min-h-screen items-center justify-center">
      <LoadingSpinner size="lg" />
    </div>
  )
}
