using AuthService.Models;

namespace AuthService.Services;

public class UserStore
{
    private readonly List<User> _users = new();
    private readonly List<UserCredential> _credentials = new();
    private int _nextUserId = 1;
   

    public UserStore()
    {
        // Seed with a default user
        AddUser( "admin@example.com","P@ssw0rd","admin");
    }

   

    public virtual User? FindByEmail(string email) =>
        _users.FirstOrDefault(u => u.Email == email);

    public virtual User AddUser( string email, string password,string? displayname =null)
    {
        var now = DateTimeOffset.UtcNow;
        var user = new User(
            Id: _nextUserId++,
            DisplayName: displayname,
            Email: email
        );

        var credential = new UserCredential(
            Id:Guid.NewGuid(),
            UserId: user.Id,
            PasswordHash: BCrypt.Net.BCrypt.HashPassword(password),
            HashAlgorithm: "bcrypt",
            CreatedAt: now,
            UpdatedAt: now
        );

        _users.Add(user);
        _credentials.Add(credential);
        return user;
    }

    public virtual bool ValidateCredentials(string email, string password)
    {
        var user = FindByEmail(email);
        if (user is null) return false;

        var credential = _credentials.FirstOrDefault(c => c.UserId == user.Id);
        return credential is not null && BCrypt.Net.BCrypt.Verify(password, credential.PasswordHash);
    }
}
