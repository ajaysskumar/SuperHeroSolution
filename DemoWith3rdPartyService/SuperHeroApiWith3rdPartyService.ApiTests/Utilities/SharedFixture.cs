using System.Text.Json.Nodes;
using Amazon;
using Amazon.Runtime;
using Amazon.SimpleNotificationService;
using Amazon.SimpleNotificationService.Model;
using DotNet.Testcontainers.Builders;
using Microsoft.EntityFrameworkCore;
using SuperHeroApiWith3rdPartyService.Data;
using Testcontainers.LocalStack;
using Testcontainers.PostgreSql;
using WireMock.Server;

namespace SuperHero.ApiTests.Utilities;

public class SharedFixture : IAsyncLifetime
{
    // Define the Suspect base URL to be overridden during application run
    public string SuspectServiceUrlOverride { get; private set; } = null!;

    private SuperHeroDbContext? _dbContext;

    // Local wiremock server instance
    private WireMockServer? _server;

    private readonly PostgreSqlContainer _dbContainer =
        new PostgreSqlBuilder()
            .WithDatabase("superhero")
            .WithUsername("postgres")
            .WithPassword("postgres")
            .Build();

    private readonly LocalStackContainer _localStackContainer =
        new LocalStackBuilder()
            .WithImage("localstack/localstack")
            .WithWaitStrategy(Wait.ForUnixContainer().UntilMessageIsLogged("Ready."))
            .WithCleanUp(true)
            .WithAutoRemove(true)
            .Build();

    public string DatabaseConnectionString => _dbContainer.GetConnectionString();
    public SuperHeroDbContext SuperHeroDbContext => _dbContext;

    /*WireMockServer shared property to be used in the individual tests
     It is shared context so that it can be used in different tests scenarios
     using different configuration. Like success response mock, failure response
     mock etc.
    */
    public WireMockServer WireMockServer => _server;
    public LocalStackContainer LocalStackContainer => _localStackContainer;

    public string SnsTopicArn { get; private set; }

    public async Task InitializeAsync()
    {
        await _dbContainer.StartAsync();

        // Start the localstack container
        await _localStackContainer.StartAsync();

        var optionsBuilder = new DbContextOptionsBuilder<SuperHeroDbContext>()
            .UseNpgsql(DatabaseConnectionString);
        _dbContext = new SuperHeroDbContext(optionsBuilder.Options);
        await _dbContext.Database.MigrateAsync();

        /* Assigning the mocked base URL to replace the actual Suspect service URL
        at runtime
        */
        SuspectServiceUrlOverride = StartWireMockForService();
        string? topicArn = await CreateSnsTopic();

        // Set the SNS topic ARN
        SnsTopicArn = topicArn;
    }

    private async Task<string?> CreateSnsTopic()
    {
        // Create the SNS topic using LocalStackContainer.ExecAsync
        var createTopicResult = await _localStackContainer.ExecAsync(
        [
            "awslocal", "sns", "create-topic", "--name", "dev-superhero-called"
        ]);

        // Parse the output to extract the TopicArn
        var createTopicOutput = createTopicResult.Stdout;
        var topicArn = JsonNode.Parse(createTopicOutput)?["TopicArn"]?.ToString();
        if (string.IsNullOrEmpty(topicArn))
        {
            throw new InvalidOperationException("Failed to create SNS topic in LocalStack.");
        }

        return topicArn;
    }

    public async Task DisposeAsync()
    {
        await _dbContainer.DisposeAsync();
        await _localStackContainer.DisposeAsync();
    }

    /* Starting wiremock service and returning the mocked up server URL */
    private string StartWireMockForService()
    {
        _server = WireMockServer.Start();

        return _server.Urls[0];
    }
}