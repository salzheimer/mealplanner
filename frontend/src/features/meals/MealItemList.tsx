import { useState } from 'react'
import { useCreateMealItem, useUpdateMealItem, useDeleteMealItem } from '../../hooks/useMeals'
import { useRecipes } from '../../hooks/useRecipes'
import Button from '../../components/ui/Button'
import Select from '../../components/ui/Select'
import Input from '../../components/ui/Input'
import Badge from '../../components/ui/Badge'
import ConfirmDialog from '../../components/ui/ConfirmDialog'
import type { MealItem } from '../../types/meal'
import type { ItemType } from '../../types/shared'

interface MealItemListProps {
  mealId: number
  items: MealItem[]
  canEdit: boolean
}

const ITEM_TYPE_OPTIONS = [
  { value: 'Recipe' as ItemType, label: 'Recipe' },
  { value: 'Homemade' as ItemType, label: 'Homemade' },
  { value: 'StoreBought' as ItemType, label: 'Store-bought' },
]

export default function MealItemList({ mealId, items, canEdit }: MealItemListProps) {
  const createItem = useCreateMealItem(mealId)
  const updateItem = useUpdateMealItem(mealId)
  const deleteItem = useDeleteMealItem(mealId)
  const { data: recipes } = useRecipes()

  const [adding, setAdding] = useState(false)
  const [newName, setNewName] = useState('')
  const [newType, setNewType] = useState<ItemType>('Homemade')
  const [newRecipeId, setNewRecipeId] = useState('')

  const [editingId, setEditingId] = useState<number | null>(null)
  const [editName, setEditName] = useState('')
  const [editType, setEditType] = useState<ItemType>('Homemade')
  const [editRecipeId, setEditRecipeId] = useState('')

  const [confirmDeleteId, setConfirmDeleteId] = useState<number | null>(null)

  const recipeOptions = (recipes ?? []).map(r => ({ value: String(r.id), label: r.name }))

  async function handleAdd() {
    await createItem.mutateAsync({
      name: newName.trim() || undefined,
      itemType: newType,
      recipeId: newType === 'Recipe' && newRecipeId ? Number(newRecipeId) : undefined,
    })
    setAdding(false)
    setNewName('')
    setNewType('Homemade')
    setNewRecipeId('')
  }

  async function handleUpdate(item: MealItem) {
    await updateItem.mutateAsync({
      id: item.id,
      itemType: editType,
      name: editName.trim() || undefined,
      recipeId: editType === 'Recipe' && editRecipeId ? Number(editRecipeId) : undefined,
    })
    setEditingId(null)
  }

  async function handleDelete(id: number) {
    await deleteItem.mutateAsync(id)
    setConfirmDeleteId(null)
  }

  function startEdit(item: MealItem) {
    setEditingId(item.id)
    setEditName(item.name ?? '')
    setEditType(item.itemType)
    setEditRecipeId(item.recipeId ? String(item.recipeId) : '')
  }

  return (
    <div>
      <div className="mb-2 flex items-center justify-between">
        <h3 className="font-semibold text-gray-900">Items</h3>
        {canEdit && !adding && (
          <Button variant="ghost" size="sm" onClick={() => setAdding(true)}>
            + Add
          </Button>
        )}
      </div>

      {items.length === 0 && !adding && (
        <p className="text-sm text-gray-500">No items added yet.</p>
      )}

      <ul className="space-y-2">
        {items.map(item => (
          <li key={item.id} className="rounded-md border border-gray-100 bg-gray-50 p-3">
            {editingId === item.id ? (
              <div className="space-y-2">
                <Select
                  options={ITEM_TYPE_OPTIONS}
                  value={editType}
                  onChange={e => setEditType(e.target.value as ItemType)}
                  aria-label="Item type"
                />
                {editType === 'Recipe' ? (
                  <Select
                    options={recipeOptions}
                    placeholder="Select a recipe"
                    value={editRecipeId}
                    onChange={e => setEditRecipeId(e.target.value)}
                    aria-label="Recipe"
                  />
                ) : (
                  <Input
                    placeholder="Item name"
                    value={editName}
                    onChange={e => setEditName(e.target.value)}
                  />
                )}
                <div className="flex gap-2">
                  <Button size="sm" onClick={() => handleUpdate(item)} isLoading={updateItem.isPending}>
                    Save
                  </Button>
                  <Button size="sm" variant="ghost" onClick={() => setEditingId(null)}>
                    Cancel
                  </Button>
                </div>
              </div>
            ) : (
              <div className="flex items-center justify-between gap-2">
                <div className="flex items-center gap-2">
                  <Badge label={item.itemType} variant={item.itemType} />
                  <span className="text-sm text-gray-900">{item.name ?? 'Unnamed item'}</span>
                </div>
                {canEdit && (
                  <div className="flex shrink-0 gap-1">
                    <Button variant="ghost" size="sm" onClick={() => startEdit(item)}>
                      Edit
                    </Button>
                    <Button variant="ghost" size="sm" onClick={() => setConfirmDeleteId(item.id)}>
                      Delete
                    </Button>
                  </div>
                )}
              </div>
            )}
          </li>
        ))}
      </ul>

      {adding && (
        <div className="mt-3 space-y-2 rounded-md border border-blue-200 bg-blue-50 p-3">
          <Select
            options={ITEM_TYPE_OPTIONS}
            value={newType}
            onChange={e => setNewType(e.target.value as ItemType)}
            aria-label="Item type"
          />
          {newType === 'Recipe' ? (
            <Select
              options={recipeOptions}
              placeholder="Select a recipe"
              value={newRecipeId}
              onChange={e => setNewRecipeId(e.target.value)}
              aria-label="Recipe"
            />
          ) : (
            <Input
              placeholder="Item name"
              value={newName}
              onChange={e => setNewName(e.target.value)}
            />
          )}
          <div className="flex gap-2">
            <Button size="sm" onClick={handleAdd} isLoading={createItem.isPending}>
              Add item
            </Button>
            <Button size="sm" variant="ghost" onClick={() => setAdding(false)}>
              Cancel
            </Button>
          </div>
        </div>
      )}

      <ConfirmDialog
        isOpen={confirmDeleteId !== null}
        onClose={() => setConfirmDeleteId(null)}
        onConfirm={() => confirmDeleteId != null && handleDelete(confirmDeleteId)}
        title="Remove item"
        message="Remove this item from the meal?"
        confirmLabel="Remove"
        isDestructive
        isLoading={deleteItem.isPending}
      />
    </div>
  )
}
