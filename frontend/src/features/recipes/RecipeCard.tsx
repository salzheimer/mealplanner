import { Link } from 'react-router-dom'
import Card from '../../components/ui/Card'
import type { RecipeSummary } from '../../types/recipe'
import { durationLabel } from '../../utils/dateUtils'

interface RecipeCardProps {
  recipe: RecipeSummary
}

export default function RecipeCard({ recipe }: RecipeCardProps) {
  return (
    <Card className="hover:shadow-md transition-shadow">
      <Link to={`/recipes/${recipe.id}`} className="block">
        <h3 className="font-semibold text-gray-900">{recipe.name}</h3>
        {recipe.description && (
          <p className="mt-1 text-sm text-gray-500 line-clamp-2">{recipe.description}</p>
        )}
        <div className="mt-3 flex flex-wrap gap-3 text-xs text-gray-500">
          {recipe.servings != null && (
            <span>{recipe.servings} serving{recipe.servings !== 1 ? 's' : ''}</span>
          )}
          {recipe.prepTime && <span>Prep: {durationLabel(recipe.prepTime)}</span>}
          {recipe.cookTime && <span>Cook: {durationLabel(recipe.cookTime)}</span>}
        </div>
      </Link>
    </Card>
  )
}
