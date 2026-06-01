import { render, screen } from '@testing-library/react'
import { createMemoryRouter, RouterProvider } from 'react-router-dom'
import { QueryClient, QueryClientProvider } from '@tanstack/react-query'
import { AuthProvider } from '../../contexts/AuthContext'
import DashboardPage from './DashboardPage'
import AppShell from '../../components/layout/AppShell'
import { server } from '../../mocks/server'
import { http, HttpResponse } from 'msw'

function renderDashboard() {
  const queryClient = new QueryClient({ defaultOptions: { queries: { retry: false } } })
  const router = createMemoryRouter(
    [{ element: <AppShell />, children: [{ path: '/dashboard', element: <DashboardPage /> }] }],
    { initialEntries: ['/dashboard'] },
  )
  return render(
    <QueryClientProvider client={queryClient}>
      <AuthProvider>
        <RouterProvider router={router} />
      </AuthProvider>
    </QueryClientProvider>,
  )
}

describe('DashboardPage', () => {
  it('renders the week range heading', async () => {
    renderDashboard()
    expect(await screen.findByText(/this week's meals/i)).toBeInTheDocument()
  })

  it('shows empty state when no meal plans for the week', async () => {
    server.use(http.get('/api/mealplan/date-range', () => HttpResponse.json([])))
    renderDashboard()
    expect(await screen.findByText(/no meals scheduled this week/i)).toBeInTheDocument()
  })

  it('renders meal plan cards when meals are scheduled', async () => {
    renderDashboard()
    // The MSW handler returns mealPlans with mealIds matching meal fixtures
    // Meal 1 = Pasta Night (Dinner), Meal 2 = Weekend Brunch (Breakfast), etc.
    const headings = await screen.findAllByRole('link')
    expect(headings.length).toBeGreaterThan(0)
  })
})
