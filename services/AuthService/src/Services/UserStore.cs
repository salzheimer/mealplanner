using AuthService.Models;

namespace AuthService.Services;

public class UserStore
{
    private readonly List<User> _users = new();
    private int _nextId = 1;

    public UserStore()
    {
        // Seed with a default user
        AddUser("admin", "P@ssw0rd", "admin@example.com");
    }

    public User? FindByUsername(string username) => _users.FirstOrDefault(u => u.Username == username);

    public User AddUser(string username, string password, string? email = null)
    {
        var user = new User(_nextId++, username, BCrypt.Net.BCrypt.HashPassword(password), email);
        _users.Add(user);
        return user;
    }

    public bool ValidateCredentials(string username, string password)
    {
        var user = FindByUsername(username);
        return user is not null && BCrypt.Net.BCrypt.Verify(password, user.PasswordHash);
    }
}
