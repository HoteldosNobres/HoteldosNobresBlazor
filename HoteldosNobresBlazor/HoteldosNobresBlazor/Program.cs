using HoteldosNobresBlazor.Components;
using HoteldosNobresBlazor.Services;
using Blazored.LocalStorage;
using KEYs = HoteldosNobresBlazor.Funcoes.KEYs;
using Microsoft.AspNetCore.Components.Authorization;
using HoteldosNobresBlazor.Modelo;
using Microsoft.AspNetCore.Authorization;
using HoteldosNobresBlazor.Client.API;
using MudBlazor.Services;
using Microsoft.AspNetCore.Identity;
using HoteldosNobresBlazor;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddMudServices();

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents()
    .AddInteractiveWebAssemblyComponents();

builder.Services.AddAuthorization(options =>
{
options.AddPolicy("Policy_Name", x => x.RequireRole("admin", "client") );
});
builder.Services.AddCascadingAuthenticationState();
builder.Services.AddOptions();

builder.Services.AddScoped<IdentityRedirectManager>();
builder.Services.AddScoped<AuthenticationStateProvider, PersistingRevalidatingAuthenticationStateProvider>();

builder.Services.AddAuthentication().AddCookie();

builder.Services.AddDbContext<ApplicationDbContext>(); 
builder.Services.AddIdentityCore<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddSignInManager()
    .AddDefaultTokenProviders();

builder.Services.AddScoped<APICloudbeds>();

builder.Services.AddHttpClient("CloudbedsAPI", client =>
{
    client.BaseAddress = new Uri(builder.Configuration["APICloudbeds:Url"]!);
    client.BaseAddress = new Uri("https://api.cloudbeds.com/api/v1.2");
    client.DefaultRequestHeaders.Add("Accept", "application/json");
    client.DefaultRequestHeaders.Add("Authorization", "Bearer " + KEYs.TOKEN_CLOUDBEDS);
});

builder.Services.AddHttpClient(); 

builder.Services.AddScoped<CookieHandler>();
builder.Services.AddScoped<AppState>();  
builder.Services.AddSingleton<AppState>();
builder.Services.AddSingleton<IAuthorizationMiddlewareResultHandler, AuthorizationMiddlewareResultHandler>();

builder.Services.AddBlazoredLocalStorage(config => config.JsonSerializerOptions.WriteIndented = true);  // local storage
 
var app = builder.Build();

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
 
app.AddEndPointsSisTema();
 
app.Run();

