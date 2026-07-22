using System.IO.Pipelines;
using IdentityService.Interfaces;
using IdentityService.Models;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration.UserSecrets;
using Rebus.Bus;
using Shared.Models;

namespace IdentityService.Services;


public class UserService : IUserService
{

    private readonly IUserRepository _userRepository;
    private readonly IUserCredentialsRepository _credentialsRepository;
    private readonly IBus _bus;
    public UserService(IUserRepository userRepository, IUserCredentialsRepository credentialsRepository, IBus bus)
    {
        _userRepository = userRepository;
        _credentialsRepository = credentialsRepository;
        _bus = bus;
    }

    public async Task<Result<UserResponse>> CreateUserAsync(CreateUserRequest createUserRequest)
    {
        var now = DateTimeOffset.UtcNow;
        var user = new User()
        {

            DisplayName = createUserRequest.DisplayName,
            Email = createUserRequest.Email,
            EmailVerified=false,
            IsActive =true,
            CreatedAt = now,
            UpdatedAt = now,
        };

        await _userRepository.Create(user);

        var newUser = await _userRepository.GetUser(user.Email);

        var credential = new UserCredentials(

            userId: newUser.Id,
            passwordHash: BCrypt.Net.BCrypt.HashPassword(createUserRequest.Password),
            hashAlgorithm: "bcrypt",
            createdAt: now,
            updatedAt: now
        );

        await _credentialsRepository.CreateCredentials(credential);
        //publish to Rabbit MQ
        await _bus.Publish(new UserChanged { UserId = newUser.Id, DisplayName = string.IsNullOrEmpty(newUser.DisplayName) ? string.Empty : newUser.DisplayName, SourceUpdatedAt=now, Action = "Created" });
        var response = new UserResponse(user.Id, user.Email, String.IsNullOrEmpty(user.DisplayName) ? string.Empty : user.DisplayName);
        return Result<UserResponse>.Success(response);
    }

    public async Task<Result<UserResponse>> UpdateUserProfileAsync(UpdateUserProfileRequest userProfileRequest)
    {

        var user = new User
        {
            DisplayName = userProfileRequest.DisplayName,
            UpdatedAt = DateTimeOffset.UtcNow
        };
        var updated = await _userRepository.Update(userProfileRequest.Id, user);

        if (!updated) return Result<UserResponse>.Failure(UserErrors.UpdateFailed);

        var dbUser = await _userRepository.GetUser(userProfileRequest.Id);

        await _bus.Publish(new UserChanged { UserId = dbUser!.Id, DisplayName = dbUser.DisplayName!, SourceUpdatedAt=user.UpdatedAt, Action = "Updated" });
        return Result<UserResponse>.Success(new UserResponse(dbUser!.Id, dbUser.Email, dbUser.DisplayName!));
    }
    public async Task<Result<bool>> DeleteUserAsync(Guid userId)
    {
        var deleted = await _userRepository.Delete(userId);
        if (!deleted) return Result<bool>.Failure(UserErrors.DeleteFailed);

        await _bus.Publish(new UserChanged { UserId = userId, Action = "Deleted" });
        return Result<bool>.Success(true);
    }
    public async Task<Result<UserResponse>> FindByEmail(string email)
    {

        var user = await _userRepository.GetUser(email);
        if (user is null)
        {
            return Result<UserResponse>.Failure(UserErrors.NotFound);
        }
        return Result<UserResponse>.Success(new UserResponse(user.Id, user.Email, string.IsNullOrEmpty(user.DisplayName) ? string.Empty : user.DisplayName));
    }
    public async Task<Result<IEnumerable<UserResponse>>> GetAllUsersAsync()
    {
        IEnumerable<UserResponse> userResponses = Enumerable.Empty<UserResponse>();
        var users = await _userRepository.GetAllUsersAsync();
        if (users.Any())
        {
            userResponses = users.Select(u =>
                new UserResponse(Id: u.Id, Email: u.Email, DisplayName: string.IsNullOrEmpty(u.DisplayName) == false ? u.DisplayName : string.Empty)

            );
        }

        return Result<IEnumerable<UserResponse>>.Success(userResponses);
    }
    public async Task<Result<UserResponse>> FindById(Guid id)
    {
        var user = await _userRepository.GetUser(id);
        if (user is null) return Result<UserResponse>.Failure(UserErrors.NotFound);
        return Result<UserResponse>.Success(new UserResponse(user.Id, user.Email, string.IsNullOrEmpty(user.DisplayName) ? string.Empty : user.DisplayName));
    }
    public async Task<Result<string>> ValidatePassword([FromBody] string password)
    {
        int length = password.Length;
        if (length < 8)
        {
            return Result<string>.Failure(new Error("Password.LengthToShort", "Password id to short (min of 8 characters)", ErrorType.InvalidInput));
        }
        else if (length > 64)
        {
            return Result<string>.Failure(new Error("Password.LengthToLong", "Password id to short (max of 64 characters)", ErrorType.InvalidInput));
        }

        //TODO: additional checks
        return Result<string>.Success(password);
    }
    public async Task<bool> ValidateCredentials(string email, string password)
    {
        var userResult = await FindByEmail(email);
        if (!userResult.IsSuccess || userResult.Value is null) return false;

        var credential = await _credentialsRepository.GetUserCredentials(userResult.Value.Id);
        return credential is not null && BCrypt.Net.BCrypt.Verify(password, credential.PasswordHash);
    }
}