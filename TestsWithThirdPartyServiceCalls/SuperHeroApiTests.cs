using System.Net;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using SuperHeroApi;

namespace TestsWithThirdPartyApiCalls;

public class SuperHeroApiTests: IClassFixture<CustomWebApplicationFactory<Program>>
{
    [Fact(DisplayName = "Get superheros API returns all superheroes")]
    public async Task Get_All_SuperHeroes_Returns_List_Of_SuperHero()
    {
        // Arrange
        var factory = new WebApplicationFactory<Program>();
        var htmlClient = factory.CreateClient();
        // Act
        var response = await htmlClient.GetAsync("/SuperHero");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }
}
    