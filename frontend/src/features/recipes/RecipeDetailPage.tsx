import { useState } from 'react'
import { Link, useParams, useNavigate } from 'react-router-dom'
import { useRecipe, useDeleteRecipe } from '../../hooks/useRecipes'
import { recipeService } from '../../services/recipeService'
import { useAuth } from '../../contexts/AuthContext'
import Button from '../../components/ui/Button'
import LoadingSpinner from '../../components/ui/LoadingSpinner'
import PageHeader from '../../components/layout/PageHeader'
import ConfirmDialog from '../../components/ui/ConfirmDialog'
import ShareModal from '../../components/ui/ShareModal'
import IngredientList from './IngredientList'
import InstructionList from './InstructionList'
import { durationLabel } from '../../utils/dateUtils'
import type { Permission, ResourcePermission } from '../../types/shared'

export default function RecipeDetailPage() {
  const { id } = useParams<{ id: string }>()
  const recipeId = Number(id)
  const navigate = useNavigate()
  const { user } = useAuth()

  const { data: recipe, isLoading, error } = useRecipe(recipeId)
  const deleteRecipe = useDeleteRecipe()

  const [confirmDelete, setConfirmDelete] = useState(false)
  const [shareOpen, setShareOpen] = useState(false)
  const [shares, setShares] = useState<ResourcePermission[]>([])
  const [sharesLoading, setSharesLoading] = useState(false)

  const isOwner = recipe?.ownerUserId === user?.id

  async function handleDelete() {
    await deleteRecipe.mutateAsync(recipeId)
    navigate('/recipes', { replace: true })
  }

  async function handleOpenShare() {
    setShareOpen(true)
    setSharesLoading(true)
    try {
      const data = await recipeService.shares.list(recipeId)
      setShares(data)
    } finally {
      setSharesLoading(false)
    }
  }

  async function handleGrantShare(subjectId: number, permission: Permission) {
    await recipeService.shares.grant(recipeId, { subjectType: 'User', subjectId, permission })
    const data = await recipeService.shares.list(recipeId)
    setShares(data)
  }

  async function handleRevokeShare(shareId: number) {
    await recipeService.shares.revoke(recipeId, shareId)
    setShares(prev => prev.filter(s => s.id !== shareId))
  }

  async function handleSearchUsers(query: string) {
    const resp = await fetch(`/api/auth/users/search?q=${encodeURIComponent(query)}`, {
      headers: { 'Content-Type': 'application/json' },
    })
    return resp.ok ? resp.json() : []
  }

  if (isLoading) {
    return (
      <div className="flex justify-center py-16">
        <LoadingSpinner size="lg" />
      </div>
    )
  }

  if (error || !recipe) {
    return <p className="text-red-600">Recipe not found.</p>
  }

  return (
    <div className="max-w-3xl">
      <PageHeader
        title={recipe.name}
        action={
          <div className="flex gap-2">
            {isOwner && (
              <Button variant="ghost" size="sm" onClick={handleOpenShare}>
                Share
              </Button>
            )}
            <Link to={`/recipes/${recipeId}/edit`}>
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

      {recipe.description && (
        <p className="mt-2 text-gray-600">{recipe.description}</p>
      )}

      <div className="mt-4 flex flex-wrap gap-4 text-sm text-gray-500">
        {recipe.servings != null && <span>{recipe.servings} servings</span>}
        {recipe.prepTime && <span>Prep: {durationLabel(recipe.prepTime)}</span>}
        {recipe.cookTime && <span>Cook: {durationLabel(recipe.cookTime)}</span>}
        {recipe.originalSource && (
          <span>Source: {recipe.originalSource}</span>
        )}
      </div>

      <div className="mt-8 space-y-8">
        <IngredientList recipeId={recipeId} ingredients={recipe.ingredients} canEdit={isOwner} />
        <InstructionList recipeId={recipeId} instructions={recipe.instructions} canEdit={isOwner} />
      </div>

      <ConfirmDialog
        isOpen={confirmDelete}
        onClose={() => setConfirmDelete(false)}
        onConfirm={handleDelete}
        title="Delete recipe"
        message={`Are you sure you want to delete "${recipe.name}"? This cannot be undone.`}
        confirmLabel="Delete recipe"
        isDestructive
        isLoading={deleteRecipe.isPending}
      />

      <ShareModal
        isOpen={shareOpen}
        onClose={() => setShareOpen(false)}
        resourceName={recipe.name}
        shares={shares}
        isLoadingShares={sharesLoading}
        onGrantShare={handleGrantShare}
        onRevokeShare={handleRevokeShare}
        onSearchUsers={handleSearchUsers}
      />
    </div>
  )
}
