using System.Configuration;
using Microsoft.EntityFrameworkCore;
using SuperHeroApiWithDatabase.Data;
using SuperHeroApiWithDatabase.Data.Models;
using SuperHeroApiWithDatabase.Data.Repos;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

var configuration = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json")
    .Build();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddTransient<ISuperHeroRepository, SuperHeroRepository>();
// builder.Services.AddEntityFrameworkSqlite().AddDbContext<SuperHeroDbContext>();
builder.Services.AddDbContext<SuperHeroDbContext>(opt =>
    opt.UseNpgsql(configuration.GetConnectionString("WebApiDatabase")));

// Seed database


var app = builder.Build();
using (var serviceScope = app.Services.CreateScope())
using(var client = serviceScope.ServiceProvider.GetService<SuperHeroDbContext>())
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

namespace SuperHeroApiWithDatabase
{
    public partial class Program { }
}