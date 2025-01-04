# Blazor App with Aspire and Auth0 Integration

This is a **very simple Blazor app** that integrates **Aspire** and **Auth0** for identity management. The app uses Auth0 to define roles, which are added to the identity token using a **post-login event trigger**.

## Key Features

- **Roles Defined in Auth0**: Roles are set up in Auth0 and injected into the identity token via a post-login trigger.
- **Mapping Auth0 Roles**: The C# code translates Auth0 role claims into standard roles for use within the application.

### Example of Role Claim Mapping Using Trigger Post-Login

```javascript
exports.onExecutePostLogin = async (event, api) => {
  const roleClaim = 'https://blazorserverapp.local';

  if (event.authorization) {
    api.idToken.setCustomClaim(
      `${roleClaim}/roles`, 
      event.authorization.roles
    );
  }
};
```

### Code To Map Auth0 Roles to Standard Roles

The application contains C# logic to map the Auth0 roles:

```csharp
// Add claim mapping logic to include custom roles
builder.Services.Configure<OpenIdConnectOptions>(Auth0Constants.AuthenticationScheme, options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        RoleClaimType = $"{audience}/roles" // Map custom role claim
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
```


