using Google.Apis.PeopleService.v1.Data;
using HoteldosNobresBlazor.Classes;
using HoteldosNobresBlazor.Client;
using HoteldosNobresBlazor.Client.API;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using MudBlazor;
using System.Security.Claims;

namespace HoteldosNobresBlazor.Services;

public class AuthAPI : AuthenticationStateProvider
{
    private static Task<AuthenticationState>  defaultUnauthenticatedTask =
            Task.FromResult(new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity())));

    private Task<AuthenticationState> authenticationStateTask = defaultUnauthenticatedTask;

    private List<Reserva> listaReserva;

    public AuthAPI(List<Reserva> listaReserv) 
    {
        listaReserva = listaReserv; 
    }

    public override Task<AuthenticationState> GetAuthenticationStateAsync() => authenticationStateTask;

    public async Task<AuthResponse> LoginAsync(string email, string senha)
    {
        bool autenticado = false;
        if (email == "fabiohcnobre@hotmail.com" && senha == "123")
        {
            autenticado = true;
            NotifyAuthenticationStateChanged(GetAuthenticationAsync(email));
        }
        else if (email == "hoteldosnobres@hotmail.com" && senha == "123")
        {
            autenticado = true;
            NotifyAuthenticationStateChanged(GetAuthenticationClienteAsync("1", email));
        }
        else
        {
            List<Reserva> listaReservaaqui;
            listaReservaaqui = listaReserva.Where(a => (a.Email != null && a.Email.Equals(email))
            || a.IDReserva.Equals(email)
            ||  (a.IDReservaAgencia != null && a.IDReservaAgencia.Equals(email))
            ).ToList();
            if(listaReservaaqui.Count > 0)
            {
                foreach (var reserva in listaReservaaqui)
                {
                    if (!string.IsNullOrEmpty(reserva.ProxyCelular)
                       && reserva.ProxyCelular.Length > 4
                       && reserva.ProxyCelular.Substring(reserva.ProxyCelular.Length - 4) == senha)
                    {
                        email = string.IsNullOrEmpty(reserva.Email) ? email : reserva.Email;
                        NotifyAuthenticationStateChanged(GetAuthenticationClienteAsync(reserva.GuestID!, email));
                        autenticado = true;
                    }
                }
            }   
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

    public async Task<AuthenticationState> GetAuthenticationClienteAsync(string id, string email)
    {
        var pessoa = new ClaimsPrincipal(); 
        //var response = await _httpClient.GetAsync("auth/manage/info");

        UserInfo userInfo = new();
        userInfo.Email = email;
        userInfo.UserId = id;

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