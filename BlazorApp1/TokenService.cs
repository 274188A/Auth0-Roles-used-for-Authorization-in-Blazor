
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Components.Authorization;

public class TokenService
{
    private readonly AuthenticationStateProvider _authenticationStateProvider;

    public TokenService(AuthenticationStateProvider authenticationStateProvider) => _authenticationStateProvider = authenticationStateProvider;

    public async Task<IEnumerable<Claim>?> GetTokenClaimsAsync()
    {
        var authState = await _authenticationStateProvider.GetAuthenticationStateAsync(); 
        var user = authState.User;

        if (user.Identity?.IsAuthenticated ?? false)
        {
            return user.Claims;            
        }

        return null;
    }
}
