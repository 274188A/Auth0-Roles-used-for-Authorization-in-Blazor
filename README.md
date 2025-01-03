Very simple Blazor app with Aspire and Auth0 for Identity.  Roles are defined in Auth0 and added to the identidy token using a post login event trigger.

![image](https://github.com/user-attachments/assets/5f7a6388-aad1-4c49-bd46-3519234bee60)
https://auth0.com/docs/customize/actions/explore-triggers

The C# code has to translates those auth0 roles claims into standard roles.
![image](https://github.com/user-attachments/assets/455727c8-a346-428c-887e-f5ef89f55947)

Currently thee is no easy way I know of to also populate the identity with Auth0 Permissions.


