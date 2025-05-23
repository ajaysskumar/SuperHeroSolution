using System.Net;
using System.Text;
using System.Text.Json.Nodes;
using FluentAssertions;
using SuperHero.ApiTests.Utilities;
using SuperHeroApiWith3rdPartyService.Data.Dto;
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
        factory.SharedFixture.SuperHeroDbContext.SuperHero.AddRange(new List<SuperHeroApiWith3rdPartyService.Data.Models.SuperHero>()
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
        var superHeroes = await response.Content.ReadFromJsonAsync<List<SuperHeroApiWith3rdPartyService.Data.Models.SuperHero>>();
        superHeroes.Should().NotBeEmpty();
        superHeroes.Should().Contain(s => s.SuperName == "Batman");
    }
    
    [Fact(DisplayName = "Get all superheroes private API returns 401 when request is unauthenticated")]
    public async Task Get_All_SuperHeroes_Private_Returns_401_When_Request_Is_Unauthenticated()
    {
        // Act
        using var httpClient = factory.CreateClient(); // Default http client without any token
        var response = await httpClient.GetAsync("/SuperHero/private");
        
        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
    
    [Fact(DisplayName = "Get all superheroes private API returns 403 when request is missing required claims")]
    public async Task Get_All_SuperHeroes_Private_Returns_403_When_Request_Is_Missing_Required_Claims()
    {
        // Act
        using var httpClient = factory.CreateClientWithClaim(AuthClaimsProvider.WithAnonymousClaim()); // Default http client without any token
        var response = await httpClient.GetAsync("/SuperHero/private");
        
        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }
    
    [Fact(DisplayName = "Get all superheroes authenticated API returns all superheroes")]
    public async Task Get_All_SuperHeroes_Authenticated_Returns_List_Of_SuperHero()
    {
        // Arrange
        factory.SharedFixture.SuperHeroDbContext.SuperHero.AddRange(new List<SuperHeroApiWith3rdPartyService.Data.Models.SuperHero>()
        {
            new(11, "Batman","Bruce Wayne","Short distance fly,Common sense","Gotham", 40),
            new(22, "Superman", "Clark kent", "Fly, Shoot laser beam, Super strength, ice breath","Gotham", 42),
            new(33, "Robin", "John Blake", "Detective","Gotham", 35)
        });
        await factory.SharedFixture.SuperHeroDbContext.SaveChangesAsync();
        
        // Act
        using var httpClient = factory.CreateClientWithClaim(AuthClaimsProvider.WithAdminClaim());
        var response = await httpClient.GetAsync("/SuperHero/private");
        
        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var superHeroes = await response.Content.ReadFromJsonAsync<List<SuperHeroApiWith3rdPartyService.Data.Models.SuperHero>>();
        superHeroes.Should().NotBeEmpty();
        superHeroes.Should().Contain(s => s.SuperName == "Batman");
    }
    
    [Fact(DisplayName = "Get superhero by Id returns superhero")]
    public async Task Get_ById_SuperHero_Returns_SuperHero()
    {
        // Arrange
        factory.SharedFixture.SuperHeroDbContext.SuperHero.AddRange(new List<SuperHeroApiWith3rdPartyService.Data.Models.SuperHero>()
        {
            new(4, "Flash","Barry Allen","Lightening fast","Missouri", 28),
        });
        await factory.SharedFixture.SuperHeroDbContext.SaveChangesAsync();
        
        // Act
        var response = await factory.CreateClient().GetAsync("/SuperHero/4");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var superHeroes = await response.Content.ReadFromJsonAsync<SuperHeroApiWith3rdPartyService.Data.Models.SuperHero>();
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

    [Fact(DisplayName = "CallSuperHero API raises correct SNS notification using LocalStack")]
    public async Task CallSuperHero_Raises_Correct_SNS_Notification_Using_LocalStack()
    {
        // Arrange
        var superHeroName = "Venom";
        factory.SharedFixture.SuperHeroDbContext.SuperHero.AddRange(new List<SuperHeroApiWith3rdPartyService.Data.Models.SuperHero>()
        {
            new(5, "Venom", "Eddie Brock", "Super strength, Shape-shifting, Healing factor", "San Francisco", 35),
        });
        await factory.SharedFixture.SuperHeroDbContext.SaveChangesAsync();

        // Subscribe the SQS queue to the SNS topic
        var queueName = "superhero-called-queue";
        var queue = await factory.SharedFixture.LocalStackContainer.ExecAsync(["awslocal", "sqs", "create-queue", "--queue-name", queueName]);
        var queueUrl = JsonNode.Parse(queue.Stdout)!["QueueUrl"]!.ToString();
        var queueAttributeResult = await factory.SharedFixture.LocalStackContainer.ExecAsync(["awslocal", "sqs", "get-queue-attributes", "--queue-url", queueUrl, "--attribute-names", "All"]);
        var queueArn = JsonNode.Parse(queueAttributeResult.Stdout)!["Attributes"]!["QueueArn"]!.ToString();
        await factory.SharedFixture.LocalStackContainer.ExecAsync(["awslocal", "sns", "subscribe", "--topic-arn", factory.SharedFixture.SnsTopicArn, "--protocol", "sqs", "--notification-endpoint", queueArn]);

        // Act
        var response = await factory.CreateClient().PostAsJsonAsync($"/SuperHero/call-superhero?superHeroName={superHeroName}", new { });

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var responseMessage = await response.Content.ReadAsStringAsync();
        responseMessage.Should().Contain($"Calling {superHeroName}! They are on their way to save the day!");

        // Verify SNS message was published
        var messages = await factory.SharedFixture.LocalStackContainer.ExecAsync(
        [
            "awslocal", "sqs", "receive-message", "--queue-url", queueUrl
        ]);
        var sqsMessages = JsonNode.Parse(messages.Stdout);
        var message = sqsMessages!["Messages"]![0]!["Body"]!.ToString();
        message.Should().Contain($"Calling {superHeroName}! They are on their way to save the day!");
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
    