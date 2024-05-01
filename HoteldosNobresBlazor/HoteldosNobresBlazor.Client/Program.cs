 
using HoteldosNobresBlazor.Client.API;
using HoteldosNobresBlazor.Client.FuncoesClient; 
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MudBlazor.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args); 
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddMudServices();


await builder.Build().RunAsync();
