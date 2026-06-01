import { render, screen, waitFor } from '@testing-library/react'
import userEvent from '@testing-library/user-event'
import { createMemoryRouter, RouterProvider } from 'react-router-dom'
import { QueryClient, QueryClientProvider } from '@tanstack/react-query'
import { AuthProvider } from '../../contexts/AuthContext'
import RegisterPage from './RegisterPage'
import { server } from '../../mocks/server'
import { http, HttpResponse } from 'msw'

function renderRegisterPage() {
  const queryClient = new QueryClient({ defaultOptions: { queries: { retry: false } } })
  const router = createMemoryRouter(
    [
      { path: '/register', element: <RegisterPage /> },
      { path: '/dashboard', element: <div>Dashboard</div> },
      { path: '/login', element: <div>Login</div> },
    ],
    { initialEntries: ['/register'] },
  )
  return render(
    <QueryClientProvider client={queryClient}>
      <AuthProvider>
        <RouterProvider router={router} />
      </AuthProvider>
    </QueryClientProvider>,
  )
}

describe('RegisterPage', () => {
  it('renders all form fields and submit button', () => {
    renderRegisterPage()
    expect(screen.getByLabelText('Email')).toBeInTheDocument()
    expect(screen.getByLabelText('Display name')).toBeInTheDocument()
    expect(screen.getByLabelText('Password')).toBeInTheDocument()
    expect(screen.getByLabelText('Confirm password')).toBeInTheDocument()
    expect(screen.getByRole('button', { name: /create account/i })).toBeInTheDocument()
  })

  it('shows validation errors for empty submission', async () => {
    renderRegisterPage()
    await userEvent.click(screen.getByRole('button', { name: /create account/i }))
    expect(await screen.findByText('Email is required')).toBeInTheDocument()
    expect(await screen.findByText('Password must be at least 8 characters')).toBeInTheDocument()
    expect(await screen.findByText('Please confirm your password')).toBeInTheDocument()
  })

  it('shows validation error for invalid email format', async () => {
    renderRegisterPage()
    await userEvent.type(screen.getByLabelText('Email'), 'notanemail')
    await userEvent.type(screen.getByLabelText('Password'), 'password123')
    await userEvent.type(screen.getByLabelText('Confirm password'), 'password123')
    await userEvent.click(screen.getByRole('button', { name: /create account/i }))
    expect(await screen.findByText('Invalid email address')).toBeInTheDocument()
  })

  it('shows error when passwords do not match', async () => {
    renderRegisterPage()
    await userEvent.type(screen.getByLabelText('Email'), 'test@example.com')
    await userEvent.type(screen.getByLabelText('Password'), 'password123')
    await userEvent.type(screen.getByLabelText('Confirm password'), 'different456')
    await userEvent.click(screen.getByRole('button', { name: /create account/i }))
    expect(await screen.findByText('Passwords do not match')).toBeInTheDocument()
  })

  it('shows error when password is too short', async () => {
    renderRegisterPage()
    await userEvent.type(screen.getByLabelText('Email'), 'test@example.com')
    await userEvent.type(screen.getByLabelText('Password'), 'short')
    await userEvent.type(screen.getByLabelText('Confirm password'), 'short')
    await userEvent.click(screen.getByRole('button', { name: /create account/i }))
    expect(await screen.findByText('Password must be at least 8 characters')).toBeInTheDocument()
  })

  it('redirects to dashboard on successful registration', async () => {
    renderRegisterPage()
    await userEvent.type(screen.getByLabelText('Email'), 'newuser@example.com')
    await userEvent.type(screen.getByLabelText('Password'), 'password123')
    await userEvent.type(screen.getByLabelText('Confirm password'), 'password123')
    await userEvent.click(screen.getByRole('button', { name: /create account/i }))
    await waitFor(() => {
      expect(screen.getByText('Dashboard')).toBeInTheDocument()
    })
  })

  it('allows registration without display name (optional field)', async () => {
    renderRegisterPage()
    await userEvent.type(screen.getByLabelText('Email'), 'nodisplay@example.com')
    await userEvent.type(screen.getByLabelText('Password'), 'password123')
    await userEvent.type(screen.getByLabelText('Confirm password'), 'password123')
    await userEvent.click(screen.getByRole('button', { name: /create account/i }))
    await waitFor(() => {
      expect(screen.getByText('Dashboard')).toBeInTheDocument()
    })
  })

  it('shows server error when registration fails', async () => {
    server.use(
      http.post('/api/auth/register', () =>
        HttpResponse.json({ message: 'Email already in use' }, { status: 409 }),
      ),
    )
    renderRegisterPage()
    await userEvent.type(screen.getByLabelText('Email'), 'taken@example.com')
    await userEvent.type(screen.getByLabelText('Password'), 'password123')
    await userEvent.type(screen.getByLabelText('Confirm password'), 'password123')
    await userEvent.click(screen.getByRole('button', { name: /create account/i }))
    expect(await screen.findByRole('alert')).toBeInTheDocument()
  })

  it('has a link to the login page', () => {
    renderRegisterPage()
    expect(screen.getByRole('link', { name: /sign in/i })).toHaveAttribute('href', '/login')
  })
})
