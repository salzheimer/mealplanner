import { useState } from 'react'
import { useCreateInstruction, useUpdateInstruction, useDeleteInstruction } from '../../hooks/useRecipes'
import Button from '../../components/ui/Button'
import TextArea from '../../components/ui/TextArea'
import ConfirmDialog from '../../components/ui/ConfirmDialog'
import type { RecipeInstruction } from '../../types/recipe'

interface InstructionListProps {
  recipeId: number
  instructions: RecipeInstruction[]
  canEdit: boolean
}

export default function InstructionList({ recipeId, instructions, canEdit }: InstructionListProps) {
  const createInstruction = useCreateInstruction(recipeId)
  const updateInstruction = useUpdateInstruction(recipeId)
  const deleteInstruction = useDeleteInstruction(recipeId)

  const [adding, setAdding] = useState(false)
  const [newText, setNewText] = useState('')

  const [editingId, setEditingId] = useState<number | null>(null)
  const [editText, setEditText] = useState('')

  const [confirmDeleteId, setConfirmDeleteId] = useState<number | null>(null)

  const sorted = [...instructions].sort((a, b) => (a.stepNumber ?? 0) - (b.stepNumber ?? 0))

  async function handleAdd() {
    if (!newText.trim()) return
    const nextStep = sorted.length > 0 ? (sorted[sorted.length - 1].stepNumber ?? 0) + 1 : 1
    await createInstruction.mutateAsync({ stepNumber: nextStep, description: newText.trim() })
    setAdding(false)
    setNewText('')
  }

  async function handleUpdate(id: number) {
    const instruction = instructions.find(i => i.id === id)
    if (!instruction || !editText.trim()) return
    await updateInstruction.mutateAsync({
      id,
      data: { id, stepNumber: instruction.stepNumber ?? undefined, description: editText.trim() },
    })
    setEditingId(null)
  }

  async function handleDelete(id: number) {
    await deleteInstruction.mutateAsync(id)
    setConfirmDeleteId(null)
  }

  return (
    <div>
      <div className="mb-2 flex items-center justify-between">
        <h3 className="font-semibold text-gray-900">Instructions</h3>
        {canEdit && !adding && (
          <Button variant="ghost" size="sm" onClick={() => setAdding(true)}>
            + Add
          </Button>
        )}
      </div>

      {sorted.length === 0 && !adding && (
        <p className="text-sm text-gray-500">No instructions added yet.</p>
      )}

      <ol className="space-y-3">
        {sorted.map(instruction => (
          <li key={instruction.id} className="flex gap-3">
            <span className="flex h-6 w-6 shrink-0 items-center justify-center rounded-full bg-blue-100 text-xs font-bold text-blue-700">
              {instruction.stepNumber}
            </span>
            <div className="flex-1">
              {editingId === instruction.id ? (
                <div className="space-y-2">
                  <TextArea
                    value={editText}
                    onChange={e => setEditText(e.target.value)}
                    rows={3}
                  />
                  <div className="flex gap-2">
                    <Button
                      size="sm"
                      onClick={() => handleUpdate(instruction.id)}
                      isLoading={updateInstruction.isPending}
                    >
                      Save
                    </Button>
                    <Button size="sm" variant="ghost" onClick={() => setEditingId(null)}>
                      Cancel
                    </Button>
                  </div>
                </div>
              ) : (
                <div className="flex items-start justify-between gap-2">
                  <p className="text-sm text-gray-700">{instruction.description}</p>
                  {canEdit && (
                    <div className="flex shrink-0 gap-1">
                      <Button
                        variant="ghost"
                        size="sm"
                        onClick={() => {
                          setEditingId(instruction.id)
                          setEditText(instruction.description ?? '')
                        }}
                      >
                        Edit
                      </Button>
                      <Button
                        variant="ghost"
                        size="sm"
                        onClick={() => setConfirmDeleteId(instruction.id)}
                      >
                        Delete
                      </Button>
                    </div>
                  )}
                </div>
              )}
            </div>
          </li>
        ))}
      </ol>

      {adding && (
        <div className="mt-3 space-y-2 rounded-md border border-blue-200 bg-blue-50 p-3">
          <TextArea
            placeholder="Describe this step…"
            value={newText}
            onChange={e => setNewText(e.target.value)}
            rows={3}
          />
          <div className="flex gap-2">
            <Button size="sm" onClick={handleAdd} isLoading={createInstruction.isPending}>
              Add step
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
        title="Delete step"
        message="Remove this instruction step?"
        confirmLabel="Delete"
        isDestructive
        isLoading={deleteInstruction.isPending}
      />
    </div>
  )
}
