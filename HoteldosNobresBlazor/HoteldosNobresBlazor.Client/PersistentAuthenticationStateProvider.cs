using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;

namespace HoteldosNobresBlazor.Client;

// This is a client-side AuthenticationStateProvider that determines the user's authentication state by
// looking for data persisted in the page when it was rendered on the server. This authentication state will
// be fixed for the lifetime of the WebAssembly application. So, if the user needs to log in or out, a full
// page reload is required.
//
// This only provides a user name and email for display purposes. It does not actually include any tokens
// that authenticate to the server when making subsequent requests. That works separately using a
// cookie that will be included on HttpClient requests to the server.
internal class PersistentAuthenticationStateProvider : AuthenticationStateProvider
{
    private static readonly Task<AuthenticationState> defaultUnauthenticatedTask =
        Task.FromResult(new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity())));

    private readonly Task<AuthenticationState> authenticationStateTask = defaultUnauthenticatedTask;

    public PersistentAuthenticationStateProvider(PersistentComponentState stat)
    {
        AuthenticationState state = authenticationStateTask.Result;
        if (!state.User!.Identity!.IsAuthenticated)
        {
            return;
        }

        //Claim[] claims = [
        //   new Claim(ClaimTypes.NameIdentifier, state.User!.Identities!.FirstOrDefault().Name!),
        //    new Claim(ClaimTypes.Name,  state.User!.Identities.FirstOrDefault().Name!),
        //    new Claim(ClaimTypes.Email,  state.User!.Identities.FirstOrDefault().Name!) ];


        //authenticationStateTask = Task.FromResult(
        //    new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity(claims,
        //        authenticationType: nameof(PersistentAuthenticationStateProvider)))));
    }

    public override Task<AuthenticationState> GetAuthenticationStateAsync() => authenticationStateTask;
}
