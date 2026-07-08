using PlanService.Models;
using Microsoft.EntityFrameworkCore;

namespace PlanService.Repositories;
public class ResourcePermissionRepository : Interfaces.IResourcePermissionRepository
{
    private readonly PlanDbContext _context; 
    public ResourcePermissionRepository(PlanDbContext context)
    {
        _context = context;
    }   
    private IQueryable<ResourcePermission> WithLookups() =>
        _context.ResourcePermissions
            .Include(rp => rp.PermissionType)
            .Include(rp => rp.ResourceType)
            .Include(rp => rp.SubjectType);

    public async Task<ResourcePermission?> GetByIdAsync(Guid id)
    {
        return await WithLookups().FirstOrDefaultAsync(rp => rp.Id == id);
    }
    public async Task<IEnumerable<ResourcePermission>> GetByResourceAsync(Guid resourceId, int resourceTypeId)
    {
        return await WithLookups().Where(rp => rp.ResourceId == resourceId && rp.ResourceTypeId == resourceTypeId).ToListAsync();
    }
    public async Task<IEnumerable<ResourcePermission>> GetBySubjectAsync(Guid subjectId, int subjectTypeId)
    {
        return await WithLookups().Where(rp => rp.SubjectId == subjectId && rp.SubjectTypeId == subjectTypeId).ToListAsync();
    }
    public async Task<IEnumerable<ResourcePermission>> GetByGrantedByAsync(Guid GrantedById)
    {
        return await WithLookups().Where(rp => rp.GrantedByUserId == GrantedById).ToListAsync();
    }
    public async Task<IEnumerable<ResourcePermission>> GetBySubjectTypeAndResourceTypeAsync(Guid subjectId, int subjectTypeId, int resourceTypeId)
    {
        return await WithLookups().Where(rp => rp.SubjectId == subjectId && rp.SubjectTypeId == subjectTypeId && rp.ResourceTypeId == resourceTypeId).ToListAsync();
    }
    public async Task<ResourcePermission?> GetBySubjectAndResourceAsync(Guid subjectId, int subjectTypeId, Guid resourceId, int resourceTypeId)
    {
        return await _context.ResourcePermissions.FirstOrDefaultAsync(rp => rp.SubjectId == subjectId && rp.SubjectTypeId == subjectTypeId && rp.ResourceId == resourceId && rp.ResourceTypeId == resourceTypeId);
    }
    public async Task<IEnumerable<ResourcePermission>> GetByResourceTypeNameAsync(Guid resourceId, string resourceTypeName)
    {
        return await WithLookups().Where(rp => rp.ResourceId == resourceId && rp.ResourceType.Name == resourceTypeName).ToListAsync();
    }
    public async Task<IEnumerable<ResourcePermission>> GetBySubjectTypeNameAsync(Guid subjectId, string subjectTypeName)
    {
        return await WithLookups().Where(rp => rp.SubjectId == subjectId && rp.SubjectType.Name == subjectTypeName).ToListAsync();
    }
    public async Task<IEnumerable<ResourcePermission>> GetBySubjectAndResourceTypeNamesAsync(Guid subjectId, string subjectTypeName, string resourceTypeName)
    {
        return await WithLookups().Where(rp => rp.SubjectId == subjectId && rp.SubjectType.Name == subjectTypeName && rp.ResourceType.Name == resourceTypeName).ToListAsync();
    }
    public async Task<ResourcePermission?> GetBySubjectAndResourceByNamesAsync(Guid subjectId, string subjectTypeName, Guid resourceId, string resourceTypeName)
    {
        return await _context.ResourcePermissions.FirstOrDefaultAsync(rp => rp.SubjectId == subjectId && rp.SubjectType.Name == subjectTypeName && rp.ResourceId == resourceId && rp.ResourceType.Name == resourceTypeName);
    }
    public async Task<ResourcePermission?> CreateAsync(ResourcePermission permission)
    {
        var entry = await _context.ResourcePermissions.AddAsync(permission);
        await _context.SaveChangesAsync();
        return entry.Entity;  
    }
    public async Task<bool> UpdateAsync(ResourcePermission permission)
    {
        var existing = await _context.ResourcePermissions.FindAsync(permission.Id);
        if (existing == null) return false;

        existing.ResourceId = permission.ResourceId;
        existing.ResourceTypeId = permission.ResourceTypeId;
        existing.SubjectId = permission.SubjectId;
        existing.SubjectTypeId = permission.SubjectTypeId;
        existing.PermissionTypeId = permission.PermissionTypeId;
        existing.ExpiresAt = permission.ExpiresAt;
        existing.UpdatedAt = DateTime.UtcNow;

        var result = await _context.SaveChangesAsync();
        return result > 0;
    }
    public async Task<bool> DeleteAsync(Guid id)
    {
        var permission = await _context.ResourcePermissions.FindAsync(id);
        if (permission == null) return false;   

        _context.ResourcePermissions.Remove(permission);
        var result = await _context.SaveChangesAsync();
        return result > 0;  
    }


}

