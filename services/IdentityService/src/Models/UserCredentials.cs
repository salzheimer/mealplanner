using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IdentityService.Models;
[Table("user_credential")]
public class UserCredentials
{
    public UserCredentials( int userId, string passwordHash, string hashAlgorithm, DateTimeOffset createdAt, DateTimeOffset updatedAt)
    {
        this.Id =Guid.NewGuid();
        this.UserId = userId;
        this.PasswordHash = passwordHash;
        this.HashAlgorithm = hashAlgorithm;
        this.CreatedAt = createdAt;
        this.UpdatedAt = updatedAt;
    }
    public UserCredentials(){}
    [Key]
    [Column("id")]
    public Guid Id {get;set;}
    [Column("user_id")]
    public int UserId{get;set;}
    [Column("password_hash")]
    public string PasswordHash{get;set;}
    [Column("hash_algorithm")]
    public string HashAlgorithm{get;set;}
    [Column("created_at")]
    public DateTimeOffset CreatedAt{get;set;}
    [Column("updated_at")]
    public DateTimeOffset UpdatedAt{get;set;}
}
