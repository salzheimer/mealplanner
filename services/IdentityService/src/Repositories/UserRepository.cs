using Microsoft.EntityFrameworkCore;
using IdentityService.Models;

namespace IdentityService.Repositories;

public partial class UserRepository : IUserRepository
{
    private readonly UserContext _context;

    public UserRepository(UserContext context)
    {
        _context = context;
    }
    public async Task<int> Create(User user)
    {
        _context.Users.Add(user);

        return await _context.SaveChangesAsync();

    }

    public async Task<User?> GetUser(int id)
    {
        return await _context.Users.FindAsync(id);

    }
    public async Task<User?> GetUser(string email)
    {
        
        var dbUser = await _context.Users.FirstOrDefaultAsync(o => o.Email == email);
        
        return dbUser;
    }
    public async Task<int> Update(int id, User user)
    {
        var dbUser = await _context.Users.FindAsync(id);
        if (dbUser == null)
            throw new Exception("User not found");
        //todo: add field changed checker
        dbUser = user;
        _context.Users.Update(dbUser);
        return await _context.SaveChangesAsync();
    }


}