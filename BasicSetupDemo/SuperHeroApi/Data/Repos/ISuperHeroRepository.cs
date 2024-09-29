using SuperHeroApi.Data.Models;

namespace SuperHeroApi.Data.Repos;

public interface ISuperHeroRepository
{
    public Task<List<SuperHero>> GetAllSuperHeroes();
    public Task<SuperHero> GetSuperHeroById(int id);
}