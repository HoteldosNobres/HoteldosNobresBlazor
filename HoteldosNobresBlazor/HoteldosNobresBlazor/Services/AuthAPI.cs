﻿using HoteldosNobresBlazor.Client;
using HoteldosNobresBlazor.Modelo; 
using Microsoft.AspNetCore.Components.Authorization;  
using System.Security.Claims;

namespace HoteldosNobresBlazor.Services;

public class AuthAPI : AuthenticationStateProvider
{
    private static bool autenticado = true;
    //private readonly HttpClient _httpClient = factory.CreateClient("API");

    private static readonly Task<AuthenticationState> defaultUnauthenticatedTask =
       Task.FromResult(new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity())));

    private static Task<AuthenticationState> authenticationStateTask = defaultUnauthenticatedTask;

    public AuthAPI( )
    {
        AuthenticationState state = authenticationStateTask.Result;
        if (state.User!.Identity == null || !state.User!.Identity!.IsAuthenticated)
        {
            return;
        }


        //Claim[] claims = [
        //   new Claim(ClaimTypes.NameIdentifier, state.User!.Identities!.FirstOrDefault().Name!),
        //    new Claim(ClaimTypes.Name,  state.User!.Identities.FirstOrDefault().Name!),
        //    new Claim(ClaimTypes.Email,  state.User!.Identities.FirstOrDefault().Name!) ];


        //authenticationStateTask = Task.FromResult(
        //    new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity(claims,
        //        authenticationType: nameof(AuthAPI)))));
    }

    public override Task<AuthenticationState> GetAuthenticationStateAsync() => authenticationStateTask;

    public  async Task<AuthResponse> LoginAsync(string email, string senha)
    {
        //var response = await _httpClient.PostAsJsonAsync("auth/login?useCookies=true", new
        //{
        //    email,
        //    password = senha
        //});

        if (email == "fabiohcnobre@hotmail.com")
        {
            
            NotifyAuthenticationStateChanged(GetAuthenticationAsync());
        }
        else
        {
            autenticado = false;
        }

        //if (response.IsSuccessStatusCode)
        //{

        return new AuthResponse { Sucesso = autenticado };
        //}

        //return new AuthResponse { Sucesso = false, Erros = ["Login/senha inválidos"] };
    }

    public static async Task<AuthenticationState> GetAuthenticationAsync()
    {  
        var pessoa = new ClaimsPrincipal();
        //var response = await _httpClient.GetAsync("auth/manage/info");

        UserInfo userInfo = new();
        userInfo.Email = "fabiohcnobre@hotmail.com";
        userInfo.UserId = "1";

        Claim[] claims = [
        new Claim(ClaimTypes.NameIdentifier, userInfo.UserId),
            new Claim(ClaimTypes.Name, userInfo.Email),
            new Claim(ClaimTypes.Email, userInfo.Email),
            new Claim(ClaimTypes.Role, "administrador") ];

        var identity = new ClaimsIdentity(claims, "Cookies");
        pessoa = new ClaimsPrincipal(identity);

        authenticationStateTask = Task.FromResult(
            new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity(claims,
                authenticationType: nameof(AuthAPI)))));

        return new AuthenticationState(pessoa);
    }

    public static async Task<AuthenticationState> GetAuthenticationLogoutAsync()
    {
        var pessoa = new ClaimsPrincipal();
        

        //Claim[] claims = new();

        //var identity = new ClaimsIdentity(claims, "Cookies");
        //pessoa = new ClaimsPrincipal(identity);

        authenticationStateTask = Task.FromResult(
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