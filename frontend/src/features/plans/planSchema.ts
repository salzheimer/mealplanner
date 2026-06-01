import { z } from 'zod'

export const planSchema = z.object({
  name: z.string().optional(),
  startDate: z.string().min(1, 'Start date is required'),
  endDate: z.string().optional(),
})

export type PlanFormData = z.infer<typeof planSchema>
