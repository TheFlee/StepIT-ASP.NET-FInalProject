using Microsoft.AspNetCore.Identity;
using System.Collections;

namespace InvoiceManagerAPI.Models;

public class User : IdentityUser
{
    public string Name { get; set; } = string.Empty;
    public string? Address { get; set; } = string.Empty;
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset? UpdatedAt { get; set; }

    public ICollection<Customer> Customers { get; set; } = new List<Customer>();

}
