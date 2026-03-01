using InvoiceManagerAPI.DTOs;
using InvoiceManagerAPI.Models;
using Microsoft.AspNetCore.Identity;

namespace InvoiceManagerAPI.Services;

public class AuthService : IAuthService
{
    private readonly UserManager<User> _userManager;
    private readonly ITokenService _tokenService;

    public AuthService(UserManager<User> userManager, ITokenService tokenService)
    {
        _userManager = userManager;
        _tokenService = tokenService;
    }

    public async Task<AuthResponseDTO> LoginAsync(LoginRequestDTO request)
    {
        var user = await _userManager.FindByEmailAsync(request.Email);

        if (user == null || !await _userManager.CheckPasswordAsync(user, request.Password))
            throw new InvalidOperationException("Invalid email or password.");

        await _tokenService.RevokeAllAsync(user.Id);

        user.UpdatedAt = DateTimeOffset.UtcNow;
        await _userManager.UpdateAsync(user);

        var token = await _tokenService.CreateAsync(user);

        return new AuthResponseDTO
        {
            Email = user.Email!,
            AccessToken = token
        };
    }

    public async Task<AuthResponseDTO> RegisterAsync(RegisterRequestDTO request)
    {
        var existingUser = await _userManager.FindByEmailAsync(request.Email);

        if (existingUser != null)
        {
            throw new InvalidOperationException("User with this email already exists.");
        }

        var newUser = new User
        {
            Name = request.Name,
            Email = request.Email,
            UserName = request.Email,
            CreatedAt = DateTimeOffset.UtcNow
        };

        var result = await _userManager.CreateAsync(newUser, request.Password);
        if (!result.Succeeded)
        {
            var errors = string.Join("; ", result.Errors.Select(e => e.Description));
            throw new InvalidOperationException($"User creation failed: {errors}");
        }

        return new AuthResponseDTO
        {
            Email = newUser.Email
        };
    }
}
