using IdentityService.Interfaces;
using IdentityService.Models;
using Microsoft.EntityFrameworkCore;

namespace IdentityService.Repositories;

public class ResourcePermissionRepository : IResourcePermissionRepository
{
    private readonly UserContext _context;

    public ResourcePermissionRepository(UserContext context)
    {
        _context = context;
    }

    public async Task<ResourcePermission?> GetPermissionAsync(ResourceType resourceType, int resourceId, SubjectType subjectType, int subjectId)
    {
        return await _context.ResourcePermissions
            .FirstOrDefaultAsync(p => p.ResourceType == resourceType && p.ResourceId == resourceId && p.SubjectType == subjectType && p.SubjectId == subjectId);
    }

    public async Task<IEnumerable<ResourcePermission>> GetPermissionsForResourceAsync(ResourceType resourceType, int resourceId)
    {
        return await _context.ResourcePermissions
            .Where(p => p.ResourceType == resourceType && p.ResourceId == resourceId)
            .ToListAsync();
    }

    public async Task<IEnumerable<ResourcePermission>> GetPermissionsForSubjectAsync(SubjectType subjectType, int subjectId)
    {
        return await _context.ResourcePermissions
            .Where(p => p.SubjectType == subjectType && p.SubjectId == subjectId)
            .ToListAsync();
    }

    public async Task<ResourcePermission?> AddPermissionAsync(ResourcePermission permission)
    {
        _context.ResourcePermissions.Add(permission);
        var result = await _context.SaveChangesAsync();
        return result > 0 ? permission : null;
    }

    public async Task<bool> UpdatePermissionAsync(ResourcePermission permission)
    {
        _context.ResourcePermissions.Update(permission);
        var result = await _context.SaveChangesAsync();
        return result > 0;
    }

    public async Task<bool> DeletePermissionAsync(long permissionId)
    {
        var permission = await _context.ResourcePermissions.FindAsync(permissionId);
        if (permission != null)
        {
            _context.ResourcePermissions.Remove(permission);
            var result = await _context.SaveChangesAsync();
            return result > 0;
        }
        return false;
    }
}
