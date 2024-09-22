using Microsoft.EntityFrameworkCore;
using SuperHeroMoviesApi.Data.Models;

namespace SuperHeroMoviesApi.Data.Repos;

public class SuperHeroMovieMovieRepository(SuperHeroMovieDbContext context) : ISuperHeroMovieRepository
{
    public async Task<List<SuperHeroMovie>> GetAllSuperHeroMovies()
    {
        return await context.Movies.ToListAsync();
    }
}