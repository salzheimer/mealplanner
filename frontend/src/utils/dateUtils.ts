import { format, parseISO, startOfWeek, endOfWeek, isToday as dfnsIsToday } from 'date-fns'

export function getCurrentWeekRange(): { startDate: string; endDate: string } {
  const now = new Date()
  const start = startOfWeek(now, { weekStartsOn: 1 }) // Monday
  const end = endOfWeek(now, { weekStartsOn: 1 })   // Sunday
  return {
    startDate: format(start, 'yyyy-MM-dd'),
    endDate: format(end, 'yyyy-MM-dd'),
  }
}

export function formatDate(dateStr: string, pattern = 'MMM d, yyyy'): string {
  return format(parseISO(dateStr), pattern)
}

export function formatShortDate(dateStr: string): string {
  return format(parseISO(dateStr), 'EEE, MMM d')
}

export function isToday(dateStr: string): boolean {
  return dfnsIsToday(parseISO(dateStr))
}

export function parseDuration(duration: string | null): { hours: number; minutes: number } {
  if (!duration) return { hours: 0, minutes: 0 }
  const parts = duration.split(':')
  return {
    hours: parseInt(parts[0] ?? '0', 10),
    minutes: parseInt(parts[1] ?? '0', 10),
  }
}

export function formatDuration(hours: number, minutes: number): string {
  return `${String(hours).padStart(2, '0')}:${String(minutes).padStart(2, '0')}:00`
}

export function durationLabel(duration: string | null): string {
  if (!duration) return ''
  const { hours, minutes } = parseDuration(duration)
  if (hours > 0 && minutes > 0) return `${hours}h ${minutes}m`
  if (hours > 0) return `${hours}h`
  if (minutes > 0) return `${minutes}m`
  return ''
}
