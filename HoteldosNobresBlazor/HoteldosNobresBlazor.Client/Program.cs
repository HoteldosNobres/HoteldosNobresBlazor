 
using HoteldosNobresBlazor.Client.API;
using HoteldosNobresBlazor.Client.FuncoesClient; 
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting; 

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<HeadOutlet>("head::after");


 
 
await builder.Build().RunAsync();
