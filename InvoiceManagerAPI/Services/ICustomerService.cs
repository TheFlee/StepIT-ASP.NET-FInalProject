using InvoiceManagerAPI.Common;
using InvoiceManagerAPI.DTOs;

namespace InvoiceManagerAPI.Services;

public interface ICustomerService
{
    Task<IEnumerable<CustomerResponseDTO>> GetAllAsync(string currentUserId);
    Task<CustomerResponseDTO?> GetByIdAsync(Guid id, string currentUserId);
    Task<PagedResult<CustomerResponseDTO>> GetPagedAsync(CustomerQueryParams queryParams);
    Task<CustomerResponseDTO> CreateAsync(CreateCustomerRequestDTO customer, string currentUserId);
    Task<CustomerResponseDTO?> UpdateAsync(Guid id, UpdateCustomerRequestDTO customer);
    Task<bool> DeleteAsync(Guid id);
    Task<bool> ArchiveAsync(Guid id);
}
