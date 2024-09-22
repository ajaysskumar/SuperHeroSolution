namespace SuperHeroMoviesApi.Data.Models;

public class SuperHeroMovie
{
    public SuperHeroMovie()
    {
        // For EF Core
    }
    public SuperHeroMovie(string name, List<string> superHeroIds)
    {
        Id = Guid.NewGuid();
        Name = name;
        SuperHeroIds = superHeroIds;
    }

    public Guid Id { get; set; }
    
    public string Name { get; set; }
    public List<string> SuperHeroIds { get; set; }
}