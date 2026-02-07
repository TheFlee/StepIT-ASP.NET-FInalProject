namespace InvoiceManagerAPI.DTOs;

/// <summary>
/// DTO for creating a new customer.
/// </summary>
public class CreateCustomerRequestDTO
{
    /// <summary>
    /// Customer's full name.
    /// </summary>
    /// <example>John Doe</example>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Customer's email address.
    /// </summary>
    /// <example>john.doe@example.com</example>
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// Customer's physical address (optional).
    /// </summary>
    /// <example>123 Main St, Baku, Azerbaijan</example>
    public string? Address { get; set; } = string.Empty;

    /// <summary>
    /// Customer's phone number (optional).
    /// </summary>
    /// <example>+994501234567</example>
    public string? PhoneNumber { get; set; } = string.Empty;
}

/// <summary>
/// DTO for updating an existing customer.
/// </summary>
public class UpdateCustomerRequestDTO
{
    /// <summary>
    /// Customer's full name.
    /// </summary>
    /// <example>Jane Doe</example>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Customer's email address.
    /// </summary>
    /// <example>jane.doe@example.com</example>
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// Customer's physical address (optional).
    /// </summary>
    /// <example>456 Elm St, Baku, Azerbaijan</example>
    public string? Address { get; set; } = string.Empty;

    /// <summary>
    /// Customer's phone number (optional).
    /// </summary>
    /// <example>+994551234567</example>
    public string? PhoneNumber { get; set; } = string.Empty;
}

public class CustomerResponseDTO
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? Address { get; set; } = string.Empty;
    public string? PhoneNumber { get; set; } = string.Empty;
    public int InvoicesCount { get; set; }
}

public class CustomerQueryParams
{
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;

    public string? SortBy { get; set; }
    public string? SortDirection { get; set; }

    public string? Search { get; set; }
}
