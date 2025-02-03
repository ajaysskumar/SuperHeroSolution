using Microsoft.AspNetCore.TestHost;

namespace SuperHero.ApiTests.Utilities;

public static class WebApplicationFactoryExtensions
{
    public static HttpClient CreateClientWithClaim(this CustomApiFactory factory, AuthClaimsProvider claims)
    {
        var client = factory.WithWebHostBuilder(builder =>
            {
                builder.ConfigureTestServices(services =>
                {
                    var descriptor = Enumerable.SingleOrDefault<ServiceDescriptor>(services, d => d.ServiceType == typeof(AuthClaimsProvider));

                    if (descriptor != null)
                    {
                        services.Remove(descriptor);
                        ServiceCollectionServiceExtensions.AddScoped(services, _ => claims);
                    }
                });
            })
            .CreateClient();

        return client;
    }
}