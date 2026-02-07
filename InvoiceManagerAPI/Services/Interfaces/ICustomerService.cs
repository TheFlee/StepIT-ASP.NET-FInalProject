using InvoiceManagerAPI.Common;
using InvoiceManagerAPI.DTOs;

namespace InvoiceManagerAPI.Services.Interfaces;

public interface ICustomerService
{
    Task<IEnumerable<CustomerResponseDTO>> GetAllAsync();
    Task<CustomerResponseDTO?> GetByIdAsync(Guid id);
    Task<PagedResult<CustomerResponseDTO>> GetPagedAsync(CustomerQueryParams queryParams);
    Task<CustomerResponseDTO> CreateAsync(CreateCustomerRequestDTO customer);
    Task<CustomerResponseDTO?> UpdateAsync(Guid id, UpdateCustomerRequestDTO customer);
    Task<bool> DeleteAsync(Guid id);
    Task<bool> ArchiveAsync(Guid id);
}
