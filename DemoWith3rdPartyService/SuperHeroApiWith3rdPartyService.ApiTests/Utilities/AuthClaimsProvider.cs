using System.Security.Claims;

namespace SuperHero.ApiTests.Utilities;

public class AuthClaimsProvider
{
    public IList<Claim> Claims { get; } = new List<Claim>();

    public static AuthClaimsProvider WithAdminClaim()
    {
        var provider = new AuthClaimsProvider();
        provider.Claims.Add(new Claim(ClaimTypes.Role, "Admin"));

        return provider;
    }
    
    public static AuthClaimsProvider WithAnonymousClaim()
    {
        var provider = new AuthClaimsProvider();
        provider.Claims.Add(new Claim(ClaimTypes.Role, "Anonymous"));

        return provider;
    }
}