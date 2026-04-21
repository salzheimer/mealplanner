using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

public class UserContext : DbContext
{
    public UserContext(DbContextOptions<UserContext> options) : base(options)
    {
    }

    public DbSet<IdentityService.Models.User> Users { get; set; }
    public DbSet<IdentityService.Models.UserCredentials> UserCredentials {get;set;}
}