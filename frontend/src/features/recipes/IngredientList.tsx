import { useState } from 'react'
import { useCreateIngredient, useUpdateIngredient, useDeleteIngredient } from '../../hooks/useRecipes'
import Button from '../../components/ui/Button'
import Input from '../../components/ui/Input'
import ConfirmDialog from '../../components/ui/ConfirmDialog'
import type { RecipeIngredient } from '../../types/recipe'

interface IngredientListProps {
  recipeId: number
  ingredients: RecipeIngredient[]
  canEdit: boolean
}

interface EditState {
  id: number
  name: string
  amount: string
  measurementType: string
  note: string
}

export default function IngredientList({ recipeId, ingredients, canEdit }: IngredientListProps) {
  const createIngredient = useCreateIngredient(recipeId)
  const updateIngredient = useUpdateIngredient(recipeId)
  const deleteIngredient = useDeleteIngredient(recipeId)

  const [adding, setAdding] = useState(false)
  const [newName, setNewName] = useState('')
  const [newAmount, setNewAmount] = useState('')
  const [newMeasurementType, setNewMeasurementType] = useState('')
  const [newNote, setNewNote] = useState('')

  const [editing, setEditing] = useState<EditState | null>(null)
  const [confirmDeleteId, setConfirmDeleteId] = useState<number | null>(null)

  async function handleAdd() {
    if (!newName.trim()) return
    await createIngredient.mutateAsync({
      name: newName.trim() || undefined,
      amount: newAmount ? Number(newAmount) : undefined,
      measurementType: newMeasurementType.trim() || undefined,
      note: newNote.trim() || undefined,
    })
    setAdding(false)
    setNewName('')
    setNewAmount('')
    setNewMeasurementType('')
    setNewNote('')
  }

  async function handleUpdate() {
    if (!editing) return
    await updateIngredient.mutateAsync({
      id: editing.id,
      data: {
        id: editing.id,
        name: editing.name.trim() || undefined,
        amount: editing.amount ? Number(editing.amount) : undefined,
        measurementType: editing.measurementType.trim() || undefined,
        note: editing.note.trim() || undefined,
      },
    })
    setEditing(null)
  }

  async function handleDelete(id: number) {
    await deleteIngredient.mutateAsync(id)
    setConfirmDeleteId(null)
  }

  return (
    <div>
      <div className="mb-2 flex items-center justify-between">
        <h3 className="font-semibold text-gray-900">Ingredients</h3>
        {canEdit && !adding && (
          <Button variant="ghost" size="sm" onClick={() => setAdding(true)}>
            + Add
          </Button>
        )}
      </div>

      {ingredients.length === 0 && !adding && (
        <p className="text-sm text-gray-500">No ingredients added yet.</p>
      )}

      <ul className="space-y-2">
        {ingredients.map(ingredient => (
          <li key={ingredient.id} className="rounded-md border border-gray-100 bg-gray-50 p-3">
            {editing?.id === ingredient.id ? (
              <div className="space-y-2">
                <div className="flex gap-2">
                  <Input
                    placeholder="Name"
                    value={editing.name}
                    onChange={e => setEditing({ ...editing, name: e.target.value })}
                    className="flex-1"
                  />
                  <Input
                    placeholder="Amount"
                    type="number"
                    value={editing.amount}
                    onChange={e => setEditing({ ...editing, amount: e.target.value })}
                    className="w-24"
                  />
                  <Input
                    placeholder="Unit"
                    value={editing.measurementType}
                    onChange={e => setEditing({ ...editing, measurementType: e.target.value })}
                    className="w-24"
                  />
                </div>
                <Input
                  placeholder="Note"
                  value={editing.note}
                  onChange={e => setEditing({ ...editing, note: e.target.value })}
                />
                <div className="flex gap-2">
                  <Button size="sm" onClick={handleUpdate} isLoading={updateIngredient.isPending}>
                    Save
                  </Button>
                  <Button size="sm" variant="ghost" onClick={() => setEditing(null)}>
                    Cancel
                  </Button>
                </div>
              </div>
            ) : (
              <div className="flex items-start justify-between gap-2">
                <div className="text-sm">
                  <span className="font-medium text-gray-900">
                    {ingredient.amount != null && `${ingredient.amount} `}
                    {ingredient.measurementType && `${ingredient.measurementType} `}
                    {ingredient.name}
                  </span>
                  {ingredient.note && (
                    <span className="ml-1 text-gray-500">({ingredient.note})</span>
                  )}
                </div>
                {canEdit && (
                  <div className="flex shrink-0 gap-1">
                    <Button
                      variant="ghost"
                      size="sm"
                      onClick={() =>
                        setEditing({
                          id: ingredient.id,
                          name: ingredient.name ?? '',
                          amount: ingredient.amount?.toString() ?? '',
                          measurementType: ingredient.measurementType ?? '',
                          note: ingredient.note ?? '',
                        })
                      }
                    >
                      Edit
                    </Button>
                    <Button
                      variant="ghost"
                      size="sm"
                      onClick={() => setConfirmDeleteId(ingredient.id)}
                    >
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
          <div className="flex gap-2">
            <Input
              placeholder="Ingredient name *"
              value={newName}
              onChange={e => setNewName(e.target.value)}
              className="flex-1"
            />
            <Input
              placeholder="Amount"
              type="number"
              value={newAmount}
              onChange={e => setNewAmount(e.target.value)}
              className="w-24"
            />
            <Input
              placeholder="Unit"
              value={newMeasurementType}
              onChange={e => setNewMeasurementType(e.target.value)}
              className="w-24"
            />
          </div>
          <Input
            placeholder="Note (optional)"
            value={newNote}
            onChange={e => setNewNote(e.target.value)}
          />
          <div className="flex gap-2">
            <Button size="sm" onClick={handleAdd} isLoading={createIngredient.isPending}>
              Add ingredient
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
        title="Delete ingredient"
        message="Remove this ingredient from the recipe?"
        confirmLabel="Delete"
        isDestructive
        isLoading={deleteIngredient.isPending}
      />
    </div>
  )
}
