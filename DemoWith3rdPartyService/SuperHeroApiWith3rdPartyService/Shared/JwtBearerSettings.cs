namespace SuperHeroApiWith3rdPartyService.Shared;

public class JwtBearerSettings
{
    public string Audience { get; set; }
    public string Issuer { get; set; }
    public string SigningKey { get; set; }
}