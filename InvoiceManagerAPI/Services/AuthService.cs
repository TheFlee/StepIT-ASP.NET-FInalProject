using AutoMapper;
using InvoiceManagerAPI.DTOs;
using InvoiceManagerAPI.Models;
using Microsoft.AspNetCore.Identity;

namespace InvoiceManagerAPI.Services;

public class AuthService : IAuthService
{
    private readonly UserManager<User> _userManager;
    private readonly ITokenService _tokenService;
    private readonly IMapper _mapper;

    public AuthService(UserManager<User> userManager, ITokenService tokenService, IMapper mapper)
    {
        _userManager = userManager;
        _tokenService = tokenService;
        _mapper = mapper;
    }

    public async Task ChangePasswordAsync(string userId, ChangePasswordRequestDTO request)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
            throw new InvalidOperationException("User not found");

        var result = await _userManager.ChangePasswordAsync(user, request.CurrentPassword, request.NewPassword);

        if (!result.Succeeded)
        {
            var errors = string.Join("; ", result.Errors.Select(e => e.Description));
            throw new InvalidOperationException($"Password change failed: {errors}");
        }

        user.UpdatedAt = DateTimeOffset.UtcNow;
        await _userManager.UpdateAsync(user);
    }

    public async Task DeleteProfileAsync(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
            throw new InvalidOperationException("User not found");

        var result = await _userManager.DeleteAsync(user);

        if (!result.Succeeded)
        {
            var errors = string.Join("; ", result.Errors.Select(e => e.Description));
            throw new InvalidOperationException($"Profile deletion failed: {errors}");
        }

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

        var newUser = _mapper.Map<User>(request);

        var result = await _userManager.CreateAsync(newUser, request.Password);
        if (!result.Succeeded)
        {
            var errors = string.Join("; ", result.Errors.Select(e => e.Description));
            throw new InvalidOperationException($"User creation failed: {errors}");
        }

        return new AuthResponseDTO
        {
            Email = newUser.Email!
        };
    }

    public async Task UpdateProfileAsync(string userId, UpdateProfileRequestDTO request)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
            throw new InvalidOperationException("User not found");

        _mapper.Map(request, user);
        user.UpdatedAt = DateTimeOffset.UtcNow;

        var result = await _userManager.UpdateAsync(user);

        if (!result.Succeeded)
        {
            var errors = string.Join("; ", result.Errors.Select(e => e.Description));
            throw new InvalidOperationException($"Profile update failed: {errors}");
        }
    }
}
