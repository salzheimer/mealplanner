public interface IRecipeRepository
{
    Task<Recipe?> GetByIdAsync(int id);
    Task<IEnumerable<Recipe>> GetAllAsync();
    Task<Recipe> CreateAsync(Recipe recipe);
    Task UpdateAsync(Recipe recipe);
    Task DeleteAsync(int id);
}