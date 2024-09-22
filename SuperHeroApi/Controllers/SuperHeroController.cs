using Microsoft.AspNetCore.Mvc;
using SuperHeroApi.Data.Models;
using SuperHeroApi.Data.Repos;

namespace SuperHeroApi.Controllers;

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
    public async Task<SuperHero> GetById(int id)
    {
        return await superHeroRepository.GetSuperHeroById(id);
    }
}