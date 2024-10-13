using SuperHeroApiWithDatabase.Data;

namespace SuperHeroIntegrationTests.Utilities;

public abstract class BaseTest(CustomApiFactory factory)
{
    protected HttpClient Client { get; } = factory.CreateClient();
    protected SharedFixture SharedContext { get; } = factory.SharedFixture;
    protected SuperHeroDbContext DbContext { get; } = factory.SharedFixture.SuperHeroDbContext;
}