using System.Net.Http;
using System.Text.Json;
using NUnit.Framework;
using PetApiTests.ApiClients;
using PetApiTests.Models;
using TechTalk.SpecFlow;

namespace PetApiTests.StepDefinitions;

[Binding, Scope(Tag = "store")]
public class StoreApiSteps : IDisposable
{
    private readonly StoreApiPage _storeApiPage = new();
    private HttpResponseMessage? _lastResponse;
    private long _currentOrderId;
    private long _currentPetId;
    private int _currentQuantity;
    private string _currentStatus = string.Empty;
    private bool _currentComplete;

     [Given("a new order with id (.*) ,pet id (.*),quantity (.*),status \"(.*)\",and complete (.*)")]
     public void GivenANewOrderWithIdPetIdQuantityStatusAndComplete(long id,long petId,int quantity,string status,bool complete)
     {
        _currentOrderId = id;
        _currentPetId = petId;
        _currentQuantity = quantity;
        _currentStatus = status;
        _currentComplete = complete;
    }
    [When(@"I send a GET request to \/store\/inventory")]
    public async Task WhenISendAGetRequestToStoreInventory()
    {
        _lastResponse = await _storeApiPage.GetAsync("store/inventory");
    }
    
    [When("I send a POST request to /store/order")]
    public async Task WhenISendAPostRequestToStoreOrder()
    {
        var order = new StoreOrder
        {
            Id = _currentOrderId,
            PetId = _currentPetId,
            Quantity = _currentQuantity,
            Status = _currentStatus,
            Complete = _currentComplete
        };

        _lastResponse = await _storeApiPage.CreateOrderAsync(order);
    }
    
    [Then("the response should contain order id (.*)")]
    public async Task ThenTheResponseShouldContainOrderId(long expectedId)
    {

        var body = await _lastResponse.Content.ReadAsStringAsync();
        Assert.That(body, Is.Not.Null.And.Not.Empty, "Response body was empty.");

        var jsonOptions = new JsonSerializerOptions(JsonSerializerDefaults.Web);
        var order = JsonSerializer.Deserialize<StoreOrder>(body, jsonOptions);

      
        Assert.That(order!.Id, Is.EqualTo(expectedId),
            "Order id in response did not match the expected id.");
    }
    
        [Then("the response should contain pet id (.*)")]
    public async Task ThenTheResponseShouldContainPetId(long expectedPetId)
    {

        var body = await _lastResponse.Content.ReadAsStringAsync();
       Assert.That(body, Is.Not.Null.And.Not.Empty, "Response body was empty.");

        var jsonOptions = new JsonSerializerOptions(JsonSerializerDefaults.Web);
        var order = JsonSerializer.Deserialize<StoreOrder>(body, jsonOptions);

    
        Assert.That(order!.PetId, Is.EqualTo(expectedPetId),
            "PetId in response did not match the expected PetId.");
    }

            [Then("the response should contain quantity (.*)")]
    public async Task ThenTheResponseShouldContainQuantity(int expectedQuantity)
    {

        var body = await _lastResponse.Content.ReadAsStringAsync();
       Assert.That(body, Is.Not.Null.And.Not.Empty, "Response body was empty.");

        var jsonOptions = new JsonSerializerOptions(JsonSerializerDefaults.Web);
        var order = JsonSerializer.Deserialize<StoreOrder>(body, jsonOptions);

    
        Assert.That(order!.Quantity, Is.EqualTo(expectedQuantity),
            "Quantity in response did not match the expected Quantity.");
    }

            [Then("the response should contain status \"(.*)\"")]
    public async Task ThenTheResponseShouldContainStatus(string expectedStatus)
    {

        var body = await _lastResponse.Content.ReadAsStringAsync();
       Assert.That(body, Is.Not.Null.And.Not.Empty, "Response body was empty.");

        var jsonOptions = new JsonSerializerOptions(JsonSerializerDefaults.Web);
        var order = JsonSerializer.Deserialize<StoreOrder>(body, jsonOptions);

    
        Assert.That(order!.Status, Is.EqualTo(expectedStatus),
            "Status in response did not match the expected Status.");
    }

           [Then("the response should contain complete (.*)")]
    public async Task ThenTheResponseShouldContainComplete(bool expectedComplete)
    {

        var body = await _lastResponse.Content.ReadAsStringAsync();
       Assert.That(body, Is.Not.Null.And.Not.Empty, "Response body was empty.");

        var jsonOptions = new JsonSerializerOptions(JsonSerializerDefaults.Web);
        var order = JsonSerializer.Deserialize<StoreOrder>(body, jsonOptions);

    
        Assert.That(order!.Complete, Is.EqualTo(expectedComplete),
            "Complete in response did not match the expected Complete.");
    }



       [Then(@"the response status code should be (.*)")]
    public void ThenTheResponseStatusCodeShouldBe(int expectedStatusCode)
    {
        Assert.That(_lastResponse, Is.Not.Null, "No response was captured.");
        var statusCode = (int)_lastResponse!.StatusCode;
        Assert.That(statusCode, Is.EqualTo(expectedStatusCode),
            $"Expected HTTP {expectedStatusCode} but got {(int)_lastResponse.StatusCode} ({_lastResponse.StatusCode}).");
    }

    [Then(@"the response should contain inventory data")]
    public async Task ThenTheResponseShouldContainInventoryData()
    {
        Assert.That(_lastResponse, Is.Not.Null, "No response was captured.");
        Assert.That(_lastResponse!.Content, Is.Not.Null);
        var body = await _lastResponse.Content.ReadAsStringAsync();
        Assert.That(body, Is.Not.Null.And.Not.Empty, "Response body was empty.");
        // Inventory is a JSON object (e.g. {"available": 1, "pending": 0})
        Assert.That(body, Does.StartWith("{").And.EndWith("}"), "Response should be a JSON object.");
    }

    public async Task<string> GetLastResponseDetailsAsync()
    {
        if (_lastResponse is null)
        {
            return "No HTTP response was captured.";
        }

        var method = _lastResponse.RequestMessage?.Method.Method ?? "<unknown>";
        var uri = _lastResponse.RequestMessage?.RequestUri;
        var endpoint = uri is null
            ? "<unknown>"
            : (string.IsNullOrEmpty(uri.PathAndQuery) ? uri.ToString() : uri.PathAndQuery);

        var statusCode = (int)_lastResponse.StatusCode;
        var reasonPhrase = _lastResponse.ReasonPhrase ?? string.Empty;

        var headers = string.Join(
            "\n",
            _lastResponse.Headers.Select(h => $"{h.Key}: {string.Join(", ", h.Value)}"));

        var contentHeaders = _lastResponse.Content?.Headers is null
            ? string.Empty
            : string.Join(
                "\n",
                _lastResponse.Content.Headers.Select(h => $"{h.Key}: {string.Join(", ", h.Value)}"));

        var body = _lastResponse.Content is null
            ? "<no content>"
            : await _lastResponse.Content.ReadAsStringAsync();

        return
            $"Request: {method} {endpoint}\n" +
            $"Status: {statusCode} ({reasonPhrase})\n" +
            $"Headers:\n{headers}\n" +
            (string.IsNullOrWhiteSpace(contentHeaders) ? string.Empty : $"Content headers:\n{contentHeaders}\n") +
            "\nBody:\n" +
            body;
    }

    public void Dispose()
    {
        _storeApiPage.Dispose();
    }
}
