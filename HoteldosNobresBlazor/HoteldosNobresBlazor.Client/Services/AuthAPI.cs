using Google.Apis.PeopleService.v1.Data;
using HoteldosNobresBlazor.Classes;
using HoteldosNobresBlazor.Client;
using HoteldosNobresBlazor.Client.API;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using MudBlazor;
using System.Linq;
using System.Security.Claims;

namespace HoteldosNobresBlazor.Services;

public class AuthAPI  
{
    private static Task<AuthenticationState>  defaultUnauthenticatedTask =
            Task.FromResult(new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity())));

    private Task<AuthenticationState> authenticationStateTask = defaultUnauthenticatedTask;

    private List<Reserva> listaReserva;

    public AuthAPI(List<Reserva> listaReserv) 
    {
        listaReserva = listaReserv; 
    }

    public Task<AuthenticationState> GetAuthenticationStateAsync() => authenticationStateTask;

    public async Task<AuthResponse> LoginAsync(string email, string senha)
    {
        bool autenticado = false;
        if (email == "fabiohcnobre@hotmail.com" && senha == "123")
        {
            autenticado = true;
            GetAuthenticationAsync(email);
        }
        else if (email == "hoteldosnobres@hotmail.com" && senha == "123")
        {
            autenticado = true;

            Reserva reserva = listaReserva.Where(a => (a.Email != null && a.Email.Equals(email))).FirstOrDefault();
             
            GetAuthenticationClienteAsync(reserva.GuestID!, email);
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
                         GetAuthenticationClienteAsync(reserva.GuestID!, email);
                        autenticado = true;
                    }
                }
            }   
        }

        return new AuthResponse { Sucesso = autenticado, User = defaultUnauthenticatedTask.Result.User };
    }

    public async void GetAuthenticationAsync(string email)
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

        //return defaultUnauthenticatedTask.Result;
    }

    public async void GetAuthenticationClienteAsync(string id, string email)
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
         
    }

    public void Logout()
    { 
        defaultUnauthenticatedTask = Task.FromResult(new AuthenticationState(new ClaimsPrincipal()));
        authenticationStateTask = defaultUnauthenticatedTask; 

    }
     
}