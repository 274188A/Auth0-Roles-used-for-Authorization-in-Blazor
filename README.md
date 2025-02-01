# Blazor App with Aspire and Auth0 Integration

The app uses Auth0 to define roles, which are added to the identity token using a **post-login event trigger** on the Auth0 cloud side.
The Blazor app then maps the Auth0 roles to standard roles for use within the application using the ClaimsTransformation feature in the OpenID Connect middleware.

## Key Features

- **Roles Defined in Auth0**: Roles are set up in Auth0 and injected into the identity token via a post-login trigger.
- **Mapping Auth0 Roles**: The C# code translates Auth0 role claims into standard roles for use within the application.

### Example of Role Claim Mapping Using Trigger Post-Login Trigger

```javascript
exports.onExecutePostLogin = async (event, api) => {
  const roleClaim = 'https://blazorserverapp.local';
  if (event.authorization) {
    api.idToken.setCustomClaim(${roleClaim}/roles, event.authorization.roles);
  }
};
```

### Code To Map Auth0 Roles to Standard Roles

The application contains C# logic to map the Auth0 roles:

```csharp
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
```


