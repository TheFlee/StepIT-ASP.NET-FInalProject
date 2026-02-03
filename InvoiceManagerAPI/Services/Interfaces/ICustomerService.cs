using InvoiceManagerAPI.DTOs;

namespace InvoiceManagerAPI.Services.Interfaces;

public interface ICustomerService
{
    Task<IEnumerable<CustomerResponseDTO>> GetAllAsync();
    Task<CustomerResponseDTO?> GetByIdAsync(Guid id);
    Task<CustomerResponseDTO> CreateAsync(CreateCustomerRequestDTO customer);
    Task<CustomerResponseDTO?> UpdateAsync(Guid id, UpdateCustomerRequestDTO customer);
    Task<bool> DeleteAsync(Guid id);
    Task<bool> ArchiveAsync(Guid id);
}
