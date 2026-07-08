using Microsoft.EntityFrameworkCore;
using IdentityService.Models;

namespace IdentityService.Repositories;

public partial class UserRepository : Interfaces.IUserRepository
{
    private readonly IdentityDbContext _context;

    public UserRepository(IdentityDbContext context)
    {
        _context = context;
    }
    public async Task<int> Create(User user)
    {
        _context.Users.Add(user);

        return await _context.SaveChangesAsync();

    }

    public async Task<User?> GetUser(Guid id)
    {
        return await _context.Users.FindAsync(id);

    }
    public async Task<User?> GetUser(string email)
    {
        
        var dbUser = await _context.Users.FirstOrDefaultAsync(o => o.Email == email);
        
        return dbUser;
    }
    public async Task<bool> Update(Guid id, User user)
    {
        var dbUser = await _context.Users.FindAsync(id);
        if (dbUser == null)
            throw new Exception("User not found");
        //todo: add field changed checker
        dbUser = user;
        _context.Users.Update(dbUser);
        return await _context.SaveChangesAsync() >0;
    }

    public async Task<bool> Delete(Guid userId)
    {
        var existing = await _context.Users.FindAsync(userId);
        if(existing ==null) return false;
        _context.Users.Remove(existing);
        return await _context.SaveChangesAsync()>0;
    }
}