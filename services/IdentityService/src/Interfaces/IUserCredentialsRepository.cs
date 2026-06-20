using IdentityService.Models;


namespace IdentityService.Interfaces;
public partial interface IUserCredentialsRepository
{
    Task<UserCredentials> GetUserCredentials(Guid userId);
    Task<int> CreateCredentials(UserCredentials credentials);
}