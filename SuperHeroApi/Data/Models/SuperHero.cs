namespace SuperHeroApi.Data.Models;

public class SuperHero
{
    public SuperHero()
    {
        // for EF
    }
    public SuperHero(int id, string superName, string realName, string powers, string city, int ageInYears)
    {
        Id = id;
        SuperName = superName;
        RealName = realName;
        Powers = powers;
        City = city;
        AgeInYears = ageInYears;
    }

    public int Id { get; set; }
    public string SuperName { get; set; }
    public string RealName { get; set; }
    public string Powers { get; set; }
    public string City { get; set; }
    public int AgeInYears { get; set; }
}