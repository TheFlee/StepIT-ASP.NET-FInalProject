using InvoiceManagerAPI.Common;
using InvoiceManagerAPI.DTOs;

namespace InvoiceManagerAPI.Services;

public interface IInvoiceService
{
    Task<IEnumerable<InvoiceResponseDTO>> GetAllAsync(string currentUserId);
    Task<InvoiceResponseDTO?> GetByIdAsync(Guid id);
    Task<PagedResult<InvoiceResponseDTO>> GetPagedAsync(InvoiceQueryParams queryParams);
    Task<InvoiceResponseDTO> CreateAsync(CreateInvoiceRequestDTO invoice, string currentUserId);
    Task<InvoiceResponseDTO?> UpdateAsync(Guid id, UpdateInvoiceRequestDTO invoice);
    Task<bool> DeleteAsync(Guid id);
    Task<bool> ArchiveAsync(Guid id);
    Task ChangeStatusAsync(Guid id, string newStatus);
    Task<byte[]> GeneratePdfAsync(Guid id);
}
