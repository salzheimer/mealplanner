using IdentityService.Models;


namespace IdentityService.Repositories;
public partial interface IUserCredentialsRepository
{
    Task<UserCredentials> GetUserCredentials(int userId);
    Task<int> CreateCredentials(UserCredentials credentials);
}