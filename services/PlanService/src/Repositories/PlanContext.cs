using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

public class PlanContext : DbContext
{
    public PlanContext(DbContextOptions<PlanContext> options) : base(options)
    {
    }

    public DbSet<PlanService.Models.Plan> Plans { get; set; }
   
}