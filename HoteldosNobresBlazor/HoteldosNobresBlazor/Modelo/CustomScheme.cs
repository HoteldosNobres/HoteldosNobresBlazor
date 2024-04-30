using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using System.Security.Principal;
using System.Text.Encodings.Web;

namespace HoteldosNobresBlazor.Modelo;

public class CustomAuthenticationHandler(IOptionsMonitor<CustomAuthOptions> options, ILoggerFactory logger, UrlEncoder encoder, IHttpContextAccessor httpContextAccessor)
    : AuthenticationHandler<CustomAuthOptions>(options, logger, encoder)
{

    protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        return AuthenticateResult.Success(new AuthenticationTicket(new ClaimsPrincipal(new GenericIdentity("CustomIdentity")), "CustomAuthenticationScheme"));
    }
}

public class CustomAuthOptions : AuthenticationSchemeOptions
{
    // Add any custom options here if needed in the future
}