import { createBrowserRouter, RouterProvider, Navigate, Outlet, useLocation } from 'react-router-dom'
import { useAuth } from './contexts/AuthContext'
import AppShell from './components/layout/AppShell'
import LoginPage from './features/auth/LoginPage'
import RegisterPage from './features/auth/RegisterPage'
import DashboardPage from './features/dashboard/DashboardPage'
import RecipeListPage from './features/recipes/RecipeListPage'
import RecipeDetailPage from './features/recipes/RecipeDetailPage'
import RecipeFormPage from './features/recipes/RecipeFormPage'
import MealListPage from './features/meals/MealListPage'
import MealDetailPage from './features/meals/MealDetailPage'
import MealFormPage from './features/meals/MealFormPage'
import PlanListPage from './features/plans/PlanListPage'
import PlanDetailPage from './features/plans/PlanDetailPage'
import PlanFormPage from './features/plans/PlanFormPage'
import NotFoundPage from './pages/NotFoundPage'
import ErrorBoundary from './components/ErrorBoundary'

function ProtectedRoute() {
  const { isLoading, isAuthenticated } = useAuth()
  const location = useLocation()

  if (isLoading) {
    return (
      <div className="flex min-h-screen items-center justify-center">
        <div className="h-8 w-8 animate-spin rounded-full border-4 border-blue-600 border-t-transparent" />
      </div>
    )
  }

  if (!isAuthenticated) {
    return <Navigate to="/login" state={{ from: location }} replace />
  }

  return <Outlet />
}

const router = createBrowserRouter([
  {
    path: '/',
    element: <Navigate to="/dashboard" replace />,
  },
  {
    path: '/login',
    element: <LoginPage />,
  },
  {
    path: '/register',
    element: <RegisterPage />,
  },
  {
    element: <ProtectedRoute />,
    children: [
      {
        element: <AppShell />,
        children: [
          { path: '/dashboard', element: <ErrorBoundary><DashboardPage /></ErrorBoundary> },
          { path: '/recipes', element: <ErrorBoundary><RecipeListPage /></ErrorBoundary> },
          { path: '/recipes/new', element: <ErrorBoundary><RecipeFormPage /></ErrorBoundary> },
          { path: '/recipes/:id', element: <ErrorBoundary><RecipeDetailPage /></ErrorBoundary> },
          { path: '/recipes/:id/edit', element: <ErrorBoundary><RecipeFormPage /></ErrorBoundary> },
          { path: '/meals', element: <ErrorBoundary><MealListPage /></ErrorBoundary> },
          { path: '/meals/new', element: <ErrorBoundary><MealFormPage /></ErrorBoundary> },
          { path: '/meals/:id', element: <ErrorBoundary><MealDetailPage /></ErrorBoundary> },
          { path: '/meals/:id/edit', element: <ErrorBoundary><MealFormPage /></ErrorBoundary> },
          { path: '/plans', element: <ErrorBoundary><PlanListPage /></ErrorBoundary> },
          { path: '/plans/new', element: <ErrorBoundary><PlanFormPage /></ErrorBoundary> },
          { path: '/plans/:id', element: <ErrorBoundary><PlanDetailPage /></ErrorBoundary> },
          { path: '/plans/:id/edit', element: <ErrorBoundary><PlanFormPage /></ErrorBoundary> },
        ],
      },
    ],
  },
  {
    path: '/404',
    element: <NotFoundPage />,
  },
  {
    path: '*',
    element: <Navigate to="/404" replace />,
  },
])

export default function App() {
  return <RouterProvider router={router} />
}
