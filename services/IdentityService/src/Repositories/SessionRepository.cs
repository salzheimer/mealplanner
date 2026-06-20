using IdentityService.Interfaces;
using IdentityService.Models;
using Microsoft.EntityFrameworkCore;

namespace IdentityService.Repositories;

public class SessionRepository : ISessionRepository
{
    private readonly UserContext _context;

    public SessionRepository(UserContext context)
    {
        _context = context;
    }

    public async Task<Session> CreateAsync(Session session)
    {
        _context.Sessions.Add(session);
        await _context.SaveChangesAsync();
        return session;
    }

    public async Task<Session?> GetByTokenHashAsync(string tokenHash)
    {
        return await _context.Sessions
            .FirstOrDefaultAsync(s => s.TokenHash == tokenHash);
    }

    public async Task<bool> RevokeAsync(Guid id)
    {
        var session = await _context.Sessions.FindAsync(id);
        if (session == null) return false;
        session.RevokedAt = DateTime.UtcNow;
        session.UpdatedAt = DateTime.UtcNow;
        return await _context.SaveChangesAsync() > 0;
    }
}
