using Microsoft.EntityFrameworkCore;
using SuperHeroMoviesApi.Data.Models;

namespace SuperHeroMoviesApi.Data;

public class SuperHeroMovieDbContext : DbContext
{
    public DbSet<SuperHeroMovie> Movies { get; set; }

    public string DbPath { get; }

    public SuperHeroMovieDbContext(DbContextOptions<SuperHeroMovieDbContext> options)
        : base(options)
    {
        var folder = Environment.SpecialFolder.LocalApplicationData;
        var path = Environment.GetFolderPath(folder);
        DbPath = System.IO.Path.Join(path, "super_hero_movies.db");
    }

    // The following configures EF to create a Sqlite database file in the
    // special "local" folder for your platform.
    protected override void OnConfiguring(DbContextOptionsBuilder options)
        => options.UseSqlite($"Data Source={DbPath}");
}