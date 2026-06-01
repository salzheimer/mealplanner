import { useState } from 'react'
import { Link, useParams, useNavigate } from 'react-router-dom'
import { usePlan, useDeletePlan } from '../../hooks/usePlans'
import { planService } from '../../services/planService'
import { useAuth } from '../../contexts/AuthContext'
import Button from '../../components/ui/Button'
import LoadingSpinner from '../../components/ui/LoadingSpinner'
import PageHeader from '../../components/layout/PageHeader'
import ConfirmDialog from '../../components/ui/ConfirmDialog'
import ShareModal from '../../components/ui/ShareModal'
import PlanCalendarView from './PlanCalendarView'
import { formatDate } from '../../utils/dateUtils'
import type { Permission, ResourcePermission } from '../../types/shared'

export default function PlanDetailPage() {
  const { id } = useParams<{ id: string }>()
  const planId = Number(id)
  const navigate = useNavigate()
  const { user } = useAuth()

  const { data: plan, isLoading, error } = usePlan(planId)
  const deletePlan = useDeletePlan()

  const [confirmDelete, setConfirmDelete] = useState(false)
  const [shareOpen, setShareOpen] = useState(false)
  const [shares, setShares] = useState<ResourcePermission[]>([])
  const [sharesLoading, setSharesLoading] = useState(false)

  const isOwner = plan?.ownerUserId === user?.id

  async function handleDelete() {
    await deletePlan.mutateAsync(planId)
    navigate('/plans', { replace: true })
  }

  async function handleOpenShare() {
    setShareOpen(true)
    setSharesLoading(true)
    try {
      const data = await planService.shares.list(planId)
      setShares(data as unknown as ResourcePermission[])
    } finally {
      setSharesLoading(false)
    }
  }

  async function handleGrantShare(subjectId: number, permission: Permission) {
    await planService.shares.grant({
      planId,
      sharedWithUserId: subjectId,
      permission,
    })
    const data = await planService.shares.list(planId)
    setShares(data as unknown as ResourcePermission[])
  }

  async function handleRevokeShare(shareId: number) {
    await planService.shares.revoke(planId, shareId)
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

  if (error || !plan) {
    return <p className="text-red-600">Plan not found.</p>
  }

  return (
    <div>
      <PageHeader
        title={plan.name ?? 'Untitled plan'}
        action={
          <div className="flex gap-2">
            {isOwner && (
              <Button variant="ghost" size="sm" onClick={handleOpenShare}>
                Share
              </Button>
            )}
            <Link to={`/plans/${planId}/edit`}>
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

      <p className="mt-2 text-sm text-gray-500">
        {formatDate(plan.startDate)}
        {plan.endDate && ` – ${formatDate(plan.endDate)}`}
      </p>

      <div className="mt-6">
        <h2 className="mb-4 text-lg font-semibold text-gray-900">Calendar</h2>
        <PlanCalendarView planId={planId} allMealPlans={plan.mealPlans} canEdit={isOwner} />
      </div>

      <ConfirmDialog
        isOpen={confirmDelete}
        onClose={() => setConfirmDelete(false)}
        onConfirm={handleDelete}
        title="Delete plan"
        message={`Are you sure you want to delete "${plan.name ?? 'this plan'}"? This cannot be undone.`}
        confirmLabel="Delete plan"
        isDestructive
        isLoading={deletePlan.isPending}
      />

      <ShareModal
        isOpen={shareOpen}
        onClose={() => setShareOpen(false)}
        resourceName={plan.name ?? 'this plan'}
        shares={shares}
        isLoadingShares={sharesLoading}
        onGrantShare={handleGrantShare}
        onRevokeShare={handleRevokeShare}
        onSearchUsers={handleSearchUsers}
      />
    </div>
  )
}
