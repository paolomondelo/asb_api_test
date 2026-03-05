using PetApiTests.Reporting;
using TechTalk.SpecFlow;

namespace PetApiTests.Hooks;

[Binding]
public sealed class ReportingHooks
{
    private readonly ScenarioContext _scenarioContext;

    public ReportingHooks(ScenarioContext scenarioContext)
    {
        _scenarioContext = scenarioContext;
    }

    [BeforeScenario]
    public void BeforeScenario()
    {
        var title = _scenarioContext.ScenarioInfo.Title;
        var tags = _scenarioContext.ScenarioInfo.Tags ?? Array.Empty<string>();

        TestReport.StartScenario(title, tags);
    }

    [AfterStep]
    public void AfterStep()
    {
        var scenarioTitle = _scenarioContext.ScenarioInfo.Title;
        var stepInfo = _scenarioContext.StepContext.StepInfo;

        var keyword = stepInfo.StepDefinitionType.ToString();
        var text = stepInfo.Text;
        var status = _scenarioContext.TestError is null ? "Passed" : "Failed";
        var error = _scenarioContext.TestError?.ToString();

        TestReport.AddStep(scenarioTitle, keyword, text, status, error);
    }

    [AfterScenario]
    public void AfterScenario()
    {
        var scenarioTitle = _scenarioContext.ScenarioInfo.Title;
        var status = _scenarioContext.TestError is null ? "Passed" : "Failed";

        TestReport.EndScenario(scenarioTitle, status);
    }
}