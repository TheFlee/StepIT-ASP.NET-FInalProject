using InvoiceManagerAPI.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using System.Text.Encodings.Web;

namespace InvoiceManagerAPI.Authentication;

public class OpaqueAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
{
    private readonly ITokenService _tokenService;

    public OpaqueAuthenticationHandler(
        IOptionsMonitor<AuthenticationSchemeOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder,
        ITokenService tokenService)
        : base(options, logger, encoder)
    {
        _tokenService = tokenService;
    }

    protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        if (!Request.Headers.ContainsKey("Authorization"))
            return AuthenticateResult.NoResult();

        var token = Request.Headers["Authorization"]
            .FirstOrDefault()?
            .Replace("Bearer ", "");

        if (string.IsNullOrWhiteSpace(token))
            return AuthenticateResult.Fail("Token missing");

        var user = await _tokenService.ValidateAsync(token);
        if (user is null)
            return AuthenticateResult.Fail("Invalid token");

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id),
            new Claim(ClaimTypes.Email, user.Email!)
        };

        var identity = new ClaimsIdentity(claims, Scheme.Name);
        var principal = new ClaimsPrincipal(identity);
        var ticket = new AuthenticationTicket(principal, Scheme.Name);

        return AuthenticateResult.Success(ticket);
    }

}
