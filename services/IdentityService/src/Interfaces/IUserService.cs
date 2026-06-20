using Shared.Models;

namespace IdentityService.Interfaces;
public interface IUserService
{
    Task<Result<UserResponseDto>> CreateUserAsync(CreateUserDto userDto);
    Task<Result<UserResponseDto>> FindByEmail(string email);
    Task<Result<UserResponseDto>> FindById(Guid id);
    Task<Result<string>> ValidatePassword(string password);
    Task<bool> ValidateCredentials(string email, string password);
}