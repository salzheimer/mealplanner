import { useState } from 'react'
import { Link, useParams, useNavigate } from 'react-router-dom'
import { useMeal, useMealItems, useDeleteMeal } from '../../hooks/useMeals'
import { mealService } from '../../services/mealService'
import { useAuth } from '../../contexts/AuthContext'
import Button from '../../components/ui/Button'
import Badge from '../../components/ui/Badge'
import LoadingSpinner from '../../components/ui/LoadingSpinner'
import PageHeader from '../../components/layout/PageHeader'
import ConfirmDialog from '../../components/ui/ConfirmDialog'
import ShareModal from '../../components/ui/ShareModal'
import MealItemList from './MealItemList'
import type { Permission, ResourcePermission } from '../../types/shared'

export default function MealDetailPage() {
  const { id } = useParams<{ id: string }>()
  const mealId = Number(id)
  const navigate = useNavigate()
  const { user } = useAuth()

  const { data: meal, isLoading, error } = useMeal(mealId)
  const { data: items = [] } = useMealItems(mealId)
  const deleteMeal = useDeleteMeal()

  const [confirmDelete, setConfirmDelete] = useState(false)
  const [shareOpen, setShareOpen] = useState(false)
  const [shares, setShares] = useState<ResourcePermission[]>([])
  const [sharesLoading, setSharesLoading] = useState(false)

  const isOwner = meal?.ownerUserId === user?.id

  async function handleDelete() {
    await deleteMeal.mutateAsync(mealId)
    navigate('/meals', { replace: true })
  }

  async function handleOpenShare() {
    setShareOpen(true)
    setSharesLoading(true)
    try {
      const data = await mealService.shares.list(mealId)
      setShares(data)
    } finally {
      setSharesLoading(false)
    }
  }

  async function handleGrantShare(subjectId: number, permission: Permission) {
    await mealService.shares.grant(mealId, { subjectType: 'User', subjectId, permission })
    const data = await mealService.shares.list(mealId)
    setShares(data)
  }

  async function handleRevokeShare(shareId: number) {
    await mealService.shares.revoke(mealId, shareId)
    setShares(prev => prev.filter(s => s.id !== shareId))
  }

  async function handleSearchUsers(query: string) {
    const resp = await fetch(`/api/auth/users/search?q=${encodeURIComponent(query)}`)
    return resp.ok ? resp.json() : []
  }

  if (isLoading) {
    return (
      <div className="flex justify-center py-16">
        <LoadingSpinner size="lg" />
      </div>
    )
  }

  if (error || !meal) {
    return <p className="text-red-600">Meal not found.</p>
  }

  return (
    <div className="max-w-3xl">
      <PageHeader
        title={meal.name ?? 'Untitled meal'}
        action={
          <div className="flex gap-2">
            {isOwner && (
              <Button variant="ghost" size="sm" onClick={handleOpenShare}>
                Share
              </Button>
            )}
            <Link to={`/meals/${mealId}/edit`}>
              <Button variant="secondary" size="sm">Edit</Button>
            </Link>
            {isOwner && (
              <Button variant="danger" size="sm" onClick={() => setConfirmDelete(true)}>
                Delete
              </Button>
            )}
          </div>
        }
      />

      <div className="mt-2 flex items-center gap-3">
        <Badge label={meal.mealType} variant={meal.mealType} />
        {meal.isMultiDayMeal && <span className="text-xs text-blue-600">Multi-day meal</span>}
      </div>

      {meal.description && (
        <p className="mt-3 text-gray-600">{meal.description}</p>
      )}

      {meal.notes && (
        <p className="mt-2 text-sm text-gray-500 italic">{meal.notes}</p>
      )}

      <div className="mt-8">
        <MealItemList mealId={mealId} items={items} canEdit={isOwner} />
      </div>

      <ConfirmDialog
        isOpen={confirmDelete}
        onClose={() => setConfirmDelete(false)}
        onConfirm={handleDelete}
        title="Delete meal"
        message={`Are you sure you want to delete "${meal.name}"? This cannot be undone.`}
        confirmLabel="Delete meal"
        isDestructive
        isLoading={deleteMeal.isPending}
      />

      <ShareModal
        isOpen={shareOpen}
        onClose={() => setShareOpen(false)}
        resourceName={meal.name ?? 'this meal'}
        shares={shares}
        isLoadingShares={sharesLoading}
        onGrantShare={handleGrantShare}
        onRevokeShare={handleRevokeShare}
        onSearchUsers={handleSearchUsers}
      />
    </div>
  )
}
