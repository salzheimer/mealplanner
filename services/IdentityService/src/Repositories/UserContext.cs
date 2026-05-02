using IdentityService.Models;
using Microsoft.EntityFrameworkCore;

public class UserContext : DbContext
{
    public UserContext(DbContextOptions<UserContext> options) : base(options)
    {
    }

    public DbSet<IdentityService.Models.User> Users { get; set; }
    public DbSet<IdentityService.Models.UserCredentials> UserCredentials { get; set; }
    public DbSet<Session> Sessions { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasPostgresEnum<Shared.Models.ClientType>("client_type_enum");
    }
}