using System.Text.Json.Serialization;

namespace PetApiTests.Models;

public class Category
{
    [JsonPropertyName("id")]
    public long Id { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;
}

public class Tag
{
    [JsonPropertyName("id")]
    public long Id { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;
}

public class Pet
{
    [JsonPropertyName("id")]
    public long Id { get; set; }

    [JsonPropertyName("category")]
    public Category Category { get; set; } = new();

    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("photoUrls")]
    public string[] PhotoUrls { get; set; } = Array.Empty<string>();

    [JsonPropertyName("tags")]
    public Tag[] Tags { get; set; } = Array.Empty<Tag>();

    [JsonPropertyName("status")]
    public string Status { get; set; } = "available";
}

public class ErrorResponse
{
    [JsonPropertyName("code")]
    public int Code { get; set; }

    [JsonPropertyName("type")]
    public string? Type { get; set; }

    [JsonPropertyName("message")]
    public string? Message { get; set; }
}
