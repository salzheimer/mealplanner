using IdentityService.Models;
using Microsoft.EntityFrameworkCore;

public class IdentityDbContext : DbContext
{
    public IdentityDbContext(DbContextOptions<IdentityDbContext> options) : base(options)
    {
    }

    public DbSet<IdentityService.Models.User> Users { get; set; }
    public DbSet<IdentityService.Models.UserCredentials> UserCredentials { get; set; }
    
    public DbSet<Session> Sessions { get; set; }
    public DbSet<IdentityService.Models.ClientTypes> ClientType { get; set; }
    public DbSet<IdentityService.Models.GroupMemberRoleType> GroupMemberRoleTypes { get; set; }
    public DbSet<IdentityService.Models.GroupMemberStatusType> GroupMemberStatusTypes { get; set; }
    public DbSet<IdentityService.Models.Group> Groups { get; set; }
    public DbSet<IdentityService.Models.GroupMember> GroupMembers { get; set; }

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

        modelBuilder.Entity<GroupMember>()
            .HasOne(m => m.Group)
            .WithMany(g => g.GroupMembers)
            .HasForeignKey(m => m.GroupId);

        modelBuilder.Entity<GroupMember>()
            .HasOne(m => m.GroupMemberRoleType)
            .WithMany()
            .HasForeignKey(m => m.RoleId);

        modelBuilder.Entity<GroupMember>()
            .HasOne(m => m.GroupMemberStatusType)
            .WithMany()
            .HasForeignKey(m => m.StatusId);
    }
}