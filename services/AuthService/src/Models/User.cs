namespace AuthService.Models;

public class User
{
   public User(int Id,string Email,string? DisplayName)
    {
        this.Id = Id;
        this.DisplayName = DisplayName;
        this.Email = Email;
        this.EmailVerified = false;
        this.EmailVerifiedAt = null;
        this.CreatedAt = DateTimeOffset.UtcNow;
        this.UpdatedAt = DateTimeOffset.UtcNow;
        this.LastLoginAt = null;
        this.Auth0Id = null;
        this.IsActive = true;
        this.FailedLoginAttempts = 0;
        this.LockedUntil = null;
        this.TermsAcceptedAt = null;
        this.TermsVersion = null;
        this.SecurityStamp = Guid.NewGuid().ToString();
         
    } 
  public User(int Id,string Email,string password,string? DisplayName)
    {
        this.Id = Id;
        this.DisplayName = DisplayName;
        this.Email = Email;
        this.EmailVerified = false;
        this.EmailVerifiedAt = null;
        this.CreatedAt = DateTimeOffset.UtcNow;
        this.UpdatedAt = DateTimeOffset.UtcNow;
        this.LastLoginAt = null;
        this.Auth0Id = null;
        this.IsActive = true;
        this.FailedLoginAttempts = 0;
        this.LockedUntil = null;
        this.TermsAcceptedAt = null;
        this.TermsVersion = null;
        this.SecurityStamp = Guid.NewGuid().ToString();
         this.Credentials = new UserCredential(Guid.NewGuid(), Id, BCrypt.Net.BCrypt.HashPassword(password), "bcrypt", DateTimeOffset.UtcNow, DateTimeOffset.UtcNow);
    } 
    public int Id;
    public string? DisplayName;
    public string Email;
    public bool? EmailVerified;
    public DateTimeOffset? EmailVerifiedAt;
    public DateTimeOffset CreatedAt;
    public DateTimeOffset UpdatedAt;
    public DateTimeOffset? LastLoginAt;
    public string? Auth0Id;
    public bool? IsActive;
    public int? FailedLoginAttempts;
    public DateTimeOffset? LockedUntil;
    public DateTimeOffset? TermsAcceptedAt;
    public string? TermsVersion;
    public string? SecurityStamp;
    public UserCredential? Credentials;
}


public class UserCredential
{
    public UserCredential(Guid Id, int UserId, string PasswordHash, string HashAlgorithm, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt)
    {
        this.Id = Id;
        this.UserId = UserId;
        this.PasswordHash = PasswordHash;
        this.HashAlgorithm = HashAlgorithm;
        this.CreatedAt = CreatedAt;
        this.UpdatedAt = UpdatedAt;
    }
   public Guid Id;
   public int UserId;
   public string PasswordHash;
   public  string HashAlgorithm;
   public DateTimeOffset CreatedAt;
   public DateTimeOffset UpdatedAt;
}
