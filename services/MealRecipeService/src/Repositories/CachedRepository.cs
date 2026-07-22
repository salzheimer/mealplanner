using MealRecipeService.Models;
using Microsoft.EntityFrameworkCore;


namespace MealRecipeService.Repositories;

public class CachedUserRepository : Interfaces.ICachedUserRepository
{
    private readonly MealRecipeDbContext _context;
    public CachedUserRepository(MealRecipeDbContext context)
    {
        _context = context;
    }
    public async Task<bool> AnyAsync()
    {
        return await _context.CachedUsers.AnyAsync();
    }

    public async Task<CachedUser?> GetByIdAsync(Guid id)
    {
        return await _context.CachedUsers.FindAsync(id);
    }

    public async Task<CachedUser?> CreateAsync(CachedUser user)
    {
        _context.CachedUsers.Add(user);
        var result = await _context.SaveChangesAsync();
        if (result <= 0) return null;
        return user;
    }

    public async Task<bool> UpdateAsync(CachedUser user)
    {
        var result =  await _context.CachedUsers.Where(cu=>cu.Id ==user.Id)
        .ExecuteUpdateAsync(s=>s.SetProperty(
            cu=>cu.DisplayName, user.DisplayName
        ).SetProperty(cu=> cu.SyncedAt, user.SyncedAt)
        .SetProperty(cu=>cu.SourceUpdatedAt, user.SourceUpdatedAt));
       // _context.Entry(user).State = EntityState.Modified;
        //var result = await _context.SaveChangesAsync();
        return result > 0;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var existing = await _context.CachedUsers.FindAsync(id);
        if (existing == null) return false;
        _context.CachedUsers.Remove(existing);
        return await _context.SaveChangesAsync() > 0;
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

    public async Task<bool> AnyAsync()
    {
        return await _context.CachedGroups.AnyAsync();
    }

    public async Task<CachedGroup?> CreateAsync(CachedGroup group)
    {
        _context.CachedGroups.Add(group);
        var result = await _context.SaveChangesAsync();
        if (result <= 0) return null;
        return group;
    }

    public async Task<bool> UpdateAsync(CachedGroup group)
    {
        var result = await _context.CachedGroups.Where(cg=>cg.Id ==group.Id)
        .ExecuteUpdateAsync(s=>s.SetProperty(
            cg=>cg.GroupName, group.GroupName
        ).SetProperty(cg=> cg.SyncedAt, group.SyncedAt)
        .SetProperty(cg=>cg.SourceUpdatedAt, group.SourceUpdatedAt));
       // _context.Entry(group).State = EntityState.Modified;
       // var result = await _context.SaveChangesAsync();
        return result > 0;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var existing = await _context.CachedGroups.FindAsync(id);
        if (existing == null) return false;
        _context.CachedGroups.Remove(existing);
        return await _context.SaveChangesAsync() > 0;
    }
}

public class CachedGroupMemberRepository : Interfaces.ICachedGroupMemberRepository
{
    private readonly MealRecipeDbContext _context;
    public CachedGroupMemberRepository(MealRecipeDbContext context)
    {
        _context = context;
    }

    public async Task<bool> AnyAsync()
    {
        return await _context.CachedGroupMembers.AnyAsync();
    }
    public async Task<CachedGroupMember?> GetByIdAsync(Guid id)
    {
        return await _context.CachedGroupMembers.FindAsync(id);
    }



    public async Task<CachedGroupMember?> CreateAsync(CachedGroupMember groupMember)
    {
        _context.CachedGroupMembers.Add(groupMember);
        var result = await _context.SaveChangesAsync();
        if (result <= 0) return null;
        return groupMember;
    }

    public async Task<bool> UpdateAsync(CachedGroupMember groupMember)
    {
        _context.Entry(groupMember).State = EntityState.Modified;
        var result = await _context.SaveChangesAsync();
        return result > 0;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var existing = await _context.CachedGroupMembers.FindAsync(id);
        if (existing == null) return false;
        _context.CachedGroupMembers.Remove(existing);
        return await _context.SaveChangesAsync() > 0;
    }
}