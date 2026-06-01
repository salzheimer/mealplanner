import { render, screen, waitFor } from '@testing-library/react'
import userEvent from '@testing-library/user-event'
import { createMemoryRouter, RouterProvider } from 'react-router-dom'
import { QueryClient, QueryClientProvider } from '@tanstack/react-query'
import { AuthProvider } from '../../contexts/AuthContext'
import RecipeListPage from './RecipeListPage'
import RecipeDetailPage from './RecipeDetailPage'
import RecipeFormPage from './RecipeFormPage'
import AppShell from '../../components/layout/AppShell'
import { server } from '../../mocks/server'
import { http, HttpResponse } from 'msw'
import { recipeFixtures } from '../../mocks/fixtures/recipe.fixtures'

function renderWithRouter(routes: Parameters<typeof createMemoryRouter>[0], initialEntries: string[]) {
  const queryClient = new QueryClient({ defaultOptions: { queries: { retry: false } } })
  const router = createMemoryRouter(routes, { initialEntries })
  return {
    queryClient,
    ...render(
      <QueryClientProvider client={queryClient}>
        <AuthProvider>
          <RouterProvider router={router} />
        </AuthProvider>
      </QueryClientProvider>,
    ),
  }
}

const appRoutes = [
  {
    element: <AppShell />,
    children: [
      { path: '/recipes', element: <RecipeListPage /> },
      { path: '/recipes/new', element: <RecipeFormPage /> },
      { path: '/recipes/:id', element: <RecipeDetailPage /> },
      { path: '/recipes/:id/edit', element: <RecipeFormPage /> },
    ],
  },
]

describe('RecipeListPage', () => {
  it('renders loading then recipe list', async () => {
    renderWithRouter(appRoutes, ['/recipes'])
    expect(await screen.findByText('Pasta Primavera')).toBeInTheDocument()
    expect(screen.getByText('Chicken Soup')).toBeInTheDocument()
    expect(screen.getByText('Avocado Toast')).toBeInTheDocument()
  })

  it('shows empty state when no recipes', async () => {
    server.use(http.get('/api/recipes', () => HttpResponse.json([])))
    renderWithRouter(appRoutes, ['/recipes'])
    expect(await screen.findByText(/no recipes yet/i)).toBeInTheDocument()
    expect(screen.getByRole('button', { name: /create recipe/i })).toBeInTheDocument()
  })

  it('shows error message when API fails', async () => {
    server.use(http.get('/api/recipes', () => HttpResponse.json({ message: 'Error' }, { status: 500 })))
    renderWithRouter(appRoutes, ['/recipes'])
    expect(await screen.findByText(/failed to load recipes/i)).toBeInTheDocument()
  })

  it('has a link to create a new recipe', async () => {
    renderWithRouter(appRoutes, ['/recipes'])
    await screen.findByText('Pasta Primavera')
    expect(screen.getByRole('link', { name: /new recipe/i })).toBeInTheDocument()
  })
})

describe('RecipeDetailPage', () => {
  it('renders recipe details with ingredients and instructions', async () => {
    renderWithRouter(appRoutes, ['/recipes/1'])
    expect(await screen.findByRole('heading', { name: 'Pasta Primavera' })).toBeInTheDocument()
    // Ingredients
    expect(await screen.findByText(/penne pasta/i)).toBeInTheDocument()
    expect(screen.getByText(/zucchini/i)).toBeInTheDocument()
    // Instructions
    expect(screen.getByText(/cook pasta according to package directions/i)).toBeInTheDocument()
  })

  it('shows error when recipe not found', async () => {
    server.use(http.get('/api/recipes/:id', () => HttpResponse.json({ message: 'Not found' }, { status: 404 })))
    renderWithRouter(appRoutes, ['/recipes/999'])
    expect(await screen.findByText(/recipe not found/i)).toBeInTheDocument()
  })
})

describe('RecipeFormPage (create)', () => {
  it('renders the create form', async () => {
    renderWithRouter(appRoutes, ['/recipes/new'])
    expect(await screen.findByRole('heading', { name: /new recipe/i })).toBeInTheDocument()
    expect(screen.getByLabelText(/recipe name/i)).toBeInTheDocument()
  })

  it('shows validation error for missing name', async () => {
    renderWithRouter(appRoutes, ['/recipes/new'])
    await screen.findByRole('heading', { name: /new recipe/i })
    await userEvent.click(screen.getByRole('button', { name: /create recipe/i }))
    expect(await screen.findByText('Recipe name is required')).toBeInTheDocument()
  })

  it('creates recipe and navigates to detail', async () => {
    renderWithRouter(appRoutes, ['/recipes/new'])
    await screen.findByRole('heading', { name: /new recipe/i })
    await userEvent.type(screen.getByLabelText(/recipe name/i), 'My New Recipe')
    await userEvent.click(screen.getByRole('button', { name: /create recipe/i }))
    // After creation, navigates to /recipes/99 (the mock returns id: 99)
    await waitFor(() => {
      expect(screen.queryByRole('heading', { name: /new recipe/i })).not.toBeInTheDocument()
    })
  })
})

describe('RecipeFormPage (edit)', () => {
  it('pre-fills form with existing recipe data', async () => {
    renderWithRouter(appRoutes, ['/recipes/1/edit'])
    const nameInput = await screen.findByLabelText(/recipe name/i)
    expect((nameInput as HTMLInputElement).value).toBe(recipeFixtures.detail(1).name)
  })
})
