using SuperHeroMoviesApi.Data.Models;

namespace SuperHeroMoviesApi.Data.Repos;

public interface ISuperHeroMovieRepository
{
    public Task<List<SuperHeroMovie>> GetAllSuperHeroMovies();
}