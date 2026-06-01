export interface RecipeSummary {
  id: number
  name: string
  description: string | null
  ranking: number | null
  cookTime: string | null
  prepTime: string | null
  servings: number | null
  ownerUserId: number
  createdAt: string
  updatedAt: string
}

export interface RecipeIngredient {
  id: number
  recipeId: number
  name: string | null
  amount: number | null
  measurementType: string | null
  note: string | null
  createdAt: string
  updatedAt: string
}

export interface RecipeInstruction {
  id: number
  recipeId: number
  stepNumber: number | null
  description: string | null
  note: string | null
  createdAt: string
  updatedAt: string
}

export interface RecipeDetail extends RecipeSummary {
  notes: string | null
  originalSource: string | null
  ingredients: RecipeIngredient[]
  instructions: RecipeInstruction[]
}

export interface CreateRecipe {
  name: string
  description?: string
  notes?: string
  ranking?: number
  originalSource?: string
  cookTime?: string
  prepTime?: string
  servings?: number
}

export interface UpdateRecipe extends CreateRecipe {
  id: number
}

export interface CreateRecipeIngredient {
  name?: string
  amount?: number
  measurementType?: string
  note?: string
}

export interface UpdateRecipeIngredient extends CreateRecipeIngredient {
  id: number
}

export interface CreateRecipeInstruction {
  stepNumber?: number
  description?: string
  note?: string
}

export interface UpdateRecipeInstruction extends CreateRecipeInstruction {
  id: number
}
