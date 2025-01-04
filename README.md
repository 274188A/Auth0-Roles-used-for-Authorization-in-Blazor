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

![Role Claim Mapping](https://github.com/user-attachments/assets/5f7a6388-aad1-4c49-bd46-3519234bee60)

> For more information on post-login triggers, see the [Auth0 Documentation](https://auth0.com/docs/customize/actions/explore-triggers).

### Code Example

The application contains C# logic to process the Auth0 roles. Below is an illustration:

![C# Code Example](https://github.com/user-attachments/assets/455727c8-a346-428c-887e-f5ef89f55947)

---

## Current Challenge

Looking for a way to **populate the identity with Auth0 permissions**. This may require:

- Creating a **custom claim** to hold permissions.
- Exploring Auth0's available features or post-login customization options.

---

### Next Steps

1. Investigate Auth0's capabilities for adding permissions to identity tokens.
2. Explore custom claims to handle permissions effectively.
3. Experiment with post-login triggers to incorporate permissions seamlessly.

---
