using NUnit.Framework;
using PetApiTests.Reporting;
using Reqnroll;

namespace PetApiTests.Hooks;

[Binding]
public class ReportingHooks
{
    private readonly ScenarioContext _scenarioContext;

    public ReportingHooks(ScenarioContext scenarioContext)
    {
        _scenarioContext = scenarioContext;
    }

    [BeforeTestRun]
    public static void BeforeTestRun()
    {
        TestContext.Progress.WriteLine("Initializing HTML test report.");
        TestReport.Reset();
    }

    [AfterTestRun]
    public static void AfterTestRun()
    {
        TestContext.Progress.WriteLine("Generating HTML test report...");
        TestReport.GenerateHtmlReport();
    }

    [BeforeScenario]
    public void BeforeScenario()
    {
        var info = _scenarioContext.ScenarioInfo;
        TestReport.StartScenario(info.Title, info.Tags);
    }

    [AfterStep]
    public void AfterStep()
    {
        var stepContext = _scenarioContext.StepContext;
        var stepInfo = stepContext.StepInfo;

        var keyword = stepInfo.StepDefinitionType.ToString();
        var text = stepInfo.Text;

        var error = _scenarioContext.TestError;
        var status = error == null ? "Passed" : "Failed";
        var errorMessage = error?.Message;

        TestReport.AddStep(
            _scenarioContext.ScenarioInfo.Title,
            keyword,
            text,
            status,
            errorMessage);
    }

    [AfterScenario]
    public void AfterScenario()
    {
        var status = _scenarioContext.TestError == null ? "Passed" : "Failed";
        TestReport.EndScenario(_scenarioContext.ScenarioInfo.Title, status);
    }
}

