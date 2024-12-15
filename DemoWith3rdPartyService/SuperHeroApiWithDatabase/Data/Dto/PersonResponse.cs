using System.Text.Json.Serialization;

namespace SuperHeroApiWithDatabase.Controllers;

public class PersonResponse
{
    [JsonPropertyName("page")]
    public int Page { get; set; }

    [JsonPropertyName("per_page")]
    public int PerPage { get; set; }

    [JsonPropertyName("total")]
    public int Total { get; set; }

    [JsonPropertyName("total_pages")]
    public int TotalPages { get; set; }

    [JsonPropertyName("data")]
    public List<Suspect> Data { get; set; }

    [JsonPropertyName("support")]
    public Support Support { get; set; }
}

public class Suspect
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("email")]
    public string Email { get; set; }

    [JsonPropertyName("first_name")]
    public string First_Name { get; set; }

    [JsonPropertyName("last_name")]
    public string Last_Name { get; set; }

    [JsonPropertyName("avatar")]
    public string Avatar { get; set; }
}

public class Support
{
    public string Url { get; set; }
    public string Text { get; set; }
}