using InvoiceManagerAPI.Data;
using InvoiceManagerAPI.Models;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;

namespace InvoiceManagerAPI.Services;

public class TokenService : ITokenService
{
    private readonly InvoiceManagerDbContext _context;

    public TokenService(InvoiceManagerDbContext context)
    {
        _context = context;
    }

    public async Task<string> CreateAsync(User user)
    {
        var rawToken = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));

        var hash = SHA256.HashData(Encoding.UTF8.GetBytes(rawToken));
        var tokenHash = Convert.ToBase64String(hash);

        _context.TokenSessions.Add(new TokenSession
        {
            Id = Guid.NewGuid(),
            UserId = user.Id,
            TokenHash = tokenHash,
            CreatedAt = DateTimeOffset.UtcNow,
            ExpiresAt = DateTimeOffset.UtcNow.AddHours(8)
        });

        await _context.SaveChangesAsync();

        return rawToken;
    }

    public async Task<User?> ValidateAsync(string token)
    {
        var hash = SHA256.HashData(Encoding.UTF8.GetBytes(token));
        var tokenHash = Convert.ToBase64String(hash);

        var session = await _context.TokenSessions
            .Include(x => x.User)
            .FirstOrDefaultAsync(x =>
                x.TokenHash == tokenHash &&
                !x.IsRevoked && 
                x.ExpiresAt > DateTimeOffset.UtcNow);

        return session?.User;
    }
    public async Task RevokeAllAsync(string userId)
{
    var sessions = await _context.TokenSessions
            .Where(x => x.UserId == userId && !x.IsRevoked)
            .ToListAsync();

    foreach (var session in sessions) 
            session.IsRevoked = true;

    await _context.SaveChangesAsync();
}
}
