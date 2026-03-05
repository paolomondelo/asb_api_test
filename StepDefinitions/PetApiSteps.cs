using System.Net;
using System.Net.Http;
using System.Text.Json;
using NUnit.Framework;
using PetApiTests.Models;
using PetApiTests.ApiClients;
using TechTalk.SpecFlow;

namespace PetApiTests.StepDefinitions;

[Binding, Scope(Tag = "pet")]
public class PetApiSteps : IDisposable
{
    private readonly PetApiPage _petApiPage = new();
    private HttpResponseMessage? _lastResponse;
    private Pet? _lastPet;
    private ErrorResponse? _lastError;
    private long _currentPetId;
    private string _currentPetName = string.Empty;

    [Given("a new pet with id (.*) and name \"(.*)\"")]
    public void GivenANewPetWithIdAndName(long id, string name)
    {
        _currentPetId = id;
        _currentPetName = name;
    }

    [When("I send a POST request to create the pet")]
    public async Task WhenISendAPostRequestToCreateThePet()
    {
        _lastResponse = await _petApiPage.CreatePetAsync(_currentPetId, _currentPetName);
        await CapturePetOrErrorAsync();
    }

    [When("I send a PUT request to update the pet name to \"(.*)\"")]
    public async Task WhenISendAPutRequestToUpdateThePetNameTo(string newName)
    {
        _currentPetName = newName;
        _lastResponse = await _petApiPage.UpdatePetAsync(_currentPetId, _currentPetName);
        await CapturePetOrErrorAsync();
    }

    [When(@"I send a GET request to retrieve the pet by id (.*)")]
    public async Task WhenISendAGetRequestToRetrieveThePetById(long id)
    {
        _lastResponse = await _petApiPage.GetPetAsync(id);
        await CapturePetOrErrorAsync();
    }

    [When("I send a DELETE request to delete the pet using the same pet id")]
    public async Task WhenISendADeleteRequestToDeleteThePetUsingTheSamePetId()
    {
        _lastResponse = await _petApiPage.DeletePetAsync(_currentPetId);
        await CapturePetOrErrorAsync();
    }

    [When("I send another DELETE request to delete the pet using the same pet id")]
    public async Task WhenISendAnotherDeleteRequestToDeleteThePetUsingTheSamePetId()
    {
        _lastResponse = await _petApiPage.DeletePetAsync(_currentPetId);
        await CapturePetOrErrorAsync();
    }

    [When("I send a DELETE request to delete the pet with id {int}")]
    public async Task WhenISendADeleteRequestToDeleteThePetWithId(int id)
    {
        _lastResponse = await _petApiPage.DeletePetAsync(id);
        await CapturePetOrErrorAsync();
    }

    [Then(@"the response status code should be (.*)")]
    public void ThenTheResponseStatusCodeShouldBe(int expectedStatusCode)
    {
        Assert.That(_lastResponse, Is.Not.Null, "No response was captured.");
        var statusCode = (int)_lastResponse!.StatusCode;
        Assert.That(statusCode, Is.EqualTo(expectedStatusCode),
            $"Expected HTTP {expectedStatusCode} but got {(int)_lastResponse.StatusCode} ({_lastResponse.StatusCode}).");
    }

    [Then("the response should contain pet id (.*) and name \"(.*)\"")]
    public void ThenTheResponseShouldContainPetIdAndName(long expectedId, string expectedName)
    {
        Assert.That(_lastPet, Is.Not.Null, "Pet response body was not available.");
        Assert.Multiple(() =>
        {
            Assert.That(_lastPet!.Id, Is.EqualTo(expectedId), "Pet id did not match.");
            Assert.That(_lastPet!.Name, Is.EqualTo(expectedName), "Pet name did not match.");
        });
    }

    [Then(@"the delete response should contain pet id (.*)")]
    public void ThenTheDeleteResponseShouldContainPetId(long expectedId)
    {
        Assert.That(_lastError, Is.Not.Null, "Delete response body was not available.");
        Assert.That(_lastError!.Message, Is.EqualTo(expectedId.ToString()),
            "Delete response message did not contain the expected id.");
    }

    [Given("the pet is \"(.*)\" before deletion")]
    public async Task GivenThePetIsStateBeforeDeletion(string state)
    {
        if (string.Equals(state, "existing", StringComparison.OrdinalIgnoreCase))
        {
            _lastResponse = await _petApiPage.CreatePetAsync(_currentPetId, _currentPetName);
            await CapturePetOrErrorAsync();
        }
        else if (string.Equals(state, "non-existing", StringComparison.OrdinalIgnoreCase))
        {
            // Do not create the pet, so DELETE will target a non-existing id.
        }
        else
        {
            Assert.Fail($"Unknown pet state '{state}' in step.");
        }
    }

    [Then("the response should contain error message \"(.*)\"")]
    public void ThenTheResponseShouldContainErrorMessage(string expectedMessage)
    {
        Assert.That(_lastError, Is.Not.Null, "Error response body was not available.");
        Assert.That(_lastError!.Message, Is.EqualTo(expectedMessage));
    }

    private async Task CapturePetOrErrorAsync()
    {
        _lastPet = null;
        _lastError = null;

        if (_lastResponse == null || _lastResponse.Content == null)
        {
            return;
        }

        var contentString = await _lastResponse.Content.ReadAsStringAsync();
        if (string.IsNullOrWhiteSpace(contentString))
        {
            return;
        }

        try
        {
            var jsonOptions = new JsonSerializerOptions(JsonSerializerDefaults.Web);

            if (_lastResponse.RequestMessage?.Method == HttpMethod.Delete)
            {
                _lastError = JsonSerializer.Deserialize<ErrorResponse>(contentString, jsonOptions);
            }
            else if (_lastResponse.StatusCode == HttpStatusCode.OK)
            {
                _lastPet = JsonSerializer.Deserialize<Pet>(contentString, jsonOptions);
            }
            else
            {
                _lastError = JsonSerializer.Deserialize<ErrorResponse>(contentString, jsonOptions);
            }
        }
        catch (JsonException)
        {
            // If deserialization fails, keep _lastPet and _lastError as null.
        }
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
        _petApiPage.Dispose();
    }
}
