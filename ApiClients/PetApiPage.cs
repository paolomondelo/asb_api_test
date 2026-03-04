using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using PetApiTests.Models;

namespace PetApiTests.ApiClients;

public class PetApiPage : IDisposable
{
    private readonly HttpClient _httpClient;
    private readonly JsonSerializerOptions _jsonOptions = new(JsonSerializerDefaults.Web);

    public PetApiPage()
    {
        _httpClient = new HttpClient
        {
            BaseAddress = new Uri("https://petstore.swagger.io/v2/")
        };
    }

    public async Task<HttpResponseMessage> CreatePetAsync(long id, string name)
    {
        var pet = CreateDefaultPet(id, name);
        return await SendPetAsync(HttpMethod.Post, "pet", pet);
    }

    public async Task<HttpResponseMessage> UpdatePetAsync(long id, string name)
    {
        var pet = CreateDefaultPet(id, name);
        return await SendPetAsync(HttpMethod.Put, "pet", pet);
    }

    public async Task<HttpResponseMessage> GetPetAsync(long id)
    {
        return await _httpClient.GetAsync($"pet/{id}");
    }

    public async Task<HttpResponseMessage> DeletePetAsync(long id)
    {
        var request = new HttpRequestMessage(HttpMethod.Delete, $"pet/{id}");
        request.Headers.Add("api_key", "api_key");
        return await _httpClient.SendAsync(request);
    }

    public async Task<Pet?> ReadPetAsync(HttpResponseMessage response)
    {
        return await response.Content.ReadFromJsonAsync<Pet>(_jsonOptions);
    }

    public async Task<ErrorResponse?> ReadErrorAsync(HttpResponseMessage response)
    {
        return await response.Content.ReadFromJsonAsync<ErrorResponse>(_jsonOptions);
    }

    private async Task<HttpResponseMessage> SendPetAsync(HttpMethod method, string relativeUrl, Pet pet)
    {
        var json = JsonSerializer.Serialize(pet, _jsonOptions);
        using var content = new StringContent(json, Encoding.UTF8, "application/json");
        var request = new HttpRequestMessage(method, relativeUrl)
        {
            Content = content
        };

        return await _httpClient.SendAsync(request);
    }

    private static Pet CreateDefaultPet(long id, string name)
    {
        return new Pet
        {
            Id = id,
            Name = name,
            Category = new Category
            {
                Id = 0,
                Name = "string"
            },
            PhotoUrls = new[] { "string" },
            Tags = new[]
            {
                new Tag
                {
                    Id = 0,
                    Name = "string"
                }
            },
            Status = "available"
        };
    }

    public void Dispose()
    {
        _httpClient.Dispose();
    }
}
