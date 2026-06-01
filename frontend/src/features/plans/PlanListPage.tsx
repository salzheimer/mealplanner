import { useState } from 'react'
import { Link } from 'react-router-dom'
import { usePlans, useSharedPlans } from '../../hooks/usePlans'
import Button from '../../components/ui/Button'
import Card from '../../components/ui/Card'
import EmptyState from '../../components/ui/EmptyState'
import LoadingSpinner from '../../components/ui/LoadingSpinner'
import PageHeader from '../../components/layout/PageHeader'
import { formatDate } from '../../utils/dateUtils'

type Tab = 'mine' | 'shared'

export default function PlanListPage() {
  const [activeTab, setActiveTab] = useState<Tab>('mine')
  const { data: myPlans, isLoading: isLoadingMine } = usePlans()
  const { data: sharedPlans, isLoading: isLoadingShared } = useSharedPlans()

  const plans = activeTab === 'mine' ? myPlans : sharedPlans
  const isLoading = activeTab === 'mine' ? isLoadingMine : isLoadingShared

  return (
    <div>
      <PageHeader
        title="Plans"
        action={
          <Link to="/plans/new">
            <Button>New plan</Button>
          </Link>
        }
      />

      <div className="mt-4 flex gap-1 border-b border-gray-200">
        {(['mine', 'shared'] as const).map(tab => (
          <button
            key={tab}
            onClick={() => setActiveTab(tab)}
            className={`px-4 py-2 text-sm font-medium transition-colors ${
              activeTab === tab
                ? 'border-b-2 border-blue-600 text-blue-600'
                : 'text-gray-600 hover:text-gray-900'
            }`}
          >
            {tab === 'mine' ? 'My plans' : 'Shared with me'}
          </button>
        ))}
      </div>

      {isLoading ? (
        <div className="flex justify-center py-16">
          <LoadingSpinner size="lg" />
        </div>
      ) : plans && plans.length === 0 ? (
        <EmptyState
          heading={activeTab === 'mine' ? 'No plans yet' : 'No plans shared with you'}
          description={activeTab === 'mine' ? 'Create a plan to start organizing your meals.' : undefined}
          action={
            activeTab === 'mine' ? (
              <Link to="/plans/new">
                <Button>Create plan</Button>
              </Link>
            ) : undefined
          }
        />
      ) : (
        <div className="mt-6 grid gap-4 sm:grid-cols-2 lg:grid-cols-3">
          {plans?.map(plan => (
            <Card key={plan.id} className="hover:shadow-md transition-shadow">
              <Link to={`/plans/${plan.id}`} className="block">
                <h3 className="font-semibold text-gray-900">{plan.name ?? 'Untitled plan'}</h3>
                <p className="mt-1 text-sm text-gray-500">
                  Starts {formatDate(plan.startDate)}
                  {plan.endDate && ` · Ends ${formatDate(plan.endDate)}`}
                </p>
              </Link>
            </Card>
          ))}
        </div>
      )}
    </div>
  )
}
