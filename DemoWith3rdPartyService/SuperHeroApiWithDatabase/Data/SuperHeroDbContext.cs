using System.Configuration;
using SuperHeroApiWithDatabase.Data.Models;

namespace SuperHeroApiWithDatabase.Data;

using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;

public class SuperHeroDbContext : DbContext
{
    
    public SuperHeroDbContext(DbContextOptions<SuperHeroDbContext> options)
        : base(options)
    {
    }
    public DbSet<SuperHero> SuperHero { get; set; }
}