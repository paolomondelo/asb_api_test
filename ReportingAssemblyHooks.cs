using NUnit.Framework;
using PetApiTests.Reporting;

namespace PetApiTests;

[SetUpFixture]
public sealed class ReportingAssemblyHooks
{
    [OneTimeSetUp]
    public void BeforeAllTests()
    {
        TestReport.Reset();
    }

    [OneTimeTearDown]
    public void AfterAllTests()
    {
        TestReport.GenerateHtmlReport();
    }
}

