using Amazon;
using Amazon.Runtime;
using Amazon.SimpleNotificationService;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using SuperHeroApiWith3rdPartyService;
using SuperHeroApiWith3rdPartyService.Data;
using SuperHeroApiWithDatabase;

namespace SuperHero.ApiTests.Utilities;

public class CustomApiFactory(SharedFixture sharedFixture) : WebApplicationFactory<Program>
{
    public SharedFixture SharedFixture => sharedFixture;

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureTestServices(services =>
        {
            services.AddAuthentication(TestAuthHandler.SchemeName)
                .AddScheme<AuthenticationSchemeOptions, TestAuthHandler>(TestAuthHandler.SchemeName, _ => { });

            services.AddScoped(_ => new AuthClaimsProvider());
        });
        
        builder.ConfigureLogging((_, loggingBuilder) =>
        {
            loggingBuilder.ClearProviders();
            loggingBuilder.AddSimpleConsole(); // added to see logs in console
        });
        
        builder.ConfigureServices(services =>
        {
            var snsClient = services.SingleOrDefault(d => d.ServiceType == typeof(IAmazonSimpleNotificationService));
            services.Remove(snsClient);
            services.AddSingleton(GetSnsClient(new Uri(SharedFixture.LocalStackContainer.GetConnectionString())));

            var dbContextDescriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(DbContextOptions<SuperHeroDbContext>));
            services.Remove(dbContextDescriptor!);
        
            var ctx = services.SingleOrDefault(d => d.ServiceType == typeof(SuperHeroDbContext));
            services.Remove(ctx!);
        
            // add back the container-based dbContext
            services.AddDbContext<SuperHeroDbContext>(opts =>
                opts.UseNpgsql(sharedFixture.DatabaseConnectionString));
        });
        
        // Overriding app settings
        builder.ConfigureAppConfiguration((_, configBuilder) =>
        {
            Console.WriteLine($"SNS topic in config {sharedFixture.SnsTopicArn}");
            configBuilder.AddInMemoryCollection(new Dictionary<string, string>
            {
                ["SuspectServiceUrl"] = sharedFixture.SuspectServiceUrlOverride,
                ["AWS:SnsTopicArn"] = sharedFixture.SnsTopicArn
            }!);
        });
    }

    private IAmazonSimpleNotificationService GetSnsClient(Uri serviceUrl)
    {
        var credentials = new BasicAWSCredentials("keyId", "secret");
        var clientConfig = new AmazonSimpleNotificationServiceConfig
        {
            ServiceURL = serviceUrl.ToString()
        };
        return new AmazonSimpleNotificationServiceClient(credentials, clientConfig);
    }
}