using Microsoft.EntityFrameworkCore;
using SuperHeroApi.Data;
using SuperHeroApi.Data.Models;
using SuperHeroApi.Data.Repos;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddTransient<ISuperHeroRepository, SuperHeroRepository>();
builder.Services.AddEntityFrameworkSqlite().AddDbContext<SuperHeroDbContext>();

// Seed database
using(var client = new SuperHeroDbContext(new DbContextOptions<SuperHeroDbContext>()))
{
    client.Database.EnsureCreated();
    if (!client.SuperHero.Any())
    {
        client.SuperHero.AddRange(new List<SuperHero>()
        {
            new SuperHero(1, "Batman","Bruce Wayne","Short distance fly,Common sense","Gotham", 40),
            new SuperHero(2, "Superman", "Clark kent", "Fly, Shoot laser beam, Super strength, ice breath","Gotham", 42),
            new SuperHero(3, "Robin", "John Blake", "Detective","Gotham", 35)
        });
        client.SaveChanges();
    }
}

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

namespace SuperHeroApi
{
    public partial class Program { }
}