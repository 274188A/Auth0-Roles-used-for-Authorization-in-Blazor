using System.Security.Claims;
using Auth0.AspNetCore.Authentication;
using BlazorApp1.Client.Pages;
using BlazorApp1.Components;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCascadingAuthenticationState();

string? audience = builder.Configuration["Auth0:Audience"];

builder.Services
    .AddAuth0WebAppAuthentication(options =>
    {
        options.Domain = builder.Configuration["Auth0:Domain"]!;
        options.ClientId = builder.Configuration["Auth0:ClientId"]!;
        options.ClientSecret = builder.Configuration["Auth0:ClientSecret"];
        options.Scope = "openid profile email";
    })
    .WithAccessToken(options => options.Audience = audience);

// Add claim mapping logic to include custom roles
builder.Services.Configure<OpenIdConnectOptions>(Auth0Constants.AuthenticationScheme, options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        RoleClaimType = "https://blazorserverapp.local/roles" // Map custom role claim
    };

    options.Events = new OpenIdConnectEvents
    {
        OnTokenValidated = context =>
        {
            if (context.Principal?.Identity is ClaimsIdentity identity)
            {
                // Map custom roles to standard role claims
                var roleClaims = identity.FindAll($"{audience}/roles").ToList();
                foreach (var roleClaim in roleClaims)
                {
                    identity.AddClaim(new Claim(ClaimTypes.Role, roleClaim.Value));
                }
            }

            return Task.CompletedTask;
        }
    };
});

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents()
    .AddInteractiveWebAssemblyComponents()
    .AddAuthenticationStateSerialization()
    .AddInteractiveWebAssemblyComponents();

builder.Services.AddScoped<TokenService>();

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

app.MapGet("/Account/Login", async (HttpContext httpContext, string returnUrl = "/") =>
{
    var authenticationProperties = new LoginAuthenticationPropertiesBuilder()
            .WithRedirectUri(returnUrl)
            .Build();

    await httpContext.ChallengeAsync(Auth0Constants.AuthenticationScheme, authenticationProperties);
});

app.MapGet("/Account/Logout", async (HttpContext httpContext) =>
{
    var authenticationProperties = new LogoutAuthenticationPropertiesBuilder()
            .WithRedirectUri("/")
            .Build();

    await httpContext.SignOutAsync(Auth0Constants.AuthenticationScheme, authenticationProperties);
    await httpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
});

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode()    
    .AddInteractiveWebAssemblyRenderMode()
    .AddAdditionalAssemblies(typeof(Counter).Assembly);

app.Run();
