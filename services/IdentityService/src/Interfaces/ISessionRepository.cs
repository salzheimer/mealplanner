using IdentityService.Models;

namespace IdentityService.Interfaces;

public interface ISessionRepository
{
    Task<Session> CreateAsync(Session session);
    Task<Session?> GetByTokenHashAsync(string tokenHash);
    Task<bool> RevokeAsync(long id);
}
