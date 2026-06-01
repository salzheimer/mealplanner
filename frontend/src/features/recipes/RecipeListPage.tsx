import { Link } from 'react-router-dom'
import { useRecipes } from '../../hooks/useRecipes'
import Button from '../../components/ui/Button'
import EmptyState from '../../components/ui/EmptyState'
import LoadingSpinner from '../../components/ui/LoadingSpinner'
import PageHeader from '../../components/layout/PageHeader'
import RecipeCard from './RecipeCard'

export default function RecipeListPage() {
  const { data: recipes, isLoading, error } = useRecipes()

  if (isLoading) {
    return (
      <div className="flex justify-center py-16">
        <LoadingSpinner size="lg" />
      </div>
    )
  }

  if (error) {
    return <p className="text-red-600">Failed to load recipes. Please try again.</p>
  }

  return (
    <div>
      <PageHeader
        title="Recipes"
        action={
          <Link to="/recipes/new">
            <Button>New recipe</Button>
          </Link>
        }
      />

      {recipes && recipes.length === 0 ? (
        <EmptyState
          heading="No recipes yet"
          description="Create your first recipe to start building your collection."
          action={
            <Link to="/recipes/new">
              <Button>Create recipe</Button>
            </Link>
          }
        />
      ) : (
        <div className="mt-6 grid gap-4 sm:grid-cols-2 lg:grid-cols-3">
          {recipes?.map(recipe => (
            <RecipeCard key={recipe.id} recipe={recipe} />
          ))}
        </div>
      )}
    </div>
  )
}
