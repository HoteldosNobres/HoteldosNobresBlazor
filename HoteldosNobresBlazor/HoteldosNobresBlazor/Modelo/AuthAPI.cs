using HoteldosNobresBlazor.Client;
using HoteldosNobresBlazor.Modelo;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;

namespace HoteldosNobresBlazor.Services;

public class AuthAPI : AuthenticationStateProvider
{
    private static bool autenticado = true;
     
    private static Task<AuthenticationState> defaultUnauthenticatedTask =
            Task.FromResult(new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity())));

    private  readonly Task<AuthenticationState> authenticationStateTask = defaultUnauthenticatedTask;

    private int Idtask = 0;

    public AuthAPI(PersistentComponentState state)
    {
        Idtask = authenticationStateTask.Id;
         
        if (state == null) 
            return;

        if (!state.TryTakeFromJson<UserInfo>(nameof(UserInfo), out var userInfo) || userInfo is null)
        {
            return;
        }

        Claim[] claims = [
            new Claim(ClaimTypes.NameIdentifier, userInfo.UserId),
                new Claim(ClaimTypes.Name, userInfo.Email),
                new Claim(ClaimTypes.Email, userInfo.Email) ];

        authenticationStateTask = Task.FromResult(
            new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity(claims,
                authenticationType: nameof(AuthAPI)))));


    }

    public override Task<AuthenticationState> GetAuthenticationStateAsync() => authenticationStateTask;

    public async Task<AuthResponse> LoginAsync(string email, string senha)
    {
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

        return new AuthResponse { Sucesso = autenticado };
    }

    public static async Task<AuthenticationState> GetAuthenticationAsync(string email)
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

        defaultUnauthenticatedTask = Task.FromResult(
            new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity(claims,
                authenticationType: nameof(AuthAPI)))));
          
        return defaultUnauthenticatedTask.Result;
    }

    public static async Task<AuthenticationState> GetAuthenticationClienteAsync(string email)
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

        defaultUnauthenticatedTask = Task.FromResult(
            new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity(claims,
                authenticationType: nameof(AuthAPI)))));

        //return defaultUnauthenticatedTask.Result;
        return new AuthenticationState(pessoa);
    }

    public static async Task<AuthenticationState> GetAuthenticationLogoutAsync()
    {
        var pessoa = new ClaimsPrincipal();


        //Claim[] claims = new();

        //var identity = new ClaimsIdentity(claims, "Cookies");
        //pessoa = new ClaimsPrincipal(identity);

        defaultUnauthenticatedTask = Task.FromResult(
            new AuthenticationState(pessoa));

        return new AuthenticationState(pessoa);
    }

    public async Task LogoutAsync()
    {
        //await _httpClient.PostAsync("auth/logout", null);
        NotifyAuthenticationStateChanged(GetAuthenticationLogoutAsync());
    }

    public async Task<bool> VerificaAutenticado()
    {
        await GetAuthenticationStateAsync();
        return autenticado;
    }

    public void Autentificador(bool aut = false)
    {
        autenticado = aut;
    }
}

 