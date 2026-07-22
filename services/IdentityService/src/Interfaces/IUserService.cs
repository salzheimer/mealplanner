using Shared.Models;

namespace IdentityService.Interfaces;
public interface IUserService
{
    Task<Result<UserResponse>> CreateUserAsync(CreateUserRequest userDto);
    Task<Result<UserResponse>> FindByEmail(string email);
    Task<Result<UserResponse>> FindById(Guid id);
    Task<Result<IEnumerable<UserResponse>>> GetAllUsersAsync();
    Task<Result<string>> ValidatePassword(string password);
    Task<bool> ValidateCredentials(string email, string password);
}