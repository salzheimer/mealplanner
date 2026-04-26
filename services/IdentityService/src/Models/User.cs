using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IdentityService.Models;

[Table("user")]
public class User
{
   public User(string email,string? displayName)
    {
        
        this.DisplayName = displayName;
        this.Email = email;
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
//   public User(string Email,string password,string? DisplayName)
//     {
        
//         this.DisplayName = DisplayName;
//         this.Email = Email;
//         this.EmailVerified = false;
//         this.EmailVerifiedAt = null;
//         this.CreatedAt = DateTimeOffset.UtcNow;
//         this.UpdatedAt = DateTimeOffset.UtcNow;
//         this.LastLoginAt = null;
//         this.Auth0Id = null;
//         this.IsActive = true;
//         this.FailedLoginAttempts = 0;
//         this.LockedUntil = null;
//         this.TermsAcceptedAt = null;
//         this.TermsVersion = null;
//         this.SecurityStamp = Guid.NewGuid().ToString();
//          this.Credentials = new UserCredentials( Id, BCrypt.Net.BCrypt.HashPassword(password), "bcrypt", DateTimeOffset.UtcNow, DateTimeOffset.UtcNow);
//     } 
    [Key]
    [Column("id")]
    public int Id{get;set;}
    [Column("display_name")]
    public string? DisplayName{get;set;}
    [Column("email")]
    public string Email{get;set;}
    [Column("email_verified")]
    public bool? EmailVerified{get;set;}
    [Column("email_verified_at")]
    public DateTimeOffset? EmailVerifiedAt{get;set;}
    [Column("created_at")]
    public DateTimeOffset CreatedAt{get;set;}
    [Column("updated_at")]
    public DateTimeOffset UpdatedAt{get;set;}
    [Column("last_login_at")]
    public DateTimeOffset? LastLoginAt{get;set;}
    [Column("auth0_id")]
    public string? Auth0Id{get;set;}
    [Column("is_active")]
    public bool? IsActive{get;set;}
    [Column("failed_login_attempts")]
    public int? FailedLoginAttempts{get;set;}
    [Column("locked_until")]
    public DateTimeOffset? LockedUntil{get;set;}
    [Column("terms_accepted_at")]
    public DateTimeOffset? TermsAcceptedAt{get;set;}
    [Column("terms_version")]
    public string? TermsVersion{get;set;}
    [Column("security_stamp")]
    public string? SecurityStamp{get;set;}
     
   // public UserCredentials? Credentials{get;set;}= new UserCredentials();
}


