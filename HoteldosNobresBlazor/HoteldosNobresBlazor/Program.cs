using HoteldosNobresBlazor.Components;
using HoteldosNobresBlazor.Funcoes;
using HoteldosNobresBlazor.Services;
using System.Text;
using Blazored.LocalStorage;
using KEYs = HoteldosNobresBlazor.Funcoes.KEYs; 
using Microsoft.AspNetCore.Components.Authorization;
using HoteldosNobresBlazor.Modelo;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using HoteldosNobresBlazor.Client.API;
using MudBlazor.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddMudServices();

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents()
    .AddInteractiveWebAssemblyComponents();


builder.Services.AddAuthorizationCore();
builder.Services.AddCascadingAuthenticationState();
builder.Services.AddOptions();

//builder.Services.AddScoped<IdentityUserAccessor>();
builder.Services.AddScoped<IdentityRedirectManager>();
builder.Services.AddScoped<AuthenticationStateProvider, AuthAPI>();
builder.Services.AddScoped<AuthAPI>(sp => (AuthAPI)sp.GetRequiredService<AuthenticationStateProvider>());


builder.Services.AddScoped<APICloudbeds>();

builder.Services.AddHttpClient("CloudbedsAPI", client =>
{
    client.BaseAddress = new Uri(builder.Configuration["APICloudbeds:Url"]!);
    client.BaseAddress = new Uri("https://api.cloudbeds.com/api/v1.2");
    client.DefaultRequestHeaders.Add("Accept", "application/json");
    client.DefaultRequestHeaders.Add("Authorization", "Bearer " + KEYs.TOKEN_CLOUDBEDS);
});

builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
}).AddCookie(options =>
{
    options.LoginPath = "/Account/Login";   
});
 
 
//builder.Services.AddAuthentication(options =>
//{
//    options.DefaultScheme = IdentityConstants.ApplicationScheme;
//})
//    .AddIdentityCookies();

//.AddCookie("Admin", options =>
// {
//     options.LoginPath = "/admin/login";
//     options.LogoutPath = "/admin/logout";
// });

builder.Services.AddHttpClient();
builder.Services.AddBlazorBootstrap();

//builder.Services.AddCascadingAuthenticationState();
builder.Services.AddScoped<CookieHandler>();
builder.Services.AddScoped<AppState>();  
builder.Services.AddSingleton<AppState>();
builder.Services.AddSingleton<AuthenticationStateProvider, AuthAPI>();
builder.Services.AddSingleton<IAuthorizationMiddlewareResultHandler, AuthorizationMiddlewareResultHandler>();


//builder.Services.AddBlazoredLocalStorage();   // local storage
builder.Services.AddBlazoredLocalStorage(config => config.JsonSerializerOptions.WriteIndented = true);  // local storage
 
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging(); 
}
else
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true); 
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseAntiforgery();
app.UseStaticFiles();

app.UseAuthentication();
app.UseAuthorization();

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
        await httpContext.Response.WriteAsync(cache2.CacheCreateReservationAsync(body).Result); 
          
    }
    catch (Exception ex)
    {
        await httpContext.Response.WriteAsync(ex.Message);
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
            await httpContext.Response.WriteAsync(ex.Message);
            httpContext.Response.StatusCode = 500;
        }

    });

app.MapPost("/accommodation_changed", async (HttpContext httpContext) =>
{
    try
    {
        using var reader = new StreamReader(httpContext.Request.Body, Encoding.UTF8);
        string body = await reader.ReadToEndAsync();

        CacheHotel cache2 = new CacheHotel(sCOPP);
        await httpContext.Response.WriteAsync(cache2.CacheAccommodation_changed(body));

    }
    catch (Exception ex)
    {
        await httpContext.Response.WriteAsync(ex.Message);
        httpContext.Response.StatusCode = 500;
    }

});

app.MapPost("/details_changed", async (HttpContext httpContext) =>
{
    try
    {
        using var reader = new StreamReader(httpContext.Request.Body, Encoding.UTF8);
        string body = await reader.ReadToEndAsync();

        CacheHotel cache2 = new CacheHotel(sCOPP);
        await httpContext.Response.WriteAsync(cache2.CacheDetails_changed(body));

    }
    catch (Exception ex)
    {
        await httpContext.Response.WriteAsync(ex.Message);
        httpContext.Response.StatusCode = 500;
    }

});

