using HoteldosNobresBlazor.Client.Pages;
using HoteldosNobresBlazor.Components;
using HoteldosNobresBlazor.Funcoes;
using HoteldosNobresBlazor.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.DependencyInjection;
using System;
using static System.Formats.Asn1.AsnWriter;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System.Net.Mime;

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
app.UseRouting();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode()
    .AddInteractiveWebAssemblyRenderMode()
    .AddAdditionalAssemblies(typeof(HoteldosNobresBlazor.Client._Imports).Assembly);


AppState sCOPP = app.Services.GetRequiredService<AppState>();


app.MapPost("/addreserva", async (HttpContext httpContext) =>
{
    try
    {
        using var reader = new StreamReader(httpContext.Request.Body, Encoding.UTF8);
        string body = await reader.ReadToEndAsync();

        CacheHotel cache2 = new CacheHotel(sCOPP);  
        await httpContext.Response.WriteAsync(cache2.CacheNovaReserva(body)); 
          
    }
    catch (Exception ex)
    {
        httpContext.Response.StatusCode = 500;
    }

});

app.MapPost("/status_changed", async (HttpContext httpContext) =>
    {
        try
        {
            using var reader = new StreamReader(httpContext.Request.Body, Encoding.UTF8);
            string body = await reader.ReadToEndAsync();

            CacheHotel cache2 = new CacheHotel(sCOPP); 
            await httpContext.Response.WriteAsync(cache2.CacheChangedStatus(body));

        }
        catch (Exception ex)
        {
            httpContext.Response.StatusCode = 500;
        }

    });

var cache = new CacheHotel(sCOPP);

cache.CacheExecutanado();


app.Run();

