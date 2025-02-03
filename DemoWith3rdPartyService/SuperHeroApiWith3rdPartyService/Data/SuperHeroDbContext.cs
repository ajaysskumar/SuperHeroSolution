using Microsoft.EntityFrameworkCore;
using SuperHeroApiWith3rdPartyService.Data.Models;

namespace SuperHeroApiWith3rdPartyService.Data;

public class SuperHeroDbContext : DbContext
{
    
    public SuperHeroDbContext(DbContextOptions<SuperHeroDbContext> options)
        : base(options)
    {
    }
    public DbSet<SuperHero> SuperHero { get; set; }
}