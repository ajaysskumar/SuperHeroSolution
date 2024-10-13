using SuperHeroApiWithDatabase.Data.Models;

namespace SuperHeroApiWithDatabase.Data.Repos;

public interface ISuperHeroRepository
{
    public Task<List<SuperHero>> GetAllSuperHeroes();
    public Task<SuperHero> GetSuperHeroById(int id);
}