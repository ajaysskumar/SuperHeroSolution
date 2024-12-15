using Microsoft.AspNetCore.Mvc;
using SuperHeroApiWithDatabase.Data.Models;
using SuperHeroApiWithDatabase.Data.Repos;

namespace SuperHeroApiWithDatabase.Controllers;

[ApiController]
[Route("[controller]")]
public class SuperHeroController(ISuperHeroRepository superHeroRepository, IConfiguration configuration)
    : ControllerBase
{
    [HttpGet("")]
    public async Task<IEnumerable<SuperHero>> Get()
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