app.MapPost("/whatsapp", async (HttpContext httpContext) =>
{
    try
    {
        using var reader = new StreamReader(httpContext.Request.Body, Encoding.UTF8);
        string body = await reader.ReadToEndAsync();

        //string verify_token = httpContext.Request.Query["hub.verify_token"];
        //string challenge = httpContext.Request.Query["hub.challenge"];
        //string mode = httpContext.Request.Query["hub.mode"];

        sCOPP.MyMessageLogWhatsapp = " -Bodi-  " + body + " -Bodi-  ";
        CacheHotel cache2 = new CacheHotel(sCOPP);
        cache2.RecebeMensagem(body);

        //sCOPP.MyMessage += verify_token + "<verify_token ";
        //sCOPP.MyMessage += challenge + "<challenge ";
        //sCOPP.MyMessage += mode + "<mode ";

        //if (string.IsNullOrEmpty(verify_token))
        //{
        //    httpContext.Response.StatusCode = 400; // Bad Request
        //    await httpContext.Response.WriteAsync("Token não fornecido.");
        //    return;
        //}
        //bool isValid = verify_token.Replace("Bearer ", "") == "TOKENHOTELDOSNOBRES19";
        //if (!isValid)
        //{
        //    httpContext.Response.StatusCode = 401; // Unauthorized
        //    await httpContext.Response.WriteAsync("Token inválido.");
        //    return;
        //}

        //sCOPP.MyMessage += token + " ";

        httpContext.Response.StatusCode = 200;
        //await httpContext.Response.WriteAsync(challenge);

    }
    catch (Exception ex)
    {
        await httpContext.Response.WriteAsync(ex.Message);
        httpContext.Response.StatusCode = 500;
    }

});

app.MapGet("/whatsapp", async (HttpContext httpContext) =>
{
    try
    {
        //string token = httpContext.Request.Headers["Authorization"];
        ////if (string.IsNullOrEmpty(token))
        ////{
        ////    httpContext.Response.StatusCode = 400; // Bad Request
        ////    await httpContext.Response.WriteAsync("Token não fornecido.");
        ////    return;
        ////}

        ////bool isValid = token.Replace("Bearer ", "") == "TOKENHOTELDOSNOBRES19";
        ////if (!isValid)
        ////{
        ////    httpContext.Response.StatusCode = 401; // Unauthorized
        ////    await httpContext.Response.WriteAsync("Token inválido.");
        ////    return;
        ////}
        ///


        using var reader = new StreamReader(httpContext.Request.Body, Encoding.UTF8);
        string body = await reader.ReadToEndAsync();

        string verify_token = httpContext.Request.Query["hub.verify_token"];
        string challenge = httpContext.Request.Query["hub.challenge"];
        string mode = httpContext.Request.Query["hub.mode"];

        sCOPP.MyMessage += body + " ";
        sCOPP.MyMessage += verify_token + " ";
        sCOPP.MyMessage += challenge + " ";
        sCOPP.MyMessage += mode + " ";


        if (string.IsNullOrEmpty(verify_token))
        {
            httpContext.Response.StatusCode = 400; // Bad Request
            await httpContext.Response.WriteAsync("Token não fornecido.");
            return;
        }
        bool isValid = verify_token.Replace("Bearer ", "") == "TOKENHOTELDOSNOBRES19";
        if (!isValid)
        {
            httpContext.Response.StatusCode = 401; // Unauthorized
            await httpContext.Response.WriteAsync("Token inválido.");
            return;
        }


        httpContext.Response.StatusCode = 200;
        await httpContext.Response.WriteAsync(challenge);

    }
    catch (Exception ex)
    {
        await httpContext.Response.WriteAsync(ex.Message);
        httpContext.Response.StatusCode = 500;
    }

});

var cache = new CacheHotel(sCOPP);

cache.CacheExecutanado();

// Add additional endpoints required by the Identity /Account Razor components.
//app.MapAdditionalIdentityEndpoints();

app.MapPost("/Logout", async (
              [FromForm] string returnUrl) =>
{
    AuthAPI authapi = new();
    await authapi.LogoutAsync();
    return TypedResults.LocalRedirect($"~/{returnUrl}");
});

app.Run();

