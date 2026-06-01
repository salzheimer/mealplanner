import { render, screen, waitFor } from '@testing-library/react'
import userEvent from '@testing-library/user-event'
import { createMemoryRouter, RouterProvider } from 'react-router-dom'
import { QueryClient, QueryClientProvider } from '@tanstack/react-query'
import { AuthProvider } from '../../contexts/AuthContext'
import PlanListPage from './PlanListPage'
import PlanDetailPage from './PlanDetailPage'
import PlanFormPage from './PlanFormPage'
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
      { path: '/plans', element: <PlanListPage /> },
      { path: '/plans/new', element: <PlanFormPage /> },
      { path: '/plans/:id', element: <PlanDetailPage /> },
      { path: '/plans/:id/edit', element: <PlanFormPage /> },
    ],
  },
]

describe('PlanListPage', () => {
  it('renders plans from fixture', async () => {
    renderWithRouter(appRoutes, ['/plans'])
    expect(await screen.findByText('Week of May 19')).toBeInTheDocument()
    expect(screen.getByText('Family BBQ Weekend')).toBeInTheDocument()
  })

  it('shows My plans and Shared with me tabs', async () => {
    renderWithRouter(appRoutes, ['/plans'])
    await screen.findByText('Week of May 19')
    expect(screen.getByRole('button', { name: /my plans/i })).toBeInTheDocument()
    expect(screen.getByRole('button', { name: /shared with me/i })).toBeInTheDocument()
  })

  it('shows empty state when no plans', async () => {
    server.use(http.get('/api/plans', () => HttpResponse.json([])))
    renderWithRouter(appRoutes, ['/plans'])
    expect(await screen.findByText(/no plans yet/i)).toBeInTheDocument()
  })
})

describe('PlanDetailPage', () => {
  it('renders plan name and calendar', async () => {
    renderWithRouter(appRoutes, ['/plans/1'])
    expect(await screen.findByRole('heading', { name: 'Week of May 19' })).toBeInTheDocument()
    expect(await screen.findByText('Calendar')).toBeInTheDocument()
  })

  it('shows error when plan not found', async () => {
    server.use(http.get('/api/plans/:id', () => HttpResponse.json({ message: 'Not found' }, { status: 404 })))
    renderWithRouter(appRoutes, ['/plans/999'])
    expect(await screen.findByText(/plan not found/i)).toBeInTheDocument()
  })
})

describe('PlanFormPage (create)', () => {
  it('renders create form with date fields', async () => {
    renderWithRouter(appRoutes, ['/plans/new'])
    expect(await screen.findByRole('heading', { name: /new plan/i })).toBeInTheDocument()
    expect(screen.getByLabelText(/start date/i)).toBeInTheDocument()
  })

  it('shows validation error for missing start date', async () => {
    renderWithRouter(appRoutes, ['/plans/new'])
    await screen.findByRole('heading', { name: /new plan/i })
    await userEvent.click(screen.getByRole('button', { name: /create plan/i }))
    expect(await screen.findByText('Start date is required')).toBeInTheDocument()
  })

  it('creates plan and navigates away', async () => {
    renderWithRouter(appRoutes, ['/plans/new'])
    await screen.findByRole('heading', { name: /new plan/i })
    const startInput = screen.getByLabelText(/start date/i)
    await userEvent.type(startInput, '2026-06-01')
    await userEvent.click(screen.getByRole('button', { name: /create plan/i }))
    await waitFor(() => {
      expect(screen.queryByRole('heading', { name: /new plan/i })).not.toBeInTheDocument()
    })
  })
})
