using Microsoft.EntityFrameworkCore;
using PlanService.Models;


namespace PlanService.Repositories;

public class CachedUserRepository : Interfaces.ICachedUserRepository
{
    private readonly PlanDbContext _context;
    public CachedUserRepository(PlanDbContext context)
    {
        _context = context;
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
        _context.Entry(user).State = EntityState.Modified;
        var result = await _context.SaveChangesAsync();
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
    private readonly PlanDbContext _context;
    public CachedGroupRepository(PlanDbContext context)
    {
        _context = context;
    }


    public async Task<CachedGroup?> GetByIdAsync(Guid id)
    {
        return await _context.CachedGroups.FindAsync(id);
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
        _context.Entry(group).State = EntityState.Modified;
        var result = await _context.SaveChangesAsync();
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
    private readonly PlanDbContext _context;
    public CachedGroupMemberRepository(PlanDbContext context)
    {
        _context = context;
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