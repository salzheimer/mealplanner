import { render, screen, waitFor } from '@testing-library/react'
import userEvent from '@testing-library/user-event'
import { createMemoryRouter, RouterProvider } from 'react-router-dom'
import { QueryClient, QueryClientProvider } from '@tanstack/react-query'
import { AuthProvider } from '../../contexts/AuthContext'
import MealListPage from './MealListPage'
import MealDetailPage from './MealDetailPage'
import MealFormPage from './MealFormPage'
import AppShell from '../../components/layout/AppShell'
import { server } from '../../mocks/server'
import { http, HttpResponse } from 'msw'

function renderWithRouter(routes: Parameters<typeof createMemoryRouter>[0], initialEntries: string[]) {
  const queryClient = new QueryClient({ defaultOptions: { queries: { retry: false } } })
  const router = createMemoryRouter(routes, { initialEntries })
  return render(
    <QueryClientProvider client={queryClient}>
      <AuthProvider>
        <RouterProvider router={router} />
      </AuthProvider>
    </QueryClientProvider>,
  )
}

const appRoutes = [
  {
    element: <AppShell />,
    children: [
      { path: '/meals', element: <MealListPage /> },
      { path: '/meals/new', element: <MealFormPage /> },
      { path: '/meals/:id', element: <MealDetailPage /> },
      { path: '/meals/:id/edit', element: <MealFormPage /> },
    ],
  },
]

describe('MealListPage', () => {
  it('renders all meals from fixture', async () => {
    renderWithRouter(appRoutes, ['/meals'])
    expect(await screen.findByText('Pasta Night')).toBeInTheDocument()
    expect(screen.getByText('Weekend Brunch')).toBeInTheDocument()
    expect(screen.getByText('Taco Tuesday')).toBeInTheDocument()
    expect(screen.getByText('Soup & Salad')).toBeInTheDocument()
  })

  it('shows type filter pills', async () => {
    renderWithRouter(appRoutes, ['/meals'])
    await screen.findByText('Pasta Night')
    expect(screen.getByRole('button', { name: 'Breakfast' })).toBeInTheDocument()
    expect(screen.getByRole('button', { name: 'Dinner' })).toBeInTheDocument()
    expect(screen.getByRole('button', { name: 'Lunch' })).toBeInTheDocument()
  })

  it('filters meals by type when pill clicked', async () => {
    renderWithRouter(appRoutes, ['/meals'])
    await screen.findByText('Pasta Night')
    await userEvent.click(screen.getByRole('button', { name: 'Breakfast' }))
    expect(screen.getByText('Weekend Brunch')).toBeInTheDocument()
    expect(screen.queryByText('Pasta Night')).not.toBeInTheDocument()
  })

  it('shows empty state when no meals', async () => {
    server.use(http.get('/api/meal', () => HttpResponse.json([])))
    renderWithRouter(appRoutes, ['/meals'])
    expect(await screen.findByText(/no meals yet/i)).toBeInTheDocument()
  })
})

describe('MealDetailPage', () => {
  it('renders meal name, type badge, and items', async () => {
    renderWithRouter(appRoutes, ['/meals/1'])
    expect(await screen.findByRole('heading', { name: 'Pasta Night' })).toBeInTheDocument()
    expect(await screen.findByText('Pasta Primavera')).toBeInTheDocument()
    expect(screen.getByText('Garlic bread')).toBeInTheDocument()
  })

  it('shows error when meal not found', async () => {
    server.use(http.get('/api/meal/:id', () => HttpResponse.json({ message: 'Not found' }, { status: 404 })))
    renderWithRouter(appRoutes, ['/meals/999'])
    expect(await screen.findByText(/meal not found/i)).toBeInTheDocument()
  })
})

describe('MealFormPage (create)', () => {
  it('renders create form with meal type select', async () => {
    renderWithRouter(appRoutes, ['/meals/new'])
    expect(await screen.findByRole('heading', { name: /new meal/i })).toBeInTheDocument()
    expect(screen.getByLabelText(/meal name/i)).toBeInTheDocument()
    expect(screen.getByLabelText(/meal type/i)).toBeInTheDocument()
  })

  it('shows validation error for missing name', async () => {
    renderWithRouter(appRoutes, ['/meals/new'])
    await screen.findByRole('heading', { name: /new meal/i })
    await userEvent.click(screen.getByRole('button', { name: /create meal/i }))
    expect(await screen.findByText('Meal name is required')).toBeInTheDocument()
  })

  it('creates meal and navigates away', async () => {
    renderWithRouter(appRoutes, ['/meals/new'])
    await screen.findByRole('heading', { name: /new meal/i })
    await userEvent.type(screen.getByLabelText(/meal name/i), 'Sunday Roast')
    await userEvent.click(screen.getByRole('button', { name: /create meal/i }))
    await waitFor(() => {
      expect(screen.queryByRole('heading', { name: /new meal/i })).not.toBeInTheDocument()
    })
  })
})
