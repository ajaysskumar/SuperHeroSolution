using System.Net;
using FluentAssertions;
using SuperHeroApiWithDatabase.ApiTests.Utilities;
using SuperHeroApiWithDatabase.Data.Models;

namespace SuperHeroApiWithDatabase.ApiTests;

[Collection(nameof(IntegrationTestCollection))]
public class SuperHeroApiTests(CustomApiFactory factory): IClassFixture<CustomApiFactory>
{
    [Fact(DisplayName = "Get all superheros API returns all superheroes")]
    public async Task Get_All_SuperHeroes_Returns_List_Of_SuperHero()
    {
        // Arrange
        factory.SharedFixture.SuperHeroDbContext.SuperHero.AddRange(new List<SuperHero>()
        {
            new SuperHero(1, "Batman","Bruce Wayne","Short distance fly,Common sense","Gotham", 40),
            new SuperHero(2, "Superman", "Clark kent", "Fly, Shoot laser beam, Super strength, ice breath","Gotham", 42),
            new SuperHero(3, "Robin", "John Blake", "Detective","Gotham", 35)
        });
        await factory.SharedFixture.SuperHeroDbContext.SaveChangesAsync();
        
        // Act
        var response = await factory.CreateClient().GetAsync("/SuperHero");

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
        factory.SharedFixture.SuperHeroDbContext.SuperHero.AddRange(new List<SuperHero>()
        {
            new SuperHero(4, "Flash","Barry Allen","Lightening fast","Missouri", 28),
        });
        await factory.SharedFixture.SuperHeroDbContext.SaveChangesAsync();
        
        // Act
        var response = await factory.CreateClient().GetAsync("/SuperHero/4");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var superHeroes = await response.Content.ReadFromJsonAsync<SuperHero>();
        superHeroes.Should().NotBeNull();
        superHeroes!.Id.Should().Be(4);
        superHeroes!.SuperName.Should().Be("Flash");
    }
    
    [Fact(DisplayName = "Get superhero by invalid Id returns not found")]
    public async Task Get_By_Invalid_Id_Returns_NotFound()
    {
        // Act
        var response = await factory.CreateClient().GetAsync("/SuperHero/-1");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}
    