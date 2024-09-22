using Microsoft.AspNetCore.Mvc;
using SuperHeroMoviesApi.Data.Models;
using SuperHeroMoviesApi.Data.Repos;

namespace SuperHeroMoviesApi.Controllers;

[ApiController]
[Route("[controller]")]
public class SuperHeroMovieController : ControllerBase
{
    private readonly ILogger<SuperHeroMovieController> _logger;
    private readonly ISuperHeroMovieRepository _superHeroMovieRepository;

    public SuperHeroMovieController(ILogger<SuperHeroMovieController> logger, ISuperHeroMovieRepository superHeroMovieRepository)
    {
        _logger = logger;
        _superHeroMovieRepository = superHeroMovieRepository;
    }

    [HttpGet(Name = "GetSuperHeroMovies")]
    public async Task<IEnumerable<SuperHeroMovie>> Get()
    {
        return await _superHeroMovieRepository.GetAllSuperHeroMovies();
    }
}