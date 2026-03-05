using System.Net.Http.Json;
using Microsoft.Extensions.Configuration;
using PetApiTests.Models;

namespace PetApiTests.ApiClients;

public class StoreApiPage : IDisposable
{
    private readonly HttpClient _httpClient;

    public StoreApiPage()
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(AppContext.BaseDirectory)
            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: false)
            .Build();

        var baseUrl = configuration["PetApi:BaseUrl"] ?? "https://petstore.swagger.io/v2/";
        var timeoutSecondsValue = configuration["PetApi:TimeoutSeconds"];
        if (!int.TryParse(timeoutSecondsValue, out var timeoutSeconds))
            timeoutSeconds = 30;

        _httpClient = new HttpClient
        {
            BaseAddress = new Uri(baseUrl),
            Timeout = TimeSpan.FromSeconds(timeoutSeconds)
        };
    }

    /// <summary>
    /// Sends a GET request to the given path (e.g. "store/inventory" or "store/inventory").
    /// Path should not have a leading slash.
    /// </summary>
    public async Task<HttpResponseMessage> GetAsync(string path)
    {
        var relativePath = path.TrimStart('/');
        return await _httpClient.GetAsync(relativePath);
    }

    public async Task<HttpResponseMessage> CreateOrderAsync(StoreOrder order)
    {
        return await _httpClient.PostAsJsonAsync("store/order", order);
    }

    public void Dispose()
    {
        _httpClient.Dispose();
    }
}
