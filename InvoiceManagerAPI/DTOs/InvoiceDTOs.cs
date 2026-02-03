using InvoiceManagerAPI.Models;

namespace InvoiceManagerAPI.DTOs;

/// <summary>
/// DTO for creating a new invoice.
/// </summary>
public class CreateInvoiceRequestDTO
{
    /// <summary>
    /// Id of the customer for whom the invoice is created.
    /// </summary>
    /// <example>b7cdadac-a6ea-434e-8123-08de63164d96</example>
    public Guid CustomerId { get; set; }

    /// <summary>
    /// Start date of the work/service period.
    /// </summary>
    /// <example>2026-02-03T09:00:00Z</example>
    public DateTimeOffset StartDate { get; set; }

    /// <summary>
    /// End date of the work/service period.
    /// </summary>
    /// <example>2026-02-10T17:00:00Z</example>
    public DateTimeOffset EndDate { get; set; }

    /// <summary>
    /// Status of the invoice.
    /// </summary>
    /// <example>Created</example>
    public InvoiceStatus Status { get; set; }

    /// <summary>
    /// Optional comment about the invoice.
    /// </summary>
    /// <example>Monthly consulting services</example>
    public string? Comment { get; set; } = string.Empty;

    /// <summary>
    /// Array of invoice rows detailing each service/item.
    /// </summary>
    public CreateInvoiceRowRequestDTO[]? Rows { get; set; }
}

/// <summary>
/// DTO for updating an existing invoice.
/// </summary>
public class UpdateInvoiceRequestDTO
{
    /// <summary>
    /// Start date of the work/service period.
    /// </summary>
    /// <example>2026-02-03T09:00:00Z</example>
    public DateTimeOffset StartDate { get; set; }

    /// <summary>
    /// End date of the work/service period.
    /// </summary>
    /// <example>2026-02-10T17:00:00Z</example>
    public DateTimeOffset EndDate { get; set; }

    /// <summary>
    /// Status of the invoice.
    /// </summary>
    /// <example>Created</example>
    public InvoiceStatus Status { get; set; }

    /// <summary>
    /// Optional comment about the invoice.
    /// </summary>
    /// <example>Updated work period</example>
    public string? Comment { get; set; } = string.Empty;

    /// <summary>
    /// Array of invoice rows detailing each service/item.
    /// </summary>
    public CreateInvoiceRowRequestDTO[]? Rows { get; set; }
}

/// <summary>
/// Represents a single row (line item) when creating an invoice.
/// </summary>
public class CreateInvoiceRowRequestDTO
{
    /// <summary>
    /// Name or description of the service/work being invoiced.
    /// </summary>
    /// <example>Website Development</example>
    public string Service { get; set; } = string.Empty;

    /// <summary>
    /// Amount of the service provided (e.g., hours, pieces, kg).
    /// </summary>
    /// <example>10</example>
    public decimal Quantity { get; set; }

    /// <summary>
    /// Price per unit of the service.
    /// </summary>
    /// <example>50.75</example>
    public decimal Amount { get; set; }
}

public class InvoiceResponseDTO
{
    public Guid Id { get; set; }
    public Guid CustomerId { get; set; }
    public DateTimeOffset StartDate { get; set; }
    public DateTimeOffset EndDate { get; set; }
    public InvoiceRow[]? Rows { get; set; }
    public decimal TotalSum { get; set; }
    public string? Comment { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string CustomerName { get; set; } = string.Empty;
}