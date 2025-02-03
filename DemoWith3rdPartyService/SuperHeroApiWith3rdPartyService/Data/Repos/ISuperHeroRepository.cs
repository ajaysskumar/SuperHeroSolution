using SuperHeroApiWith3rdPartyService.Data.Models;

namespace SuperHeroApiWith3rdPartyService.Data.Repos;

public interface ISuperHeroRepository
{
    public Task<List<SuperHero>> GetAllSuperHeroes();
    public Task<SuperHero> GetSuperHeroById(int id);
}