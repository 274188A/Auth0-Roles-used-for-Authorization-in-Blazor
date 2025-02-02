using System.Security.Claims;
using Auth0.AspNetCore.Authentication;
using BlazorApp1;
using BlazorApp1.Client;
using BlazorApp1.Client.Pages;
using BlazorApp1.Client.Services;
using BlazorApp1.Components;
using BlazorApp1.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddAuthenticationStateSerialization()
    .AddInteractiveServerComponents()
    .AddInteractiveWebAssemblyComponents();

builder.Services.AddCascadingAuthenticationState();
builder.Services.AddScoped<TokenService>();

string? audience = builder.Configuration["Auth0:Audience"];

builder.Services.AddAuth0WebAppAuthentication(options =>
    {
        options.Domain = builder.Configuration["Auth0:Domain"]!;
        options.ClientId = builder.Configuration["Auth0:ClientId"]!;
        options.ClientSecret = builder.Configuration["Auth0:ClientSecret"];
        options.Scope = "openid profile email";
    })
    .WithAccessToken(options => options.Audience = audience);

builder.Services.AddTransient<IClaimsTransformation, CustomClaimsTransformation>(); // claim mapping logic to include Auth0 roles
builder.Services.AddScoped<HostingEnvironmentService>();
builder.Services.AddSingleton<BaseUrlProvider>();
builder.Services.AddHttpContextAccessor();

builder.Services.AddTransient<CookieHandler>()
    .AddScoped(sp => sp
        .GetRequiredService<IHttpClientFactory>()
        .CreateClient("API"))
    .AddHttpClient("API", (provider, client) =>
    {
        // Get base address
        string? uri = provider.GetRequiredService<BaseUrlProvider>().BaseUrl;
        client.BaseAddress = new Uri(uri!);
    }).AddHttpMessageHandler<CookieHandler>();

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

app.UseAuthentication();
app.UseAuthorization();

app.UseStaticFiles();
app.UseAntiforgery();

app.SetupEndpoints(builder.Configuration);

app.MapStaticAssets();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode()
    .AddInteractiveWebAssemblyRenderMode()
    .AddAdditionalAssemblies(typeof(Counter).Assembly);

app.Run();
