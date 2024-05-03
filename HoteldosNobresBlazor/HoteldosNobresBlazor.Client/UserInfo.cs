using System.Security.Claims;

namespace HoteldosNobresBlazor.Client;

// Add properties to this class and update the server and client AuthenticationStateProviders
// to expose more information about the authenticated user to the client.
public class UserInfo
{
    public  string UserId { get; set; }
    public  string Email { get; set; }

    public UserInfo (string userId, string email)
    {
        UserId = userId;
        Email = email;
    }

    public UserInfo()
    {
    }

    public UserInfo(ClaimsPrincipal claimsPrincipal)
    {
        if (claimsPrincipal.Identity!.IsAuthenticated){
            Email = claimsPrincipal.Identity.Name!;
            UserId = claimsPrincipal.FindFirst(ClaimTypes.NameIdentifier)?.Value!; 
        } 
    }
}
