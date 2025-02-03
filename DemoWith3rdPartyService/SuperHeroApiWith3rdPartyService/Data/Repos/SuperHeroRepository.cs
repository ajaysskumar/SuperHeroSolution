using Microsoft.EntityFrameworkCore;
using SuperHeroApiWith3rdPartyService.Data.Models;

namespace SuperHeroApiWith3rdPartyService.Data.Repos;

public class SuperHeroRepository(SuperHeroDbContext context) : ISuperHeroRepository
{
    public async Task<List<SuperHero>> GetAllSuperHeroes()
    {
        return await context.SuperHero.ToListAsync();
    }

    public async Task<SuperHero> GetSuperHeroById(int id)
    {
        return await context.SuperHero.FindAsync(id);
    }
}