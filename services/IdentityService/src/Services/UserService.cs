using System.IO.Pipelines;
using IdentityService.Repositories;
using IdentityService.Models;
using Shared.Models;

namespace IdentityService.Services;

public interface IUserService
{
    Task<Result<UserResponseDto>> CreateUserAsync(CreateUserDto userDto);
    Task<Result<UserResponseDto?>> FindByEmail(string email);
    Task<bool> ValidateCredentials(string email, string password);
}
public class UserService : IUserService
{

    private readonly IUserRepository _userRepository;
    private readonly IUserCredentialsRepository _credentialsRepository;
    public UserService(IUserRepository userRepository, IUserCredentialsRepository credentialsRepository)
    {
        _userRepository = userRepository;
        _credentialsRepository = credentialsRepository;
    }

    public async Task<Result<UserResponseDto>> CreateUserAsync(CreateUserDto userDto)
    {
        var now = DateTimeOffset.UtcNow;
        var user = new Models.User(

            displayName: userDto.DisplayName,
            email: userDto.Email
        );
       
        await _userRepository.Create(user);
       
        var newUser = await _userRepository.GetUser(user.Email);
       
        var credential = new UserCredentials(

            userId: newUser.Id,
            passwordHash: BCrypt.Net.BCrypt.HashPassword(userDto.Password),
            hashAlgorithm: "bcrypt",
            createdAt: now,
            updatedAt: now
        );
       
        await _credentialsRepository.CreateCredentials(credential);
        var response = new UserResponseDto(user.Id, user.Email, user.DisplayName);
        return Result<UserResponseDto>.Success(response);
    }

    public async Task<Result<UserResponseDto?>> FindByEmail(string email)
    {

        var user = await _userRepository.GetUser(email);
        if (user is null) return null;
        return Result<UserResponseDto?>.Success(new UserResponseDto(user.Id, user.Email, user.DisplayName));
    }

    public async Task<bool> ValidateCredentials(string email, string password)
    {
        var user = FindByEmail(email);
        if (user is null) return false;

        var credential = await _credentialsRepository.GetUserCredentials(user.Id);
        return credential is not null && BCrypt.Net.BCrypt.Verify(password, credential.PasswordHash);
    }
}