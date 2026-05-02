using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

public class PlanContext : DbContext
{
    public PlanContext(DbContextOptions<PlanContext> options) : base(options)
    {
    }

    public DbSet<PlanService.Models.Plan> Plans { get; set; }
    public DbSet<PlanService.Models.PlanShare> PlanShares { get; set; }
    public DbSet<PlanService.Models.MealItemPlan> MealItemPlans { get; set; }   
    public DbSet<PlanService.Models.MealPlan> MealPlans { get; set; }
   
}