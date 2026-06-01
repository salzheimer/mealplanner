import { Component, type ReactNode } from 'react'

interface Props {
  children: ReactNode
  fallback?: ReactNode
}

interface State {
  hasError: boolean
  error: Error | null
}

export default class ErrorBoundary extends Component<Props, State> {
  constructor(props: Props) {
    super(props)
    this.state = { hasError: false, error: null }
  }

  static getDerivedStateFromError(error: Error): State {
    return { hasError: true, error }
  }

  render() {
    if (this.state.hasError) {
      return this.props.fallback ?? (
        <div className="flex min-h-[200px] flex-col items-center justify-center p-8 text-center">
          <p className="text-lg font-semibold text-gray-900">Something went wrong</p>
          <p className="mt-1 text-sm text-gray-500">{this.state.error?.message ?? 'An unexpected error occurred.'}</p>
          <button
            onClick={() => this.setState({ hasError: false, error: null })}
            className="mt-4 text-sm text-blue-600 hover:underline"
          >
            Try again
          </button>
        </div>
      )
    }
    return this.props.children
  }
}
