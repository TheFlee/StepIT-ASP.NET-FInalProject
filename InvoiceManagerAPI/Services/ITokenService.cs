using InvoiceManagerAPI.Models;

namespace InvoiceManagerAPI.Services;

public interface ITokenService
{
    Task<string> CreateAsync(User user);
    Task<User?> ValidateAsync(string token);
    Task RevokeAllAsync(string userId);
}
