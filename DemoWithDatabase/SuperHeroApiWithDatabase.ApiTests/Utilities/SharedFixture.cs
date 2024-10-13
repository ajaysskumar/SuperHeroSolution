using Microsoft.EntityFrameworkCore;
using SuperHeroApiWithDatabase.Data;
using Testcontainers.PostgreSql;

namespace SuperHeroIntegrationTests.Utilities;

public class SharedFixture: IAsyncLifetime
{
    private SuperHeroDbContext? _dbContext;
    
    private readonly PostgreSqlContainer _dbContainer =
        new PostgreSqlBuilder()
            .WithDatabase("offerbox")
            .WithUsername("postgres")
            .WithPassword("postgres")
            .Build();
    
    public string DatabaseConnectionString => _dbContainer.GetConnectionString();
    public SuperHeroDbContext SuperHeroDbContext => _dbContext;
    
    public async Task InitializeAsync()
    {
        await _dbContainer.StartAsync();
        
        var optionsBuilder = new DbContextOptionsBuilder<SuperHeroDbContext>()
            .UseNpgsql(DatabaseConnectionString);
        _dbContext = new SuperHeroDbContext(optionsBuilder.Options);
        await _dbContext.Database.MigrateAsync();
    }

    public async Task DisposeAsync()
    {
        await _dbContainer.DisposeAsync();
    }
}