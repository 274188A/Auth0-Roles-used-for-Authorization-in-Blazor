using Auth0.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace BlazorApp1;

public static class AuthenticationExtensions
{
    public static IEndpointRouteBuilder SetupEndpoints(this IEndpointRouteBuilder app, IConfiguration configuration)
    {
        // Login endpoint
        app.MapGet("/Account/Login", async (HttpContext httpContext, string returnUrl = "/") =>
            await httpContext.ChallengeAsync(Auth0Constants.AuthenticationScheme,
                new AuthenticationProperties
                {
                    RedirectUri = !string.IsNullOrEmpty(returnUrl) ? returnUrl : "/"
                }));

        // Logout endpoint
        app.MapGet("/Account/Logout", async (HttpContext httpContext) =>
        {
            // Dynamically build the returnTo URL using the current request
            var request = httpContext.Request;
            string returnTo = $"{request.Scheme}://{request.Host}/";

            string logoutUrl = $"https://{configuration["Auth0:Domain"]!}/v2/logout?client_id={configuration["Auth0:ClientId"]!}&returnTo={Uri.EscapeDataString(returnTo)}";

            // Sign out of the local cookie authentication
            await httpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            // Redirect to Auth0 logout endpoint
            httpContext.Response.Redirect(logoutUrl);
        });

        return app;
    }
}
