import type { HTMLAttributes, ReactNode } from 'react'
import clsx from 'clsx'

interface CardProps extends HTMLAttributes<HTMLDivElement> {
  children: ReactNode
  padding?: 'sm' | 'md' | 'lg' | 'none'
}

const paddingClasses = { none: '', sm: 'p-3', md: 'p-5', lg: 'p-8' }

export default function Card({ children, padding = 'md', className, ...props }: CardProps) {
  return (
    <div
      className={clsx(
        'rounded-xl border border-gray-200 bg-white shadow-sm',
        paddingClasses[padding],
        className,
      )}
      {...props}
    >
      {children}
    </div>
  )
}
