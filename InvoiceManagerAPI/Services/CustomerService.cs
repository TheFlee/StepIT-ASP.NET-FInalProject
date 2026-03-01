using AutoMapper;
using InvoiceManagerAPI.Common;
using InvoiceManagerAPI.Data;
using InvoiceManagerAPI.DTOs;
using InvoiceManagerAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace InvoiceManagerAPI.Services;

public class CustomerService : ICustomerService
{
    private readonly InvoiceManagerDbContext _context;
    private readonly IMapper _mapper;

    public CustomerService(InvoiceManagerDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<bool> ArchiveAsync(Guid id)
    {
        var customer = await _context.Customers.FindAsync(id);
        if (customer is null)
        {
            return false; 
        }
        customer.DeletedAt = DateTimeOffset.UtcNow;
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<CustomerResponseDTO> CreateAsync(CreateCustomerRequestDTO customer, string currentUserId)
    {
        var newCustomer = _mapper.Map<Customer>(customer);
        newCustomer.UserId = currentUserId;
        _context.Customers.Add(newCustomer);
        await _context.SaveChangesAsync();

        await _context.Entry(newCustomer).Collection(c => c.Invoices).LoadAsync();
        return _mapper.Map<CustomerResponseDTO>(newCustomer);
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var customer = await _context.Customers.Include(c => c.Invoices).FirstOrDefaultAsync(c => c.Id == id);
        if (customer is null)
        {
            return false;
        }
        if (customer.Invoices.Any(i => i.Status != InvoiceStatus.Created))
        {
            return false;
        }
        _context.Customers.Remove(customer);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<IEnumerable<CustomerResponseDTO>> GetAllAsync(string currentUserId)
    {
        var customers = await _context.Customers
            .Where(c => c.DeletedAt == null && c.UserId == currentUserId)
            .Include(c => c.Invoices)
            .ToListAsync();

        return _mapper.Map<IEnumerable<CustomerResponseDTO>>(customers);
    }

    public async Task<CustomerResponseDTO?> GetByIdAsync(Guid id, string currentUserId)
    {
        var customer = await _context.Customers
            .Where(c => c.DeletedAt == null && c.UserId == currentUserId)
            .Include(c => c.Invoices)
            .FirstOrDefaultAsync(c => c.Id == id);

        if (customer is null)
        {
            return null;
        }
        return _mapper.Map<CustomerResponseDTO>(customer);
    }

    public async Task<CustomerResponseDTO?> UpdateAsync(Guid id, UpdateCustomerRequestDTO customer)
    {
        var updatedCustomer = await _context.Customers.Include(c => c.Invoices).FirstOrDefaultAsync(c => c.Id == id);
        if (updatedCustomer is null)
        {
            return null;
        }
        if (updatedCustomer.DeletedAt != null)
        {
            return null;
        }
        _mapper.Map(customer, updatedCustomer);
        await _context.SaveChangesAsync();
        return _mapper.Map<CustomerResponseDTO>(updatedCustomer);
    }
    public async Task<PagedResult<CustomerResponseDTO>> GetPagedAsync(CustomerQueryParams queryParams)
    {
        var query = _context.Customers.Where(c => c.DeletedAt == null).AsQueryable();

        if (!string.IsNullOrEmpty(queryParams.Search))
        {
            var searchTerm = queryParams.Search.ToLower();
            query = query.Where(c => c.Name.ToLower().Contains(searchTerm) ||
                                c.Address != null && c.Address.ToLower().Contains(searchTerm) ||
                                c.Email.ToLower().Contains(searchTerm));
        }

        query = ApplySorting(query, queryParams.SortBy, queryParams.SortDirection);

        var totalCount = await query.CountAsync();
        var skip = (queryParams.Page - 1) * queryParams.PageSize;
        var customers = await query.Skip(skip)
                               .Take(queryParams.PageSize)
                               .ToListAsync();
        var customerDTOs = _mapper.Map<IEnumerable<CustomerResponseDTO>>(customers);
        return PagedResult<CustomerResponseDTO>.Create(customerDTOs, queryParams.Page, queryParams.PageSize, totalCount);
    }

    private IQueryable<Customer> ApplySorting(IQueryable<Customer> query, string? sortBy, string? sortDirection)
    {
        if (string.IsNullOrEmpty(sortBy))
        {
            return query.OrderByDescending(c => c.CreatedAt);
        }
        var isDescending = sortDirection?.ToLower() == "desc";
        return sortBy.ToLower() switch
        {
            "name" => isDescending 
                    ? query.OrderByDescending(c => c.Name) 
                    : query.OrderBy(c => c.Name),
            "email" => isDescending 
                    ? query.OrderByDescending(c => c.Email) 
                    : query.OrderBy(c => c.Email),
            "address" => isDescending 
                    ? query.OrderByDescending(c => c.Address) 
                    : query.OrderBy(c => c.Address),
            "createdat" => isDescending 
                    ? query.OrderByDescending(c => c.CreatedAt) 
                    : query.OrderBy(c => c.CreatedAt),
            _ => query.OrderByDescending(c => c.CreatedAt)
        };
    }
}
