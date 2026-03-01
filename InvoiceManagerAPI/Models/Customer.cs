namespace InvoiceManagerAPI.Models;

public class Customer
{
    public Guid Id { get; set; }
    public string UserId { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Address { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? PhoneNumber { get; set; } = string.Empty;
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset? UpdatedAt { get; set; }
    public DateTimeOffset? DeletedAt { get; set; }

    public User? User { get; set; }
    public ICollection<Invoice> Invoices { get; set; } = new List<Invoice>();

}
