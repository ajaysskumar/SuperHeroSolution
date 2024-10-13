using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using SuperHeroApiWithDatabase.Data;

namespace SuperHeroApiWithDatabase.ApiTests.Utilities;

public class CustomApiFactory(SharedFixture sharedFixture) : WebApplicationFactory<Program>
{
    public SharedFixture SharedFixture => sharedFixture;

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            var dbContextDescriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(DbContextOptions<SuperHeroDbContext>));
            services.Remove(dbContextDescriptor!);
        
            var ctx = services.SingleOrDefault(d => d.ServiceType == typeof(SuperHeroDbContext));
            services.Remove(ctx!);
        
            // add back the container-based dbContext
            services.AddDbContext<SuperHeroDbContext>(opts =>
                opts.UseNpgsql(sharedFixture.DatabaseConnectionString));
        });
    }
}