using Microsoft.EntityFrameworkCore;
using SuperHeroApiWithDatabase.Data;
using Testcontainers.PostgreSql;
using WireMock.Server;

namespace SuperHero.ApiTests.Utilities;

public class SharedFixture: IAsyncLifetime
{
    public string SuspectServiceUrlOverride { get; private set; } = null!;
    private SuperHeroDbContext? _dbContext;
    private WireMockServer? _server;
    
    private readonly PostgreSqlContainer _dbContainer =
        new PostgreSqlBuilder()
            .WithDatabase("superhero")
            .WithUsername("postgres")
            .WithPassword("postgres")
            .Build();
    
    public string DatabaseConnectionString => _dbContainer.GetConnectionString();
    public SuperHeroDbContext SuperHeroDbContext => _dbContext;
    
    public WireMockServer WireMockServer => _server;
    
    public async Task InitializeAsync()
    {
        await _dbContainer.StartAsync();
        
        var optionsBuilder = new DbContextOptionsBuilder<SuperHeroDbContext>()
            .UseNpgsql(DatabaseConnectionString);
        _dbContext = new SuperHeroDbContext(optionsBuilder.Options);
        await _dbContext.Database.MigrateAsync();

        SuspectServiceUrlOverride = StartWireMockForService();
    }

    public async Task DisposeAsync()
    {
        await _dbContainer.DisposeAsync();
    }
    
    private string StartWireMockForService()
    {
        _server = WireMockServer.Start();

        return _server.Urls[0];
    }
}