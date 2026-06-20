using IdentityService.Models;
using Microsoft.EntityFrameworkCore;

public class UserContext : DbContext
{
    public UserContext(DbContextOptions<UserContext> options) : base(options)
    {
    }

    public DbSet<IdentityService.Models.User> Users { get; set; }
    public DbSet<IdentityService.Models.UserCredentials> UserCredentials { get; set; }
    public DbSet<IdentityService.Models.ResourcePermission> ResourcePermissions { get; set; }
    public DbSet<Session> Sessions { get; set; }
    public DbSet<IdentityService.Models.ClientTypes> ClientType { get; set; }
    public DbSet<IdentityService.Models.GroupMemberRole> GroupMemberRoles { get; set; }
    public DbSet<IdentityService.Models.GroupMemberStatus> GroupMemberStatuses { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<IdentityService.Models.User>()
            .HasIndex(u => u.Email)
            .IsUnique();

        modelBuilder.Entity<IdentityService.Models.UserCredentials>()
            .HasOne<IdentityService.Models.User>()
            .WithOne()
            .HasForeignKey<IdentityService.Models.UserCredentials>(uc => uc.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Session>()
            .HasOne<ClientTypes>()
            .WithMany()
            .HasForeignKey(s => s.ClientTypeId);
    }
}