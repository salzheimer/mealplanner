import { forwardRef, type InputHTMLAttributes } from 'react'
import clsx from 'clsx'

interface InputProps extends InputHTMLAttributes<HTMLInputElement> {
  label?: string
  error?: string
}

const Input = forwardRef<HTMLInputElement, InputProps>(
  ({ label, error, className, id, ...props }, ref) => {
    const inputId = id ?? label?.toLowerCase().replace(/\s+/g, '-')
    return (
      <div className="flex flex-col gap-1">
        {label && (
          <label htmlFor={inputId} className="text-sm font-medium text-gray-700">
            {label}
          </label>
        )}
        <input
          ref={ref}
          id={inputId}
          aria-invalid={error ? true : undefined}
          aria-describedby={error ? `${inputId}-error` : undefined}
          className={clsx(
            'rounded-md border px-3 py-2 text-sm shadow-sm outline-none transition',
            'focus:border-blue-500 focus:ring-2 focus:ring-blue-500',
            error
              ? 'border-red-400 focus:border-red-400 focus:ring-red-400'
              : 'border-gray-300',
            className,
          )}
          {...props}
        />
        {error && (
          <p id={`${inputId}-error`} role="alert" className="text-xs text-red-600">
            {error}
          </p>
        )}
      </div>
    )
  },
)
Input.displayName = 'Input'

export default Input
