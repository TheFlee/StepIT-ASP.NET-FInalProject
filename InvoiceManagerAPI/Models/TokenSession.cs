namespace InvoiceManagerAPI.Models;

public class TokenSession
{
    public Guid Id { get; set; }

    public string UserId { get; set; }
    public User User { get; set; } = null!;

    public string TokenHash { get; set; } = null!;

    public DateTimeOffset ExpiresAt { get; set; }
    public DateTimeOffset CreatedAt { get; set; }

    public bool IsRevoked { get; set; }
}
