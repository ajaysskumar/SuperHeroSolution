using Microsoft.EntityFrameworkCore;
using SuperHeroApiWithDatabase.Data;
using Testcontainers.PostgreSql;
using WireMock.Server;

public class SharedFixture: IAsyncLifetime
{
    // Define the Suspect base URL to be overridden during application run
    public string SuspectServiceUrlOverride { get; private set; } = null!;
    
    private SuperHeroDbContext? _dbContext;
    
    // Locale wiremock server instance
    private WireMockServer? _server;
    
    private readonly PostgreSqlContainer _dbContainer =
        new PostgreSqlBuilder()
            .WithDatabase("superhero")
            .WithUsername("postgres")
            .WithPassword("postgres")
            .Build();
    
    public string DatabaseConnectionString => _dbContainer.GetConnectionString();
    public SuperHeroDbContext SuperHeroDbContext => _dbContext;
    
    /*WireMockServer shared property to be used in the individual tests
     It is shared context so that it can be used in different tests scenarios
     using different configuration. Like success response mock, failure response
     mock etc.
    */ 
    public WireMockServer WireMockServer => _server;
    
    public async Task InitializeAsync()
    {
        await _dbContainer.StartAsync();
        
        var optionsBuilder = new DbContextOptionsBuilder<SuperHeroDbContext>()
            .UseNpgsql(DatabaseConnectionString);
        _dbContext = new SuperHeroDbContext(optionsBuilder.Options);
        await _dbContext.Database.MigrateAsync();
        
        /* Assigning the mocked base URL to replace the actual Suspect service URL
            at runtime
        */
        SuspectServiceUrlOverride = StartWireMockForService();
    }

    public async Task DisposeAsync()
    {
        await _dbContainer.DisposeAsync();
    }
    
    /* Starting wiremock service and returning the mocked up server URL */
    private string StartWireMockForService()
    {
        _server = WireMockServer.Start();

        return _server.Urls[0];
    }
}