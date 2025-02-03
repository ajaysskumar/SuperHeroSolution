using System.Security.Claims;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace SuperHero.ApiTests.Utilities;

public class TestAuthHandler(
    IOptionsMonitor<AuthenticationSchemeOptions> options,
    ILoggerFactory logger,
    UrlEncoder encoder,
    AuthClaimsProvider claimsProvider)
    : AuthenticationHandler<AuthenticationSchemeOptions>(options, logger, encoder)
{
    public const string SchemeName = "Test";
    private readonly IList<Claim> _claims = claimsProvider.Claims;

    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        if (_claims.IsNullOrEmpty())
        {
            // Since we are injecting authentication ticket in the test setup.
            // We do not want ticket to be generated when there are no claims, i.e. for default flow.
            return Task.FromResult(AuthenticateResult.NoResult());
        }
        
        var identity = new ClaimsIdentity(_claims, SchemeName);
        var principal = new ClaimsPrincipal(identity);
        var ticket = new AuthenticationTicket(principal, SchemeName);

        var result = AuthenticateResult.Success(ticket);

        return Task.FromResult(result);
    }
}