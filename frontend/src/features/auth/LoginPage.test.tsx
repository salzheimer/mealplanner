import { render, screen, waitFor } from '@testing-library/react'
import userEvent from '@testing-library/user-event'
import { createMemoryRouter, RouterProvider } from 'react-router-dom'
import { QueryClient, QueryClientProvider } from '@tanstack/react-query'
import { AuthProvider } from '../../contexts/AuthContext'
import LoginPage from './LoginPage'
import { server } from '../../mocks/server'
import { http, HttpResponse } from 'msw'

function renderLoginPage(initialEntries = ['/login']) {
  const queryClient = new QueryClient({ defaultOptions: { queries: { retry: false } } })
  const router = createMemoryRouter(
    [
      { path: '/login', element: <LoginPage /> },
      { path: '/dashboard', element: <div>Dashboard</div> },
      { path: '/register', element: <div>Register</div> },
    ],
    { initialEntries },
  )
  return render(
    <QueryClientProvider client={queryClient}>
      <AuthProvider>
        <RouterProvider router={router} />
      </AuthProvider>
    </QueryClientProvider>,
  )
}

describe('LoginPage', () => {
  it('renders email and password fields and submit button', () => {
    renderLoginPage()
    expect(screen.getByLabelText('Email')).toBeInTheDocument()
    expect(screen.getByLabelText('Password')).toBeInTheDocument()
    expect(screen.getByRole('button', { name: /sign in/i })).toBeInTheDocument()
  })

  it('shows validation errors for empty submission', async () => {
    renderLoginPage()
    await userEvent.click(screen.getByRole('button', { name: /sign in/i }))
    expect(await screen.findByText('Email is required')).toBeInTheDocument()
    expect(await screen.findByText('Password is required')).toBeInTheDocument()
  })

  it('shows validation error for invalid email format', async () => {
    renderLoginPage()
    await userEvent.type(screen.getByLabelText('Email'), 'notanemail')
    await userEvent.type(screen.getByLabelText('Password'), 'somepassword')
    await userEvent.click(screen.getByRole('button', { name: /sign in/i }))
    expect(await screen.findByText('Invalid email address')).toBeInTheDocument()
  })

  it('shows server error for invalid credentials', async () => {
    renderLoginPage()
    await userEvent.type(screen.getByLabelText('Email'), 'wrong@example.com')
    await userEvent.type(screen.getByLabelText('Password'), 'wrongpassword')
    await userEvent.click(screen.getByRole('button', { name: /sign in/i }))
    expect(await screen.findByRole('alert')).toHaveTextContent(/invalid credentials/i)
  })

  it('redirects to dashboard on successful login', async () => {
    renderLoginPage()
    await userEvent.type(screen.getByLabelText('Email'), 'test@example.com')
    await userEvent.type(screen.getByLabelText('Password'), 'password123')
    await userEvent.click(screen.getByRole('button', { name: /sign in/i }))
    await waitFor(() => {
      expect(screen.getByText('Dashboard')).toBeInTheDocument()
    })
  })

  it('redirects to original destination after login', async () => {
    const queryClient = new QueryClient({ defaultOptions: { queries: { retry: false } } })
    const router = createMemoryRouter(
      [
        { path: '/login', element: <LoginPage /> },
        { path: '/recipes', element: <div>Recipes</div> },
      ],
      {
        initialEntries: [{ pathname: '/login', state: { from: { pathname: '/recipes' } } }],
      },
    )
    render(
      <QueryClientProvider client={queryClient}>
        <AuthProvider>
          <RouterProvider router={router} />
        </AuthProvider>
      </QueryClientProvider>,
    )
    await userEvent.type(screen.getByLabelText('Email'), 'test@example.com')
    await userEvent.type(screen.getByLabelText('Password'), 'password123')
    await userEvent.click(screen.getByRole('button', { name: /sign in/i }))
    await waitFor(() => {
      expect(screen.getByText('Recipes')).toBeInTheDocument()
    })
  })

  it('shows server error when API returns 500', async () => {
    server.use(
      http.post('/api/auth/login', () => HttpResponse.json({ message: 'Internal server error' }, { status: 500 })),
    )
    renderLoginPage()
    await userEvent.type(screen.getByLabelText('Email'), 'test@example.com')
    await userEvent.type(screen.getByLabelText('Password'), 'password123')
    await userEvent.click(screen.getByRole('button', { name: /sign in/i }))
    expect(await screen.findByRole('alert')).toBeInTheDocument()
  })

  it('has a link to the register page', () => {
    renderLoginPage()
    expect(screen.getByRole('link', { name: /create one/i })).toHaveAttribute('href', '/register')
  })
})
