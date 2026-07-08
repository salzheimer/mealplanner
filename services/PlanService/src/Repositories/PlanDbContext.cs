using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

public class PlanDbContext : DbContext
{
    public PlanDbContext(DbContextOptions<PlanDbContext> options) : base(options)
    {
    }

    public DbSet<PlanService.Models.Plan> Plans { get; set; }
    public DbSet<PlanService.Models.PlanMealItem> MealItemPlans { get; set; }
    public DbSet<PlanService.Models.PlanMeal> MealPlans { get; set; }
    public DbSet<PlanService.Models.ResourcePermission> ResourcePermissions { get; set; }

    //Lookup Tables
    public DbSet<PlanService.Models.ResourceType> ResourceTypes { get; set; }
    public DbSet<PlanService.Models.SubjectType> SubjectTypes { get; set; }
    public DbSet<PlanService.Models.PermissionType> PermissionTypes { get; set; }

    public DbSet<PlanService.Models.MealItemPlanStatusType> MealItemPlanStatusTypes { get; set; }
    //Cache identity sets
    public DbSet<PlanService.Models.CachedUser> CachedUsers { get; set; }
    public DbSet<PlanService.Models.CachedGroup> CachedGroups { get; set; }
    public DbSet<PlanService.Models.CachedGroupMember> CachedGroupMembers { get; set; }

}