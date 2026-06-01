import { z } from 'zod'

export const recipeSchema = z.object({
  name: z.string().min(1, 'Recipe name is required'),
  description: z.string().optional(),
  notes: z.string().optional(),
  originalSource: z.string().optional(),
  servings: z.string().optional(),
  prepTimeHours: z.string().optional(),
  prepTimeMinutes: z.string().optional(),
  cookTimeHours: z.string().optional(),
  cookTimeMinutes: z.string().optional(),
})

export type RecipeFormData = z.infer<typeof recipeSchema>

export function parseDurationToForm(duration: string | null): { hours: string; minutes: string } {
  if (!duration) return { hours: '', minutes: '' }
  const parts = duration.split(':')
  const h = parseInt(parts[0] ?? '0', 10)
  const m = parseInt(parts[1] ?? '0', 10)
  return {
    hours: h > 0 ? String(h) : '',
    minutes: m > 0 ? String(m) : '',
  }
}

export function formToDuration(hours: string | undefined, minutes: string | undefined): string | undefined {
  const h = parseInt(hours ?? '0', 10) || 0
  const m = parseInt(minutes ?? '0', 10) || 0
  if (h === 0 && m === 0) return undefined
  return `${String(h).padStart(2, '0')}:${String(m).padStart(2, '0')}:00`
}
