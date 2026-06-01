export const queryKeys = {
  recipes: {
    all: ['recipes'] as const,
    detail: (id: number) => ['recipes', id] as const,
    ingredients: (id: number) => ['recipes', id, 'ingredients'] as const,
    instructions: (id: number) => ['recipes', id, 'instructions'] as const,
    sharedWithMe: ['recipes', 'shared-with-me'] as const,
  },
  meals: {
    all: ['meals'] as const,
    detail: (id: number) => ['meals', id] as const,
    items: (id: number) => ['meals', id, 'items'] as const,
    sharedWithMe: ['meals', 'shared-with-me'] as const,
  },
  plans: {
    all: ['plans'] as const,
    detail: (id: number) => ['plans', id] as const,
    shares: (id: number) => ['plans', id, 'shares'] as const,
    sharedWithMe: ['plans', 'shared-with-me'] as const,
    byDateRange: (startDate: string, endDate: string) => ['plans', 'date-range', startDate, endDate] as const,
  },
  mealPlans: {
    all: ['mealplans'] as const,
    detail: (id: number) => ['mealplans', id] as const,
    byDateRange: (startDate: string, endDate: string) => ['mealplans', 'date-range', startDate, endDate] as const,
    byPlan: (planId: number) => ['mealplans', 'plan', planId] as const,
    items: (mealPlanId: number) => ['mealplans', mealPlanId, 'items'] as const,
  },
} as const
