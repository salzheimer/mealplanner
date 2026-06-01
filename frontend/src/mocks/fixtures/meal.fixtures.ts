import type { Meal, MealItem } from '../../types/meal'

const mealList: Meal[] = [
  {
    id: 1,
    name: 'Pasta Night',
    description: 'Classic Italian pasta dinner for the family.',
    notes: null,
    mealType: 'Dinner',
    isMultiDayMeal: false,
    ownerUserId: 1,
    createdAt: '2026-01-15T12:00:00Z',
    updatedAt: '2026-01-15T12:00:00Z',
  },
  {
    id: 2,
    name: 'Weekend Brunch',
    description: 'Lazy weekend morning spread.',
    notes: 'Make extra for leftovers.',
    mealType: 'Breakfast',
    isMultiDayMeal: false,
    ownerUserId: 1,
    createdAt: '2026-01-16T12:00:00Z',
    updatedAt: '2026-01-16T12:00:00Z',
  },
  {
    id: 3,
    name: 'Taco Tuesday',
    description: 'Build-your-own taco bar.',
    notes: null,
    mealType: 'Dinner',
    isMultiDayMeal: false,
    ownerUserId: 1,
    createdAt: '2026-01-17T12:00:00Z',
    updatedAt: '2026-01-17T12:00:00Z',
  },
  {
    id: 4,
    name: 'Soup & Salad',
    description: 'Light weekday lunch.',
    notes: null,
    mealType: 'Lunch',
    isMultiDayMeal: false,
    ownerUserId: 1,
    createdAt: '2026-01-18T12:00:00Z',
    updatedAt: '2026-01-18T12:00:00Z',
  },
]

const mealItemsMap: Record<number, MealItem[]> = {
  1: [
    { id: 1, mealId: 1, name: 'Pasta Primavera', recipeId: 1, itemType: 'Recipe', createdAt: '2026-01-15T12:00:00Z', updatedAt: '2026-01-15T12:00:00Z' },
    { id: 2, mealId: 1, name: 'Garlic bread', recipeId: null, itemType: 'Homemade', createdAt: '2026-01-15T12:00:00Z', updatedAt: '2026-01-15T12:00:00Z' },
    { id: 3, mealId: 1, name: 'Sparkling water', recipeId: null, itemType: 'StoreBought', createdAt: '2026-01-15T12:00:00Z', updatedAt: '2026-01-15T12:00:00Z' },
  ],
  2: [
    { id: 4, mealId: 2, name: 'Avocado Toast', recipeId: 3, itemType: 'Recipe', createdAt: '2026-01-16T12:00:00Z', updatedAt: '2026-01-16T12:00:00Z' },
    { id: 5, mealId: 2, name: 'Orange juice', recipeId: null, itemType: 'StoreBought', createdAt: '2026-01-16T12:00:00Z', updatedAt: '2026-01-16T12:00:00Z' },
  ],
  3: [
    { id: 6, mealId: 3, name: 'Taco shells', recipeId: null, itemType: 'StoreBought', createdAt: '2026-01-17T12:00:00Z', updatedAt: '2026-01-17T12:00:00Z' },
    { id: 7, mealId: 3, name: 'Seasoned beef', recipeId: null, itemType: 'Homemade', createdAt: '2026-01-17T12:00:00Z', updatedAt: '2026-01-17T12:00:00Z' },
  ],
  4: [
    { id: 8, mealId: 4, name: 'Chicken Soup', recipeId: 2, itemType: 'Recipe', createdAt: '2026-01-18T12:00:00Z', updatedAt: '2026-01-18T12:00:00Z' },
  ],
}

export const mealFixtures = {
  list: mealList,
  detail: (id: number): Meal => mealList.find(m => m.id === id) ?? mealList[0],
  items: (mealId: number): MealItem[] => mealItemsMap[mealId] ?? [],
}
