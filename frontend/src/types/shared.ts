export type MealType = 'Breakfast' | 'Lunch' | 'Dinner' | 'Snack'
export type ItemType = 'Recipe' | 'Homemade' | 'StoreBought'
export type ItemStatus = 'Unknown' | 'Pending' | 'Confirmed'
export type Permission = 'View' | 'Edit' | 'Comment' | 'Manage'
export type SubjectType = 'User' | 'Group'
export type ResourceType = 'Recipe' | 'Meal' | 'Plan'

export interface ResourcePermission {
  id: number
  resourceType: ResourceType
  resourceId: number
  subjectType: SubjectType
  subjectId: number
  permission: Permission
  grantedBy: number
  grantedAt: string
  expiresAt: string | null
}

export interface ShareRequest {
  subjectType: SubjectType
  subjectId: number
  permission: Permission
}
