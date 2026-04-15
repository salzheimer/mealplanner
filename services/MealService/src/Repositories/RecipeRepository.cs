public class RecipeRepository : IRecipeRepository
{
    public Task<Recipe?> GetByIdAsync(int id)
    {
       throw new NotImplementedException();
    }

    public Task<IEnumerable<Recipe>> GetAllAsync()
    {
        throw new NotImplementedException();
    }    
    public Task<Recipe> CreateAsync(Recipe recipe)
    {
        throw new NotImplementedException();
    }   
    public Task UpdateAsync(Recipe recipe)
    {
        throw new NotImplementedException();
    }
    public Task DeleteAsync(int id)
    {
        throw new NotImplementedException();
    }
}                      