using Google.Apis.PeopleService.v1.Data;
using HoteldosNobresBlazor.Client;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using System.Security.Claims;

namespace HoteldosNobresBlazor.Services;

public class AuthAPI : AuthenticationStateProvider
{
    private static Task<AuthenticationState>  defaultUnauthenticatedTask =
            Task.FromResult(new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity())));

    private Task<AuthenticationState> authenticationStateTask = defaultUnauthenticatedTask;

    public AuthAPI() 
    {
       
    }

    public override Task<AuthenticationState> GetAuthenticationStateAsync() => authenticationStateTask;

    public async Task<AuthResponse> LoginAsync(string email, string senha)
    {
        bool autenticado = true;
        if (email == "fabiohcnobre@hotmail.com" && senha == "123")
        { 
            NotifyAuthenticationStateChanged(GetAuthenticationAsync(email));
        }
        else if ((email == "hoteldosnobres@hotmail.com" || senha == "1234567")
            && senha == "123")
        {
            NotifyAuthenticationStateChanged(GetAuthenticationClienteAsync(email));
        }
        else
        {
            autenticado = false;
        }

        return new AuthResponse { Sucesso = autenticado, User = defaultUnauthenticatedTask.Result.User };
    }

    public async Task<AuthenticationState> GetAuthenticationAsync(string email)
    {
        var pessoa = new ClaimsPrincipal();
        //var response = await _httpClient.GetAsync("auth/manage/info"); 
        UserInfo userInfo = new();
        userInfo.Email = email;
        userInfo.UserId = "1";

        Claim[] claims = [
        new Claim(ClaimTypes.NameIdentifier, userInfo.UserId),
            new Claim(ClaimTypes.Name, userInfo.Email),
            new Claim(ClaimTypes.Email, userInfo.Email),
            new Claim(ClaimTypes.Role, "admin"),
            new Claim(ClaimTypes.Role, "client") ];

        var identity = new ClaimsIdentity(claims, "Cookies");
        pessoa = new ClaimsPrincipal(identity);

        defaultUnauthenticatedTask = Task.FromResult(new AuthenticationState(pessoa));

        return defaultUnauthenticatedTask.Result;
    }

    public async Task<AuthenticationState> GetAuthenticationClienteAsync(string email)
    {
        var pessoa = new ClaimsPrincipal(); 
        //var response = await _httpClient.GetAsync("auth/manage/info");

        UserInfo userInfo = new();
        userInfo.Email = email;
        userInfo.UserId = "1";

        Claim[] claims = [
        new Claim(ClaimTypes.NameIdentifier, userInfo.UserId),
            new Claim(ClaimTypes.Name, userInfo.Email),
            new Claim(ClaimTypes.Email, userInfo.Email),
            new Claim(ClaimTypes.Role, "client") ];

        var identity = new ClaimsIdentity(claims, "Cookies");
        pessoa = new ClaimsPrincipal(identity);

        defaultUnauthenticatedTask = Task.FromResult(new AuthenticationState(pessoa));

        return defaultUnauthenticatedTask.Result;
    }

    public void GetAuthenticationLogout()
    { 
        defaultUnauthenticatedTask = Task.FromResult(new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity())));
        authenticationStateTask = defaultUnauthenticatedTask;
        NotifyAuthenticationStateChanged(defaultUnauthenticatedTask);

    }
     
}