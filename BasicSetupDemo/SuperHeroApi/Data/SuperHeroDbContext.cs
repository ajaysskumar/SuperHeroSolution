using SuperHeroApi.Data.Models;

namespace SuperHeroApi.Data;

using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;

public class SuperHeroDbContext : DbContext
{
    public DbSet<SuperHero> SuperHero { get; set; }
    public string DbPath { get; }

    public SuperHeroDbContext(DbContextOptions<SuperHeroDbContext> options)
        : base(options)
    {
        const Environment.SpecialFolder folder = Environment.SpecialFolder.LocalApplicationData;
        var path = Environment.GetFolderPath(folder);
        DbPath = System.IO.Path.Join(path, "blogging.db");
    }

    // The following configures EF to create a Sqlite database file in the
    // special "local" folder for your platform.
    protected override void OnConfiguring(DbContextOptionsBuilder options)
        => options.UseSqlite($"Data Source={DbPath}");
}