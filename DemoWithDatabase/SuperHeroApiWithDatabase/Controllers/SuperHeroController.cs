using Microsoft.AspNetCore.Mvc;
using SuperHeroApiWithDatabase.Data.Models;
using SuperHeroApiWithDatabase.Data.Repos;

namespace SuperHeroApiWithDatabase.Controllers;

[ApiController]
[Route("[controller]")]
public class SuperHeroController(ISuperHeroRepository superHeroRepository)
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
}