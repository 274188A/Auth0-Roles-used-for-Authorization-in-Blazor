using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Policy;

namespace BlazorApp1.Services;

public class BlazorAuthorizationMiddlewareResultHandler : IAuthorizationMiddlewareResultHandler
{
    public async Task HandleAsync(RequestDelegate next, HttpContext context, AuthorizationPolicy policy, PolicyAuthorizationResult authorizeResult)
    {
        if (authorizeResult.Succeeded && context.User.Identity is ClaimsIdentity identity)
        {
            string? audience = context.RequestServices.GetRequiredService<IConfiguration>()["Auth0:Audience"];

            // Retrieve custom roles from Auth0 claim
            var roleClaims = identity.FindAll($"{audience}/roles").ToList();

            // Add mapped role claims to the identity
            foreach (var roleClaim in roleClaims)
            {
                identity.AddClaim(new Claim(ClaimTypes.Role, roleClaim.Value));
            }

            // Replace the user principal with the updated identity
            var newPrincipal = new ClaimsPrincipal(identity);
            context.User = newPrincipal;
        }

        await next(context);
    }
}
