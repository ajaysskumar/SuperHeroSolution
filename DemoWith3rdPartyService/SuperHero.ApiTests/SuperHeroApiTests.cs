using System.Net;
using System.Text;
using FluentAssertions;
using SuperHero.ApiTests.Utilities;
using SuperHeroApiWithDatabase.Controllers;
using SuperHeroApiWithDatabase.Data.Models;
using WireMock.Matchers;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;

namespace SuperHero.ApiTests;

[Collection(nameof(IntegrationTestCollection))]
public class SuperHeroApiTests(CustomApiFactory factory): IClassFixture<CustomApiFactory>
{
    [Fact(DisplayName = "Get all superheros API returns all superheroes")]
    public async Task Get_All_SuperHeroes_Returns_List_Of_SuperHero()
    {
        // Arrange
        factory.SharedFixture.SuperHeroDbContext.SuperHero.AddRange(new List<SuperHeroApiWithDatabase.Data.Models.SuperHero>()
        {
            new(1, "Batman","Bruce Wayne","Short distance fly,Common sense","Gotham", 40),
            new(2, "Superman", "Clark kent", "Fly, Shoot laser beam, Super strength, ice breath","Gotham", 42),
            new(3, "Robin", "John Blake", "Detective","Gotham", 35)
        });
        await factory.SharedFixture.SuperHeroDbContext.SaveChangesAsync();
        
        // Act
        var response = await factory.CreateClient().GetAsync("/SuperHero");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var superHeroes = await response.Content.ReadFromJsonAsync<List<SuperHeroApiWithDatabase.Data.Models.SuperHero>>();
        superHeroes.Should().NotBeEmpty();
        superHeroes.Should().Contain(s => s.SuperName == "Batman");
    }
    
    [Fact(DisplayName = "Get superhero by Id returns superhero")]
    public async Task Get_ById_SuperHero_Returns_SuperHero()
    {
        // Arrange
        factory.SharedFixture.SuperHeroDbContext.SuperHero.AddRange(new List<SuperHeroApiWithDatabase.Data.Models.SuperHero>()
        {
            new(4, "Flash","Barry Allen","Lightening fast","Missouri", 28),
        });
        await factory.SharedFixture.SuperHeroDbContext.SaveChangesAsync();
        
        // Act
        var response = await factory.CreateClient().GetAsync("/SuperHero/4");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var superHeroes = await response.Content.ReadFromJsonAsync<SuperHeroApiWithDatabase.Data.Models.SuperHero>();
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
    
    [Fact(DisplayName = "Get suspects should return all matching suspects")]
    public async Task Get_All_Suspects_Returns_List_Of_Matching_Suspects()
    {
        // Arrange
        // Setting up mocked data for success response
        SetupServiceMockForSuspectApi("1", new PersonResponse()
        {
            Data =
            [
                new Suspect()
                {
                    Id = 1,
                    First_Name = "Selina",
                    Last_Name = "Kyle",
                    Email = "selina.kyle@gotham.com",
                }
            ]
        });
        
        // Act
        var response = await factory.CreateClient().GetAsync("/suspects?searchTerm=selina");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var superHeroes = await response.Content.ReadFromJsonAsync<List<Suspect>>();
        superHeroes.Should().NotBeEmpty();
        superHeroes![0].Id.Should().Be(1);
        superHeroes![0].Email.Should().Be("selina.kyle@gotham.com");
        superHeroes![0].First_Name.Should().Be("Selina");
        superHeroes![0].Last_Name.Should().Be("Kyle");
    }
    
    [Fact(DisplayName = "Get suspects should return 500 status code when API responds with 500 status code")]
    public async Task Get_All_Suspects_Should_Return_500_StatusCode_When_3rd_Party_Api_Fails()
    {
        // Arrange
        // Setting up mocked data for failure response
        SetupServiceMockForSuspectApi("1", new {Status = "Failed" }, HttpStatusCode.InternalServerError);
        
        // Act
        var response = await factory.CreateClient().GetAsync("/suspects?searchTerm=selina");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
    }
    
    /// <summary>
    /// This method will take params and will return the list of people/suspects
    /// </summary>
    /// <param name="pageNum">Number of page to look for. Can be any number, but for this problem, lets assume this will always be 1</param>
    /// <param name="apiResponse">Response JSON returned by the API</param>
    /// <param name="expectedStatusCode">Status code returned from the API</param>
    /// <typeparam name="T">Type of the response</typeparam>
    
    private void SetupServiceMockForSuspectApi<T>(string pageNum, T apiResponse, HttpStatusCode expectedStatusCode = HttpStatusCode.OK)
    {
        factory.SharedFixture.WireMockServer
            .Given(Request
                .Create()
                .WithPath("/api/users")
                .UsingGet()
                .WithParam("page", MatchBehaviour.AcceptOnMatch, ignoreCase: true, pageNum))
            .RespondWith(Response
                .Create()
                .WithStatusCode(expectedStatusCode)
                .WithBodyAsJson(apiResponse, Encoding.UTF8));
    }
}
    