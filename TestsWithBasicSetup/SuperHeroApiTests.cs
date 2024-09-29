using System.Net;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Newtonsoft.Json;
using SuperHeroApi;
using SuperHeroApi.Data.Models;

namespace SuperHeroIntegrationTests;

public class SuperHeroApiTests: IClassFixture<WebApplicationFactory<Program>>
{
    [Fact(DisplayName = "Get all superheros API returns all superheroes")]
    public async Task Get_All_SuperHeroes_Returns_List_Of_SuperHero()
    {
        // Arrange
        var factory = new WebApplicationFactory<Program>();
        var htmlClient = factory.CreateClient();
        // Act
        var response = await htmlClient.GetAsync("/SuperHero");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var superHeroes = await response.Content.ReadFromJsonAsync<List<SuperHero>>();
        superHeroes.Should().NotBeEmpty();
        superHeroes![0].Id.Should().Be(1);
        superHeroes![0].SuperName.Should().Be("Batman");
    }
    
    [Fact(DisplayName = "Get superhero by Id returns superhero")]
    public async Task Get_ById_SuperHero_Returns_SuperHero()
    {
        // Arrange
        var factory = new WebApplicationFactory<Program>();
        var htmlClient = factory.CreateClient();
        // Act
        var response = await htmlClient.GetAsync("/SuperHero/1");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var superHeroes = await response.Content.ReadFromJsonAsync<SuperHero>();
        superHeroes.Should().NotBeNull();
        superHeroes!.Id.Should().Be(1);
        superHeroes!.SuperName.Should().Be("Batman");
    }
    
    [Fact(DisplayName = "Get superhero by invalid Id returns not found")]
    public async Task Get_By_Invalid_Id_Returns_NotFound()
    {
        // Arrange
        var factory = new WebApplicationFactory<Program>();
        var htmlClient = factory.CreateClient();
        // Act
        var response = await htmlClient.GetAsync("/SuperHero/-1");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}
    