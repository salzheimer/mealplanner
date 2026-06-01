import { forwardRef, type TextareaHTMLAttributes } from 'react'
import clsx from 'clsx'

interface TextAreaProps extends TextareaHTMLAttributes<HTMLTextAreaElement> {
  label?: string
  error?: string
}

const TextArea = forwardRef<HTMLTextAreaElement, TextAreaProps>(
  ({ label, error, className, id, rows = 3, ...props }, ref) => {
    const inputId = id ?? label?.toLowerCase().replace(/\s+/g, '-')
    return (
      <div className="flex flex-col gap-1">
        {label && (
          <label htmlFor={inputId} className="text-sm font-medium text-gray-700">
            {label}
          </label>
        )}
        <textarea
          ref={ref}
          id={inputId}
          rows={rows}
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
TextArea.displayName = 'TextArea'

export default TextArea
