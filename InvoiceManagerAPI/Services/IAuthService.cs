using InvoiceManagerAPI.DTOs;

namespace InvoiceManagerAPI.Services;

public interface IAuthService
{
    Task<AuthResponseDTO> RegisterAsync(RegisterRequestDTO request);
    Task<AuthResponseDTO> LoginAsync(LoginRequestDTO request);
    Task UpdateProfileAsync(string userId, UpdateProfileRequestDTO request);
    Task ChangePasswordAsync(string userId, ChangePasswordRequestDTO request);
    Task DeleteProfileAsync(string userId);
}
