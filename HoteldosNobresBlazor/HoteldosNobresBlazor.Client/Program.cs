
using HoteldosNobresBlazor.Client;
using HoteldosNobresBlazor.Services;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MudBlazor.Services;
  
var builder = WebAssemblyHostBuilder.CreateDefault(args);
 
builder.RootComponents.Add<HeadOutlet>("head::after");
  
builder.Services.AddMudServices();
 
builder.Services.AddAuthorizationCore();
builder.Services.AddCascadingAuthenticationState();

builder.Services.AddScoped<PersistentAuthenticationStateProvider>();
builder.Services.AddSingleton<AuthenticationStateProvider, PersistentAuthenticationStateProvider>();

//builder.Services.AddScoped<AuthAPI>();
//builder.Services.AddScoped<AuthAPI>(sp => (AuthAPI)sp.GetRequiredService<AuthenticationStateProvider>());
//builder.Services.AddSingleton<AuthenticationStateProvider, AuthAPI>();



await builder.Build().RunAsync();

