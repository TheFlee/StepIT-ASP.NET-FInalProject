using AutoMapper;
using AutoMapper.Configuration.Annotations;
using InvoiceManagerAPI.Common;
using InvoiceManagerAPI.Data;
using InvoiceManagerAPI.DTOs;
using InvoiceManagerAPI.Models;
using Microsoft.EntityFrameworkCore;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace InvoiceManagerAPI.Services;

public class InvoiceService : IInvoiceService
{
    private readonly InvoiceManagerDbContext _context;
    private readonly IMapper _mapper;

    public InvoiceService(InvoiceManagerDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<bool> ArchiveAsync(Guid id)
    {
        var invoice = await _context.Invoices.FindAsync(id);
        if (invoice == null)
        {
            return false;
        }
        invoice.DeletedAt = DateTimeOffset.UtcNow;
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task ChangeStatusAsync(Guid id, string newStatus)
    {
        var invoice = await _context.Invoices.FindAsync(id);
        if (invoice == null)
        {
            throw new ArgumentException($"Invoice with ID {id} does not exist");
        }
        if (Enum.TryParse<InvoiceStatus>(newStatus, out var status))
        {
            invoice.Status = status;
            await _context.SaveChangesAsync();
        }
        else
        {
            throw new ArgumentException($"Invalid status value: {newStatus}");
        }
    }


    public async Task<InvoiceResponseDTO> CreateAsync(CreateInvoiceRequestDTO invoice)
    {
        var isCostumerExists = await _context.Customers.AnyAsync(c => c.Id == invoice.CustomerId);
        if (!isCostumerExists)
        {
            throw new ArgumentException($"Customer with ID {invoice.CustomerId} does not exist");
        }
        var newInvoice = _mapper.Map<Invoice>(invoice);

        _context.Invoices.Add(newInvoice);
        await _context.SaveChangesAsync();

        await _context.Entry(newInvoice).Reference(i => i.Customer).LoadAsync();
        return _mapper.Map<InvoiceResponseDTO>(newInvoice);
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var invoice = await _context.Invoices.FindAsync(id);
        if (invoice == null || invoice.Status != InvoiceStatus.Created)
        {
            return false;
        }
        _context.Invoices.Remove(invoice);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<IEnumerable<InvoiceResponseDTO>> GetAllAsync()
    {
        var invoices = await _context.Invoices.Where(i => i.DeletedAt == null)
                                              .Include(i => i.Customer)
                                              .Include(i => i.Rows)
                                              .ToListAsync();

        return _mapper.Map<IEnumerable<InvoiceResponseDTO>>(invoices);
    }

    public async Task<InvoiceResponseDTO?> GetByIdAsync(Guid id)
    {
        var invoice = await _context.Invoices.Where(i => i.DeletedAt == null)
                                             .Include(i => i.Customer)
                                             .Include(i => i.Rows)
                                             .FirstOrDefaultAsync(i => i.Id == id);
        if (invoice == null)
        {
            return null;
        }
        return _mapper.Map<InvoiceResponseDTO>(invoice);
    }

    public async Task<InvoiceResponseDTO?> UpdateAsync(Guid id, UpdateInvoiceRequestDTO invoice)
    {
        var updatedInvoice = await _context.Invoices.Include(i => i.Rows).FirstOrDefaultAsync(i => i.Id == id);
        if (updatedInvoice == null)
        {
            return null;
        }

        _mapper.Map(invoice, updatedInvoice);

        await _context.SaveChangesAsync();
        await _context.Entry(updatedInvoice).Reference(i => i.Customer).LoadAsync();
        return _mapper.Map<InvoiceResponseDTO>(updatedInvoice);
    }

    public async Task<PagedResult<InvoiceResponseDTO>> GetPagedAsync(InvoiceQueryParams queryParams)
    {
        var query = _context.Invoices.Where(i => i.DeletedAt == null).Include(i => i.Customer).Include(i => i.Rows).AsQueryable();

        if (!string.IsNullOrEmpty(queryParams.CustomerName))
        {
            var searchTerm = queryParams.CustomerName.ToLower();
            query = query.Where(i => i.Customer!.Name.ToLower().Contains(searchTerm));
        }

        if (!string.IsNullOrEmpty(queryParams.Status))
        {
            if (Enum.TryParse<InvoiceStatus>(queryParams.Status, true, out var status))
            {
                query = query.Where(i => i.Status == status);
            }
        }

        if (queryParams.MinTotal.HasValue)
        {
            query = query.Where(i => i.TotalSum >= queryParams.MinTotal.Value);
        }

        if (queryParams.MaxTotal.HasValue)
        {
            query = query.Where(i => i.TotalSum <= queryParams.MaxTotal.Value);
        }

        if (queryParams.StartDateFrom.HasValue)
        {
            query = query.Where(i => i.StartDate >= queryParams.StartDateFrom.Value);
        }

        if (queryParams.StartDateTo.HasValue)
        {
            query = query.Where(i => i.StartDate <= queryParams.StartDateTo.Value);
        }

        if (!string.IsNullOrEmpty(queryParams.Search))
        {
            var searchTerm = queryParams.Search.ToLower();
            query = query.Where(i => i.Customer != null && i.Customer.Name.ToLower().Contains(searchTerm) ||
                                     i.Comment != null && i.Comment.ToLower().Contains(searchTerm));
        }

        query = ApplySorting(query, queryParams.SortBy, queryParams.SortDirection);

        var totalCount = await query.CountAsync();
        var skip = (queryParams.Page - 1) * queryParams.PageSize;
        var invoices = await query.Skip(skip)
                               .Take(queryParams.PageSize)
                               .ToListAsync();
        var invoiceDTOs = _mapper.Map<IEnumerable<InvoiceResponseDTO>>(invoices);
        return PagedResult<InvoiceResponseDTO>.Create(invoiceDTOs, queryParams.Page, queryParams.PageSize, totalCount);
    }

    private IQueryable<Invoice> ApplySorting(IQueryable<Invoice> query, string? sortBy, string? sortDirection)
    {
        if (string.IsNullOrEmpty(sortBy))
        {
            return query.OrderByDescending(i => i.CreatedAt);
        }
        var isDescending = sortDirection?.ToLower() == "desc";

        return sortBy.ToLower() switch
        {
            "customername" => isDescending 
                            ? query.OrderByDescending(i => i.Customer!.Name) 
                            : query.OrderBy(i => i.Customer!.Name),
            "startdate" => isDescending 
                            ? query.OrderByDescending(i => i.StartDate) 
                            : query.OrderBy(i => i.StartDate),
            "total" => isDescending 
                            ? query.OrderByDescending(i => i.TotalSum) 
                            : query.OrderBy(i => i.TotalSum),
            "createdat" => isDescending
                            ? query.OrderByDescending(c => c.CreatedAt)
                            : query.OrderBy(c => c.CreatedAt),
            _ => query.OrderByDescending(i => i.CreatedAt)
        };
    }

    public async Task<byte[]> GeneratePdfAsync(Guid id)
    {
        var invoice = await _context.Invoices
                                    .Where(i => i.DeletedAt == null)
                                    .Include(i => i.Customer)
                                    .Include(i => i.Rows)
                                    .FirstOrDefaultAsync(i => i.Id == id);

        if (invoice == null)
            throw new ArgumentException($"Invoice with ID {id} does not exist");

        var pdfBytes = QuestPDF.Fluent.Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(2, Unit.Centimetre);
                page.DefaultTextStyle(x => x.FontSize(12));

                page.Header()
                    .Text($"Invoice #{invoice.Id}")
                    .FontSize(18)
                    .Bold()
                    .AlignCenter();

                page.Content()
                    .Column(column =>
                    {
                        column.Item().Text("");
                        column.Item().Text($"Customer: {invoice.Customer?.Name}");
                        column.Item().Text($"Email: {invoice.Customer?.Email}");
                        column.Item().Text($"Address: {invoice.Customer?.Address}");
                        column.Item().Text($"Phone: {invoice.Customer?.PhoneNumber}");
                        column.Item().Text($"Invoice Date: {invoice.StartDate:yyyy-MM-dd}");
                        column.Item().Text("");

                        column.Item().LineHorizontal(1);

                        column.Item().Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                columns.RelativeColumn();
                                columns.RelativeColumn();
                                columns.RelativeColumn();
                                columns.RelativeColumn();
                            });

                            table.Header(header =>
                            {
                                header.Cell().Text("Service").Bold();
                                header.Cell().Text("Quantity").Bold();
                                header.Cell().Text("Amount").Bold();
                                header.Cell().Text("Sum").Bold();
                            });

                            foreach (var row in invoice.Rows)
                            {
                                table.Cell().Text(row.Service);
                                table.Cell().Text(row.Quantity.ToString());
                                table.Cell().Text(row.Amount.ToString("0.00"));
                                table.Cell().Text(row.Sum.ToString("0.00"));
                            }
                        });

                        column.Item().LineHorizontal(1);
                        column.Item().Text("");
                        column.Item().Text($"Total: {invoice.TotalSum:0.00}").Bold();
                    });

                page.Footer()
                    .AlignCenter()
                    .Text("Thank you for your business!");
            });
        }).GeneratePdf();

        return await Task.FromResult(pdfBytes);
    }
}