public class ResourceTypeRepository : Interfaces.IResourceTypeRepository
{
    private readonly PlanDbContext _context; 
    public ResourceTypeRepository(PlanDbContext context)
    {
        _context = context;
    }   
    public async Task<ResourceType?> GetByIdAsync(int id)
    {
        return await _context.ResourceTypes.FindAsync(id);
    }
    public async Task<ResourceType?> GetByNameAsync(string name)
    {
        return await _context.ResourceTypes.FirstOrDefaultAsync(rt => rt.Name == name);
    }
    public async Task<IEnumerable<ResourceType>> GetAllAsync()
    {
        return await _context.ResourceTypes.ToListAsync();
    }
    public async Task<ResourceType?> CreateAsync(ResourceType resourceType)
    {
        var entry = await _context.ResourceTypes.AddAsync(resourceType);
        await _context.SaveChangesAsync();
        return entry.Entity;  
    }
    public async Task<bool> UpdateAsync(ResourceType resourceType)
    {
        var existing = await _context.ResourceTypes.FindAsync(resourceType.Id);
        if (existing == null) return false;

        existing.Name = resourceType.Name;
        existing.DisplayName = resourceType.DisplayName;
        existing.SortOrder = resourceType.SortOrder;

        var result = await _context.SaveChangesAsync();
        return result > 0;
    }
    public async Task<bool> DeleteAsync(int id)
    {
        var resourceType = await _context.ResourceTypes.FindAsync(id);
        if (resourceType == null) return false;   

        _context.ResourceTypes.Remove(resourceType);
        var result = await _context.SaveChangesAsync();
        return result > 0;  
    }
}
public class SubjectTypeRepository : Interfaces.ISubjectTypeRepository
{
    private readonly PlanDbContext _context; 
    public SubjectTypeRepository(PlanDbContext context)
    {
        _context = context;
    }   
    public async Task<SubjectType?> GetByIdAsync(int id)
    {
        return await _context.SubjectTypes.FindAsync(id);
    }
    public async Task<SubjectType?> GetByNameAsync(string name)
    {
        return await _context.SubjectTypes.FirstOrDefaultAsync(st => st.Name == name);
    }
    public async Task<IEnumerable<SubjectType>> GetAllAsync()
    {
        return await _context.SubjectTypes.ToListAsync();
    }
    public async Task<SubjectType?> CreateAsync(SubjectType subjectType)
    {
        var entry = await _context.SubjectTypes.AddAsync(subjectType);
        await _context.SaveChangesAsync();
        return entry.Entity;  
    }
    public async Task<bool> UpdateAsync(SubjectType subjectType)
    {
        var existing = await _context.SubjectTypes.FindAsync(subjectType.Id);
        if (existing == null) return false;

        existing.Name = subjectType.Name;
        existing.DisplayName = subjectType.DisplayName;
        existing.SortOrder = subjectType.SortOrder;

        var result = await _context.SaveChangesAsync();
        return result > 0;
    }
    public async Task<bool> DeleteAsync(int id)
    {
        var subjectType = await _context.SubjectTypes.FindAsync(id);
        if (subjectType == null) return false;   

        _context.SubjectTypes.Remove(subjectType);
        var result = await _context.SaveChangesAsync();
        return result > 0;  
    }
}
public class PermissionTypeRepository : Interfaces.IPermissionTypeRepository
{
    private readonly PlanDbContext _context; 
    public PermissionTypeRepository(PlanDbContext context)
    {
        _context = context;
    }  
    public async Task<PermissionType?> GetByIdAsync(int id) 
    {
        return await _context.PermissionTypes.FindAsync(id);
    }  
    public async Task<PermissionType?> GetByNameAsync(string name)
    {
        return await _context.PermissionTypes.FirstOrDefaultAsync(pt => pt.Name == name);
    }
    public async Task<IEnumerable<PermissionType>> GetAllAsync()
    {
        return await _context.PermissionTypes.ToListAsync();
    }
    public async Task<PermissionType?> CreateAsync(PermissionType permissionType)
    {
        var entry = await _context.PermissionTypes.AddAsync(permissionType);
        await _context.SaveChangesAsync();
        return entry.Entity;  
    }
    public async Task<bool> UpdateAsync(PermissionType permissionType)
    {
        var existing = await _context.PermissionTypes.FindAsync(permissionType.Id);
        if (existing == null) return false;     

        existing.Name = permissionType.Name;
        existing.DisplayName = permissionType.DisplayName;
        existing.SortOrder = permissionType.SortOrder;

        var result = await _context.SaveChangesAsync();
        return result > 0;
    }
    public async Task<bool> DeleteAsync(int id)
    {
        var permissionType = await _context.PermissionTypes.FindAsync(id);
        if (permissionType == null) return false;       

        _context.PermissionTypes.Remove(permissionType);
        var result = await _context.SaveChangesAsync();
        return result > 0;
    }
}             