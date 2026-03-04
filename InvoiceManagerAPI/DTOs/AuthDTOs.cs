namespace InvoiceManagerAPI.DTOs
{
    /// <summary>
    /// DTO for registering a new user.
    /// </summary>
    public class RegisterRequestDTO
    {
        /// <summary>
        /// User's full name.
        /// </summary>
        /// <example>John Doe</example>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// User's email address.
        /// </summary>
        /// <example>john.doe@example.com</example>
        public string Email { get; set; } = string.Empty;

        /// <summary>
        /// Password for the new account.
        /// </summary>
        /// <example>P@ssw0rd123</example>
        public string Password { get; set; } = string.Empty;

        /// <summary>
        /// User's physical address (optional).
        /// </summary>
        /// <example>123 Main St, Baku, Azerbaijan</example>
        public string? Address { get; set; } = string.Empty;

        /// <summary>
        /// User's phone number (optional).
        /// </summary>
        /// <example>+994501234567</example>
        public string? PhoneNumber { get; set; } = string.Empty;
    }

    /// <summary>
    /// DTO for logging in a user.
    /// </summary>
    public class LoginRequestDTO
    {
        /// <summary>
        /// User's email address.
        /// </summary>
        /// <example>john.doe@example.com</example>
        public string Email { get; set; } = string.Empty;

        /// <summary>
        /// User's account password.
        /// </summary>
        /// <example>P@ssw0rd123</example>
        public string Password { get; set; } = string.Empty;
    }

    /// <summary>
    /// DTO for updating user profile information.
    /// </summary>
    public class UpdateProfileRequestDTO
    {
        /// <summary>
        /// User's full name.
        /// </summary>
        /// <example>John Doe</example>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// User's physical address (optional).
        /// </summary>
        /// <example>123 Main St, Baku, Azerbaijan</example>
        public string? Address { get; set; } = string.Empty;

        /// <summary>
        /// User's phone number (optional).
        /// </summary>
        /// <example>+994501234567</example>
        public string? PhoneNumber { get; set; } = string.Empty;
    }

    /// <summary>
    /// DTO for changing a user's password.
    /// </summary>
    public class ChangePasswordRequestDTO
    {
        /// <summary>
        /// Current password of the user.
        /// </summary>
        /// <example>OldP@ss123</example>
        public string CurrentPassword { get; set; } = string.Empty;

        /// <summary>
        /// New password for the user.
        /// </summary>
        /// <example>NewP@ss456</example>
        public string NewPassword { get; set; } = string.Empty;
    }

    /// <summary>
    /// DTO representing authentication response.
    /// </summary>
    public class AuthResponseDTO
    {
        /// <summary>
        /// User's email address.
        /// </summary>
        /// <example>john.doe@example.com</example>
        public string Email { get; set; } = string.Empty;

        /// <summary>
        /// JWT access token for authenticated requests.
        /// </summary>
        public string AccessToken { get; set; } = string.Empty;
    }
}