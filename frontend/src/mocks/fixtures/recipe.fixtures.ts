import type { RecipeSummary, RecipeDetail } from '../../types/recipe'

const recipeList: RecipeSummary[] = [
  {
    id: 1,
    name: 'Pasta Primavera',
    description: 'A light and fresh pasta with seasonal vegetables.',
    ranking: 5,
    cookTime: '00:20:00',
    prepTime: '00:15:00',
    servings: 4,
    ownerUserId: 1,
    createdAt: '2026-01-10T12:00:00Z',
    updatedAt: '2026-01-10T12:00:00Z',
  },
  {
    id: 2,
    name: 'Chicken Soup',
    description: 'Hearty homemade chicken soup with vegetables and noodles.',
    ranking: 4,
    cookTime: '01:30:00',
    prepTime: '00:20:00',
    servings: 6,
    ownerUserId: 1,
    createdAt: '2026-01-11T12:00:00Z',
    updatedAt: '2026-01-11T12:00:00Z',
  },
  {
    id: 3,
    name: 'Avocado Toast',
    description: 'Simple and nutritious breakfast toast.',
    ranking: 3,
    cookTime: '00:05:00',
    prepTime: '00:05:00',
    servings: 1,
    ownerUserId: 1,
    createdAt: '2026-01-12T12:00:00Z',
    updatedAt: '2026-01-12T12:00:00Z',
  },
]

const recipeDetails: RecipeDetail[] = [
  {
    ...recipeList[0],
    notes: 'Use whatever vegetables are in season.',
    originalSource: null,
    ingredients: [
      { id: 1, recipeId: 1, name: 'Penne pasta', amount: 400, measurementType: 'g', note: null, createdAt: '2026-01-10T12:00:00Z', updatedAt: '2026-01-10T12:00:00Z' },
      { id: 2, recipeId: 1, name: 'Zucchini', amount: 1, measurementType: 'medium', note: 'sliced', createdAt: '2026-01-10T12:00:00Z', updatedAt: '2026-01-10T12:00:00Z' },
      { id: 3, recipeId: 1, name: 'Cherry tomatoes', amount: 200, measurementType: 'g', note: 'halved', createdAt: '2026-01-10T12:00:00Z', updatedAt: '2026-01-10T12:00:00Z' },
      { id: 4, recipeId: 1, name: 'Parmesan', amount: 50, measurementType: 'g', note: 'grated', createdAt: '2026-01-10T12:00:00Z', updatedAt: '2026-01-10T12:00:00Z' },
    ],
    instructions: [
      { id: 1, recipeId: 1, stepNumber: 1, description: 'Cook pasta according to package directions.', note: null, createdAt: '2026-01-10T12:00:00Z', updatedAt: '2026-01-10T12:00:00Z' },
      { id: 2, recipeId: 1, stepNumber: 2, description: 'Sauté vegetables in olive oil until tender.', note: 'About 5 minutes', createdAt: '2026-01-10T12:00:00Z', updatedAt: '2026-01-10T12:00:00Z' },
      { id: 3, recipeId: 1, stepNumber: 3, description: 'Toss pasta with vegetables and top with parmesan.', note: null, createdAt: '2026-01-10T12:00:00Z', updatedAt: '2026-01-10T12:00:00Z' },
    ],
  },
  {
    ...recipeList[1],
    notes: 'Better the next day.',
    originalSource: 'Grandma\'s recipe box',
    ingredients: [
      { id: 5, recipeId: 2, name: 'Chicken thighs', amount: 1, measurementType: 'kg', note: 'bone-in', createdAt: '2026-01-11T12:00:00Z', updatedAt: '2026-01-11T12:00:00Z' },
      { id: 6, recipeId: 2, name: 'Carrots', amount: 3, measurementType: 'large', note: 'sliced', createdAt: '2026-01-11T12:00:00Z', updatedAt: '2026-01-11T12:00:00Z' },
      { id: 7, recipeId: 2, name: 'Egg noodles', amount: 200, measurementType: 'g', note: null, createdAt: '2026-01-11T12:00:00Z', updatedAt: '2026-01-11T12:00:00Z' },
    ],
    instructions: [
      { id: 4, recipeId: 2, stepNumber: 1, description: 'Simmer chicken in broth for 45 minutes.', note: null, createdAt: '2026-01-11T12:00:00Z', updatedAt: '2026-01-11T12:00:00Z' },
      { id: 5, recipeId: 2, stepNumber: 2, description: 'Remove chicken, shred meat, return to pot.', note: null, createdAt: '2026-01-11T12:00:00Z', updatedAt: '2026-01-11T12:00:00Z' },
      { id: 6, recipeId: 2, stepNumber: 3, description: 'Add vegetables and noodles, cook until tender.', note: null, createdAt: '2026-01-11T12:00:00Z', updatedAt: '2026-01-11T12:00:00Z' },
    ],
  },
  {
    ...recipeList[2],
    notes: null,
    originalSource: null,
    ingredients: [
      { id: 8, recipeId: 3, name: 'Sourdough bread', amount: 2, measurementType: 'slices', note: null, createdAt: '2026-01-12T12:00:00Z', updatedAt: '2026-01-12T12:00:00Z' },
      { id: 9, recipeId: 3, name: 'Avocado', amount: 1, measurementType: 'ripe', note: 'mashed', createdAt: '2026-01-12T12:00:00Z', updatedAt: '2026-01-12T12:00:00Z' },
    ],
    instructions: [
      { id: 7, recipeId: 3, stepNumber: 1, description: 'Toast bread until golden.', note: null, createdAt: '2026-01-12T12:00:00Z', updatedAt: '2026-01-12T12:00:00Z' },
      { id: 8, recipeId: 3, stepNumber: 2, description: 'Spread mashed avocado, season with salt and pepper.', note: null, createdAt: '2026-01-12T12:00:00Z', updatedAt: '2026-01-12T12:00:00Z' },
    ],
  },
]

export const recipeFixtures = {
  list: recipeList,
  detail: (id: number): RecipeDetail => recipeDetails.find(r => r.id === id) ?? recipeDetails[0],
}
