using Shared.Models;

namespace IdentityService.Interfaces;
public interface IUserService
{
    Task<Result<UserResponseDto>> CreateUserAsync(CreateUserDto userDto);
    Task<Result<UserResponseDto?>> FindByEmail(string email);
    Task<bool> ValidateCredentials(string email, string password);
}