import type { PlanSummary, PlanDetail, MealPlan, MealItemPlan } from '../../types/plan'

const planList: PlanSummary[] = [
  {
    id: 1,
    name: 'Week of May 19',
    startDate: '2026-05-19',
    endDate: '2026-05-25',
    ownerUserId: 1,
    createdAt: '2026-05-18T10:00:00Z',
    updatedAt: '2026-05-18T10:00:00Z',
  },
  {
    id: 2,
    name: 'Family BBQ Weekend',
    startDate: '2026-05-23',
    endDate: '2026-05-24',
    ownerUserId: 1,
    createdAt: '2026-05-20T10:00:00Z',
    updatedAt: '2026-05-20T10:00:00Z',
  },
]

const mealPlanList: MealPlan[] = [
  {
    id: 1,
    mealId: 1,
    planId: 1,
    serveDate: '2026-05-20',
    endDate: null,
    addedByUserId: 1,
    createdAt: '2026-05-18T10:00:00Z',
    updatedAt: '2026-05-18T10:00:00Z',
  },
  {
    id: 2,
    mealId: 2,
    planId: 1,
    serveDate: '2026-05-21',
    endDate: null,
    addedByUserId: 1,
    createdAt: '2026-05-18T10:00:00Z',
    updatedAt: '2026-05-18T10:00:00Z',
  },
  {
    id: 3,
    mealId: 3,
    planId: 1,
    serveDate: '2026-05-22',
    endDate: null,
    addedByUserId: 1,
    createdAt: '2026-05-18T10:00:00Z',
    updatedAt: '2026-05-18T10:00:00Z',
  },
  {
    id: 4,
    mealId: 4,
    planId: 1,
    serveDate: '2026-05-23',
    endDate: null,
    addedByUserId: 1,
    createdAt: '2026-05-18T10:00:00Z',
    updatedAt: '2026-05-18T10:00:00Z',
  },
]

const mealItemPlanList: MealItemPlan[] = [
  {
    id: 1,
    mealPlanId: 1,
    mealItemId: 1,
    assignedToUserId: 1,
    assignedToGuestName: null,
    status: 'Confirmed',
    notes: null,
    createdAt: '2026-05-18T10:00:00Z',
    updatedAt: '2026-05-18T10:00:00Z',
  },
  {
    id: 2,
    mealPlanId: 1,
    mealItemId: 2,
    assignedToUserId: null,
    assignedToGuestName: null,
    status: 'Pending',
    notes: null,
    createdAt: '2026-05-18T10:00:00Z',
    updatedAt: '2026-05-18T10:00:00Z',
  },
]

export const planFixtures = {
  list: planList,
  detail: (id: number): PlanDetail => {
    const plan = planList.find(p => p.id === id) ?? planList[0]
    return { ...plan, mealPlans: mealPlanList.filter(mp => mp.planId === plan.id) }
  },
  mealPlans: mealPlanList,
  mealPlansByPlan: (planId: number): MealPlan[] => mealPlanList.filter(mp => mp.planId === planId),
  mealPlansByDateRange: (startDate: string, endDate: string): MealPlan[] =>
    mealPlanList.filter(mp => {
      if (!mp.serveDate) return false
      return mp.serveDate >= startDate && mp.serveDate <= endDate
    }),
  mealItemPlans: (mealPlanId: number): MealItemPlan[] =>
    mealItemPlanList.filter(mip => mip.mealPlanId === mealPlanId),
}
