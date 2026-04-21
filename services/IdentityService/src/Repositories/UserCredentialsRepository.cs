using Microsoft.EntityFrameworkCore;
using IdentityService.Models;

namespace IdentityService.Repositories;
public partial class UserCredentialsRepository:IUserCredentialsRepository
{
     private readonly UserContext _context;

    public UserCredentialsRepository(UserContext context)
    {
        _context = context;
    }
    public async  Task<int> CreateCredentials(UserCredentials credentials)
    {
        
        _context.UserCredentials.Add(credentials);

        return await _context.SaveChangesAsync();
    }
    public async Task<UserCredentials> GetUserCredentials(int userId)
    {
        var creds =  await _context.UserCredentials.FirstOrDefaultAsync(o=>o.UserId == userId);

        return creds;
    
    }
}