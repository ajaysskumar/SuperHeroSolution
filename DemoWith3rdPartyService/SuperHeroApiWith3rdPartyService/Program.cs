using System.Text;
using Amazon.SimpleNotificationService;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Logging;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using SuperHeroApiWith3rdPartyService.Data;
using SuperHeroApiWith3rdPartyService.Data.Repos;
using SuperHeroApiWith3rdPartyService.Shared;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

var configuration = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json")
    .Build();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddAuthentication(options =>
    {
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
    {
        IdentityModelEventSource.ShowPII = true;
        options.SaveToken = true;
        options.IncludeErrorDetails = true;
        var authSettings = builder.Configuration.GetSection("JwtBearerSettings")
            .Get<JwtBearerSettings>();

        options.TokenValidationParameters = new TokenValidationParameters()
        {
            ValidAudience = authSettings!.Audience,
            ValidIssuer = authSettings.Issuer,
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(authSettings.SigningKey))
        };
    });

var snsClient =
    new AmazonSimpleNotificationServiceClient(
        Amazon.RegionEndpoint.GetBySystemName(configuration.GetValue<string>("AWS:Region")));

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddTransient<ISuperHeroRepository, SuperHeroRepository>();
builder.Services.AddSingleton<IAmazonSimpleNotificationService>(_ => snsClient);
builder.Services.AddDbContext<SuperHeroDbContext>(opt =>
    opt.UseNpgsql(configuration.GetConnectionString("WebApiDatabase")));

builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "API",
        Version = "v2",
        Description = "Your Api Description"
    });
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme (Example: 'Bearer 12345abcdef')",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

await app.RunAsync();

namespace SuperHeroApiWith3rdPartyService
{
    public partial class Program
    {
    }
}