import { useState } from 'react'
import { Link, useNavigate } from 'react-router-dom'
import { useForm } from 'react-hook-form'
import { zodResolver } from '@hookform/resolvers/zod'
import { useAuth } from '../../contexts/AuthContext'
import { registerSchema, type RegisterFormData } from './registerSchema'
import Button from '../../components/ui/Button'
import Input from '../../components/ui/Input'

export default function RegisterPage() {
  const { register: registerUser } = useAuth()
  const navigate = useNavigate()

  const [serverError, setServerError] = useState<string | null>(null)

  const {
    register,
    handleSubmit,
    formState: { errors, isSubmitting },
  } = useForm<RegisterFormData>({
    resolver: zodResolver(registerSchema),
  })

  const onSubmit = async (data: RegisterFormData) => {
    setServerError(null)
    try {
      await registerUser(data.email, data.password, data.displayName || undefined)
      navigate('/dashboard', { replace: true })
    } catch (err) {
      setServerError(err instanceof Error ? err.message : 'Registration failed.')
    }
  }

  return (
    <div className="flex min-h-screen items-center justify-center bg-gray-50 px-4">
      <div className="w-full max-w-sm rounded-xl border border-gray-200 bg-white p-8 shadow-sm">
        <h1 className="mb-6 text-2xl font-bold text-gray-900">Create account</h1>

        <form onSubmit={handleSubmit(onSubmit)} noValidate className="space-y-4">
          <Input
            label="Email"
            type="email"
            autoComplete="email"
            error={errors.email?.message}
            {...register('email')}
          />

          <Input
            label="Display name"
            type="text"
            autoComplete="name"
            placeholder="Optional"
            error={errors.displayName?.message}
            {...register('displayName')}
          />

          <Input
            label="Password"
            type="password"
            autoComplete="new-password"
            error={errors.password?.message}
            {...register('password')}
          />

          <Input
            label="Confirm password"
            type="password"
            autoComplete="new-password"
            error={errors.confirmPassword?.message}
            {...register('confirmPassword')}
          />

          {serverError && (
            <p role="alert" className="text-sm text-red-600">
              {serverError}
            </p>
          )}

          <Button type="submit" isLoading={isSubmitting} className="w-full">
            Create account
          </Button>
        </form>

        <p className="mt-4 text-center text-sm text-gray-600">
          Already have an account?{' '}
          <Link to="/login" className="text-blue-600 hover:underline">
            Sign in
          </Link>
        </p>
      </div>
    </div>
  )
}
