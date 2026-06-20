using IdentityService.Models;


namespace IdentityService.Interfaces;

public partial interface IUserRepository
{

    Task<int> Create(User user);
    Task<User?> GetUser(Guid id);
    Task<User?> GetUser(string email);
    Task<int> Update(Guid id, User user);

    

}
