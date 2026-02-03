using AutoMapper;
using InvoiceManagerAPI.Data;
using InvoiceManagerAPI.DTOs;
using InvoiceManagerAPI.Models;
using InvoiceManagerAPI.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

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
}
