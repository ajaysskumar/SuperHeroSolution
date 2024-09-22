using SuperHeroMoviesApi.Data;
using SuperHeroMoviesApi.Data.Repos;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddTransient<ISuperHeroMovieRepository, SuperHeroMovieMovieRepository>();
builder.Services.AddEntityFrameworkSqlite().AddDbContext<SuperHeroMovieDbContext>();

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

namespace SuperHeroMoviesApi
{
    public partial class Program { }
}