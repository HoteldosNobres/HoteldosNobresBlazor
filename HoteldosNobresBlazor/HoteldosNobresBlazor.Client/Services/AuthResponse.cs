using System.Security.Claims;

namespace HoteldosNobresBlazor.Services;

public class AuthResponse
{
    public bool Sucesso { get; set; }

    public string Role { get; set; }

    public string[] Erros { get; set; }

    public ClaimsPrincipal User { get; set; }
}