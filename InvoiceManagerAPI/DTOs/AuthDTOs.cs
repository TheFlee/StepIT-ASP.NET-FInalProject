namespace InvoiceManagerAPI.DTOs;

public class RegisterRequestDTO
{
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}

public class LoginRequestDTO
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}

public class AuthResponseDTO
{
    public string Email { get; set; } = string.Empty;
    public string AccessToken { get; set; } = string.Empty;
}