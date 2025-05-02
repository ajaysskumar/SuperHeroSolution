using Amazon.SimpleNotificationService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SuperHeroApiWith3rdPartyService.Data.Dto;
using SuperHeroApiWith3rdPartyService.Data.Models;
using SuperHeroApiWith3rdPartyService.Data.Repos;

namespace SuperHeroApiWith3rdPartyService.Controllers;

[ApiController]
[Route("[controller]")]
public class SuperHeroController(ISuperHeroRepository superHeroRepository, IAmazonSimpleNotificationService snsClient,  IConfiguration configuration)
    : ControllerBase
{
    [HttpGet("")]
    public async Task<IEnumerable<SuperHero>> Get()
    {
        return await superHeroRepository.GetAllSuperHeroes();
    }
    
    [HttpGet("private")]
    [Authorize(Roles = "Admin")]
    public async Task<IEnumerable<SuperHero>> GetPrivate()
    {
        return await superHeroRepository.GetAllSuperHeroes();
    }
    
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var superHero = await superHeroRepository.GetSuperHeroById(id);
        if (superHero == null)
        {
            return NotFound();
        }

        return Ok(superHero);
    }
    
    [HttpGet("/suspects")]
    public async Task<IActionResult> Suspects([FromQuery]string searchTerm)
    {
        var people = await GetPeople();
        var peopleOfInterest = people.Data.Where(person =>
            person.First_Name.Contains(searchTerm, StringComparison.InvariantCultureIgnoreCase) ||
            person.Last_Name.Contains(searchTerm, StringComparison.InvariantCultureIgnoreCase) ||
            person.Email.Contains(searchTerm, StringComparison.InvariantCultureIgnoreCase));
        if (!peopleOfInterest.Any())
        {
            return NotFound();
        }
        return Ok(peopleOfInterest);
    }

    [HttpPost("call-superhero")]
    public async Task<IActionResult> CallSuperHero(string superHeroName)
    {
        // Simulate calling a superhero
        var superHero = await superHeroRepository.GetAllSuperHeroes();
        var hero = superHero.FirstOrDefault(h => h.SuperName.Equals(superHeroName, StringComparison.InvariantCultureIgnoreCase));

        if (hero == null)
        {
            return NotFound($"Superhero with name '{superHeroName}' not found.");
        }

        // Publish an SNS notification
        var region = configuration.GetValue<string>("AWS:Region");
        var topicArn = configuration.GetValue<string>("AWS:SnsTopicArn"); // Ensure this is configured in appsettings.json
        var message = $"Calling {hero.SuperName}! They are on their way to save the day!";
        var publishRequest = new Amazon.SimpleNotificationService.Model.PublishRequest
        {
            TopicArn = topicArn,
            Message = message,
            Subject = "Superhero Alert"
        };
        Console.WriteLine($"Publishing message to SNS topic {topicArn}");

        try
        {
            var response = await snsClient.PublishAsync(publishRequest);
            if (response.HttpStatusCode != System.Net.HttpStatusCode.OK)
            {
                return StatusCode((int)response.HttpStatusCode, "Failed to send SNS notification.");
            }
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Error sending SNS notification: {ex.Message}");
        }

        return Ok($"Calling {hero.SuperName}! They are on their way to save the day!");
    }

    private async Task<PersonResponse> GetPeople()
    {
        using var client = new HttpClient();
        
        var url = $"{configuration.GetValue<string>("SuspectServiceUrl")}/api/users?page=1";
        var response = await client.GetAsync(url);
        if (!response.IsSuccessStatusCode)
        {
            throw new HttpRequestException($"Unable to get people from {url}");
        }
            
        return await response.Content.ReadFromJsonAsync<PersonResponse>();
    }
}


