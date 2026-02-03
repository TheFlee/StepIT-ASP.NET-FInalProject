namespace InvoiceManagerAPI.Models;

public class InvoiceRow
{
    public Guid Id { get; set; }
    public Guid InvoiceId { get; set; }
    public string Service { get; set; } = string.Empty;
    public decimal Quantity { get; set; }
    public decimal Amount { get; set; }
    public decimal Sum { get; set; }
}
