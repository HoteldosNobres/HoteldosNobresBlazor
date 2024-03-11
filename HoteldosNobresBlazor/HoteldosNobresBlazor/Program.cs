using HoteldosNobresBlazor.Client.Pages;
using HoteldosNobresBlazor.Components;
using HoteldosNobresBlazor.Funcoes;
using HoteldosNobresBlazor.Services;
using Microsoft.Extensions.DependencyInjection;
using System;
using static System.Formats.Asn1.AsnWriter;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents()
    .AddInteractiveWebAssemblyComponents();


builder.Services.AddHttpClient();
builder.Services.AddBlazorBootstrap();


builder.Services.AddScoped<AppState>();
builder.Services.AddSingleton<AppState>();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
}
else
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();


app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode()
    .AddInteractiveWebAssemblyRenderMode()
    .AddAdditionalAssemblies(typeof(HoteldosNobresBlazor.Client._Imports).Assembly);


//IServiceScope scope = app.Services.CreateAsyncScope();


AppState sCOPP = app.Services.GetRequiredService<AppState>();

var cache = new CacheHotel(sCOPP);

cache.CacheExecutanado();


app.Run();

