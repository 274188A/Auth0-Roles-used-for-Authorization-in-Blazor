using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;

namespace BlazorApp1.Services;
public class CustomClaimsTransformation : IClaimsTransformation
{
    private readonly IConfiguration _configuration;

    public CustomClaimsTransformation(IConfiguration configuration) => _configuration = configuration;

    public Task<ClaimsPrincipal> TransformAsync(ClaimsPrincipal principal)
    {
        if (principal.Identity is ClaimsIdentity identity)
        {
            string? audience = _configuration["Auth0:Audience"];
            var roleClaims = identity.FindAll($"{audience}/roles").ToList();

            foreach (var roleClaim in roleClaims)
            {
                identity.AddClaim(new Claim(ClaimTypes.Role, roleClaim.Value));
            }
        }

        return Task.FromResult(principal);
    }
}
