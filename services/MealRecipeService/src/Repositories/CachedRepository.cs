using MealRecipeService.Models;


namespace MealRecipeService.Repositories;

public class CachedUserRepository : Interfaces.ICachedUserRepository
{
    private readonly MealRecipeDbContext _context;
    public CachedUserRepository(MealRecipeDbContext context)
    {
        _context = context;
    }
    

    public async Task<CachedUser?> GetByIdAsync(Guid id)
    {
        return await _context.CachedUsers.FindAsync(id);
    }
     
    public Task<CachedUser?> CreateAsync(CachedUser user)
    {
        throw new NotImplementedException("Create is not supported in CachedUserRepository");
    }

    public Task<bool> UpdateAsync(CachedUser user)
    {
        throw new NotImplementedException("Update is not supported in CachedUserRepository");
    }

    public Task<bool> DeleteAsync(Guid id)
    {
        throw new NotImplementedException("Delete is not supported in CachedUserRepository");
    }
}

public class CachedGroupRepository : Interfaces.ICachedGroupRepository
{
    private readonly MealRecipeDbContext _context;
    public CachedGroupRepository(MealRecipeDbContext context)
    {
        _context = context;
    }
    

    public async Task<CachedGroup?> GetByIdAsync(Guid id)
    {
        return await _context.CachedGroups.FindAsync(id);
    }

     
     
    public Task<CachedGroup?> CreateAsync(CachedGroup group)
    {
        throw new NotImplementedException("Create is not supported in CachedGroupRepository");
    }

    public Task<bool> UpdateAsync(CachedGroup group)
    {
        throw new NotImplementedException("Update is not supported in CachedGroupRepository");
    }

    public Task<bool> DeleteAsync(Guid id)
    {
        throw new NotImplementedException("Delete is not supported in CachedGroupRepository");
    }
}