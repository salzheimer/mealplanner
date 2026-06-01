import { render, screen } from '@testing-library/react'
import userEvent from '@testing-library/user-event'
import Modal from './Modal'
import ConfirmDialog from './ConfirmDialog'
import Badge from './Badge'
import EmptyState from './EmptyState'
import Button from './Button'
import type { MealType, ItemType, ItemStatus } from '../../types/shared'

// ── Modal ────────────────────────────────────────────────────────────────────

describe('Modal', () => {
  it('renders nothing when closed', () => {
    render(<Modal isOpen={false} onClose={() => undefined} title="Test"><p>content</p></Modal>)
    expect(screen.queryByRole('dialog')).not.toBeInTheDocument()
  })

  it('renders title and children when open', () => {
    render(<Modal isOpen onClose={() => undefined} title="My Modal"><p>Modal body</p></Modal>)
    expect(screen.getByRole('dialog')).toBeInTheDocument()
    expect(screen.getByText('My Modal')).toBeInTheDocument()
    expect(screen.getByText('Modal body')).toBeInTheDocument()
  })

  it('calls onClose when Escape is pressed', async () => {
    const onClose = vi.fn()
    render(<Modal isOpen onClose={onClose} title="Test"><p>content</p></Modal>)
    await userEvent.keyboard('{Escape}')
    expect(onClose).toHaveBeenCalledTimes(1)
  })

  it('calls onClose when close button is clicked', async () => {
    const onClose = vi.fn()
    render(<Modal isOpen onClose={onClose} title="Test"><p>content</p></Modal>)
    await userEvent.click(screen.getByRole('button', { name: /close/i }))
    expect(onClose).toHaveBeenCalledTimes(1)
  })

  it('calls onClose when backdrop is clicked', async () => {
    const onClose = vi.fn()
    render(<Modal isOpen onClose={onClose} title="Test"><p>content</p></Modal>)
    // Click the backdrop div (aria-hidden overlay)
    const backdrop = document.querySelector('[aria-hidden="true"]') as HTMLElement
    await userEvent.click(backdrop)
    expect(onClose).toHaveBeenCalledTimes(1)
  })

  it('does not call onClose for Escape when closed', async () => {
    const onClose = vi.fn()
    render(<Modal isOpen={false} onClose={onClose} title="Test"><p>content</p></Modal>)
    await userEvent.keyboard('{Escape}')
    expect(onClose).not.toHaveBeenCalled()
  })
})

// ── ConfirmDialog ─────────────────────────────────────────────────────────────

describe('ConfirmDialog', () => {
  it('renders title, message, and both buttons', () => {
    render(
      <ConfirmDialog
        isOpen
        onClose={() => undefined}
        onConfirm={() => undefined}
        title="Delete item?"
        message="This cannot be undone."
      />,
    )
    expect(screen.getByText('Delete item?')).toBeInTheDocument()
    expect(screen.getByText('This cannot be undone.')).toBeInTheDocument()
    expect(screen.getByRole('button', { name: /cancel/i })).toBeInTheDocument()
    expect(screen.getByRole('button', { name: /confirm/i })).toBeInTheDocument()
  })

  it('calls onConfirm when confirm button is clicked', async () => {
    const onConfirm = vi.fn()
    render(
      <ConfirmDialog
        isOpen
        onClose={() => undefined}
        onConfirm={onConfirm}
        title="Test"
        message="Are you sure?"
      />,
    )
    await userEvent.click(screen.getByRole('button', { name: /confirm/i }))
    expect(onConfirm).toHaveBeenCalledTimes(1)
  })

  it('calls onClose when cancel button is clicked', async () => {
    const onClose = vi.fn()
    render(
      <ConfirmDialog
        isOpen
        onClose={onClose}
        onConfirm={() => undefined}
        title="Test"
        message="Are you sure?"
      />,
    )
    await userEvent.click(screen.getByRole('button', { name: /cancel/i }))
    expect(onClose).toHaveBeenCalledTimes(1)
  })

  it('uses custom confirmLabel', () => {
    render(
      <ConfirmDialog
        isOpen
        onClose={() => undefined}
        onConfirm={() => undefined}
        title="Test"
        message="Test"
        confirmLabel="Delete forever"
      />,
    )
    expect(screen.getByRole('button', { name: /delete forever/i })).toBeInTheDocument()
  })

  it('closes on Escape key', async () => {
    const onClose = vi.fn()
    render(
      <ConfirmDialog
        isOpen
        onClose={onClose}
        onConfirm={() => undefined}
        title="Test"
        message="Test"
      />,
    )
    await userEvent.keyboard('{Escape}')
    expect(onClose).toHaveBeenCalledTimes(1)
  })
})

// ── Badge ─────────────────────────────────────────────────────────────────────

const mealTypes: MealType[] = ['Breakfast', 'Lunch', 'Dinner', 'Snack']
const itemTypes: ItemType[] = ['Recipe', 'Homemade', 'StoreBought']
const itemStatuses: ItemStatus[] = ['Unknown', 'Pending', 'Confirmed']

describe('Badge', () => {
  it('renders the label', () => {
    render(<Badge label="Breakfast" variant="Breakfast" />)
    expect(screen.getByText('Breakfast')).toBeInTheDocument()
  })

  it.each(mealTypes)('MealType %s renders a non-empty className', type => {
    const { container } = render(<Badge label={type} variant={type} />)
    const span = container.querySelector('span')!
    expect(span.className.length).toBeGreaterThan(0)
    expect(span.className).not.toContain('undefined')
  })

  it.each(itemTypes)('ItemType %s renders a non-empty className', type => {
    const { container } = render(<Badge label={type} variant={type} />)
    const span = container.querySelector('span')!
    expect(span.className.length).toBeGreaterThan(0)
  })

  it.each(itemStatuses)('ItemStatus %s renders a non-empty className', status => {
    const { container } = render(<Badge label={status} variant={status} />)
    const span = container.querySelector('span')!
    expect(span.className.length).toBeGreaterThan(0)
  })

  it('renders with default variant when none provided', () => {
    const { container } = render(<Badge label="Custom" />)
    const span = container.querySelector('span')!
    expect(span.className).toContain('bg-gray-100')
  })
})

// ── EmptyState ────────────────────────────────────────────────────────────────

describe('EmptyState', () => {
  it('renders heading', () => {
    render(<EmptyState heading="No recipes yet" />)
    expect(screen.getByRole('heading', { name: 'No recipes yet' })).toBeInTheDocument()
  })

  it('renders description when provided', () => {
    render(<EmptyState heading="No recipes yet" description="Create your first recipe to get started." />)
    expect(screen.getByText('Create your first recipe to get started.')).toBeInTheDocument()
  })

  it('does not render description when omitted', () => {
    render(<EmptyState heading="No recipes yet" />)
    expect(screen.queryByText(/create/i)).not.toBeInTheDocument()
  })

  it('renders action CTA when provided', () => {
    render(
      <EmptyState
        heading="No recipes"
        action={<Button>New Recipe</Button>}
      />,
    )
    expect(screen.getByRole('button', { name: /new recipe/i })).toBeInTheDocument()
  })

  it('does not render action slot when omitted', () => {
    render(<EmptyState heading="No recipes" />)
    expect(screen.queryByRole('button')).not.toBeInTheDocument()
  })
})
