using System.Globalization;
using System.Text;
using NUnit.Framework;

namespace PetApiTests.Reporting;

public static class TestReport
{
    private sealed class StepResult
    {
        public required string Keyword { get; init; }
        public required string Text { get; init; }
        public required string Status { get; set; }
        public string? ErrorMessage { get; set; }
    }

    private sealed class ScenarioResult
    {
        public required string Title { get; init; }
        public required IReadOnlyCollection<string> Tags { get; init; }
        public required List<StepResult> Steps { get; init; }
        public required DateTimeOffset StartTime { get; init; }
        public DateTimeOffset? EndTime { get; set; }
        public string Status { get; set; } = "Passed";
    }

    private static readonly object SyncRoot = new();
    private static readonly Dictionary<string, ScenarioResult> Scenarios = new();

    public static void Reset()
    {
        lock (SyncRoot)
        {
            Scenarios.Clear();
        }
    }

    public static void StartScenario(string title, IEnumerable<string> tags)
    {
        if (string.IsNullOrWhiteSpace(title))
        {
            return;
        }

        lock (SyncRoot)
        {
            if (!Scenarios.ContainsKey(title))
            {
                Scenarios[title] = new ScenarioResult
                {
                    Title = title,
                    Tags = tags.ToArray(),
                    Steps = new List<StepResult>(),
                    StartTime = DateTimeOffset.Now
                };
            }
        }
    }

    public static void AddStep(
        string scenarioTitle,
        string keyword,
        string text,
        string status,
        string? errorMessage)
    {
        if (string.IsNullOrWhiteSpace(scenarioTitle))
        {
            return;
        }

        lock (SyncRoot)
        {
            if (!Scenarios.TryGetValue(scenarioTitle, out var scenario))
            {
                scenario = new ScenarioResult
                {
                    Title = scenarioTitle,
                    Tags = Array.Empty<string>(),
                    Steps = new List<StepResult>(),
                    StartTime = DateTimeOffset.Now
                };
                Scenarios[scenarioTitle] = scenario;
            }

            scenario.Steps.Add(new StepResult
            {
                Keyword = keyword,
                Text = text,
                Status = status,
                ErrorMessage = errorMessage
            });

            if (string.Equals(status, "Failed", StringComparison.OrdinalIgnoreCase))
            {
                scenario.Status = "Failed";
            }
        }
    }

    public static void EndScenario(string title, string status)
    {
        if (string.IsNullOrWhiteSpace(title))
        {
            return;
        }

        lock (SyncRoot)
        {
            if (!Scenarios.TryGetValue(title, out var scenario))
            {
                return;
            }

            scenario.Status = status;
            scenario.EndTime ??= DateTimeOffset.Now;
        }
    }

    public static void GenerateHtmlReport()
    {
        ScenarioResult[] snapshot;
        lock (SyncRoot)
        {
            snapshot = Scenarios.Values.OrderBy(s => s.Title, StringComparer.OrdinalIgnoreCase).ToArray();
        }

        var sb = new StringBuilder();
        sb.AppendLine("<!DOCTYPE html>");
        sb.AppendLine("<html lang=\"en\">");
        sb.AppendLine("<head>");
        sb.AppendLine("  <meta charset=\"utf-8\" />");
        sb.AppendLine("  <title>Pet API Test Report</title>");
        sb.AppendLine("  <style>");
        sb.AppendLine("    body { font-family: -apple-system, BlinkMacSystemFont, 'Segoe UI', sans-serif; margin: 0; padding: 0; background-color: #0f172a; color: #e5e7eb; }");
        sb.AppendLine("    header { background: linear-gradient(to right, #6366f1, #14b8a6); padding: 16px 32px; color: white; }");
        sb.AppendLine("    header h1 { margin: 0; font-size: 1.6rem; }");
        sb.AppendLine("    header p { margin: 4px 0 0; font-size: 0.9rem; opacity: 0.9; }");
        sb.AppendLine("    main { padding: 24px 32px 40px; }");
        sb.AppendLine("    .summary { display: flex; gap: 16px; margin-bottom: 24px; flex-wrap: wrap; }");
        sb.AppendLine("    .card { padding: 12px 16px; border-radius: 8px; background-color: #020617; border: 1px solid #1e293b; min-width: 140px; }");
        sb.AppendLine("    .card-title { font-size: 0.8rem; text-transform: uppercase; letter-spacing: .08em; color: #9ca3af; margin-bottom: 4px; }");
        sb.AppendLine("    .card-value { font-size: 1.2rem; font-weight: 600; }");
        sb.AppendLine("    .scenario { border-radius: 10px; border: 1px solid #1f2937; margin-bottom: 20px; background: radial-gradient(circle at top left, rgba(59,130,246,0.15), transparent 55%), #020617; }");
        sb.AppendLine("    .scenario-header { padding: 12px 16px; display: flex; justify-content: space-between; align-items: baseline; gap: 12px; border-bottom: 1px solid #111827; }");
        sb.AppendLine("    .scenario-title { font-size: 1rem; font-weight: 600; }");
        sb.AppendLine("    .scenario-meta { display: flex; gap: 10px; align-items: center; font-size: 0.8rem; color: #9ca3af; }");
        sb.AppendLine("    .tag { padding: 2px 8px; border-radius: 999px; background-color: #1e293b; font-size: 0.7rem; text-transform: uppercase; letter-spacing: .08em; }");
        sb.AppendLine("    .status-pill { padding: 4px 10px; border-radius: 999px; font-size: 0.75rem; font-weight: 600; text-transform: uppercase; letter-spacing: .09em; }");
        sb.AppendLine("    .status-passed { background-color: rgba(34,197,94,0.12); color: #4ade80; border: 1px solid rgba(34,197,94,0.45); }");
        sb.AppendLine("    .status-failed { background-color: rgba(248,113,113,0.12); color: #fca5a5; border: 1px solid rgba(248,113,113,0.45); }");
        sb.AppendLine("    table { width: 100%; border-collapse: collapse; font-size: 0.85rem; }");
        sb.AppendLine("    th, td { padding: 8px 10px; text-align: left; }");
        sb.AppendLine("    thead { background-color: #020617; border-bottom: 1px solid #111827; }");
        sb.AppendLine("    tbody tr:nth-child(even) { background-color: rgba(15,23,42,0.9); }");
        sb.AppendLine("    tbody tr:nth-child(odd) { background-color: rgba(15,23,42,0.7); }");
        sb.AppendLine("    .step-status { font-weight: 600; }");
        sb.AppendLine("    .step-status-passed { color: #22c55e; }");
        sb.AppendLine("    .step-status-failed { color: #f97373; }");
        sb.AppendLine("    .error { color: #fca5a5; font-size: 0.78rem; margin-top: 4px; white-space: pre-wrap; }");
        sb.AppendLine("    footer { padding: 12px 32px 24px; font-size: 0.75rem; color: #6b7280; border-top: 1px solid #111827; background-color: #020617; }");
        sb.AppendLine("    @media (max-width: 640px) { header, main, footer { padding-left: 16px; padding-right: 16px; } .scenario-header { flex-direction: column; align-items: flex-start; } }");
        sb.AppendLine("  </style>");
        sb.AppendLine("</head>");
        sb.AppendLine("<body>");

        var total = snapshot.Length;
        var passed = snapshot.Count(s => string.Equals(s.Status, "Passed", StringComparison.OrdinalIgnoreCase));
        var failed = total - passed;
        var generatedAt = DateTimeOffset.Now.ToString("u", CultureInfo.InvariantCulture);

        sb.AppendLine("<header>");
        sb.AppendLine("  <h1>Pet API Test Report</h1>");
        sb.AppendLine($"  <p>Generated at {Escape(generatedAt)} &middot; Total scenarios: {total}</p>");
        sb.AppendLine("</header>");

        sb.AppendLine("<main>");
        sb.AppendLine("  <section class=\"summary\">");
        sb.AppendLine("    <div class=\"card\">");
        sb.AppendLine("      <div class=\"card-title\">Total scenarios</div>");
        sb.AppendLine($"      <div class=\"card-value\">{total}</div>");
        sb.AppendLine("    </div>");
        sb.AppendLine("    <div class=\"card\">");
        sb.AppendLine("      <div class=\"card-title\">Passed</div>");
        sb.AppendLine($"      <div class=\"card-value\" style=\"color:#4ade80;\">{passed}</div>");
        sb.AppendLine("    </div>");
        sb.AppendLine("    <div class=\"card\">");
        sb.AppendLine("      <div class=\"card-title\">Failed</div>");
        sb.AppendLine($"      <div class=\"card-value\" style=\"color:#fca5a5;\">{failed}</div>");
        sb.AppendLine("    </div>");
        sb.AppendLine("  </section>");

        foreach (var scenario in snapshot)
        {
            var statusClass = string.Equals(scenario.Status, "Passed", StringComparison.OrdinalIgnoreCase)
                ? "status-passed"
                : "status-failed";

            var statusText = scenario.Status.ToUpperInvariant();
            var durationText = GetDurationText(scenario);

            sb.AppendLine("  <section class=\"scenario\">");
            sb.AppendLine("    <div class=\"scenario-header\">");
            sb.AppendLine($"      <div class=\"scenario-title\">{Escape(scenario.Title)}</div>");
            sb.AppendLine("      <div class=\"scenario-meta\">");

            if (scenario.Tags.Count > 0)
            {
                foreach (var tag in scenario.Tags.OrderBy(t => t, StringComparer.OrdinalIgnoreCase))
                {
                    sb.AppendLine($"        <span class=\"tag\">{Escape(tag)}</span>");
                }
            }

            if (!string.IsNullOrEmpty(durationText))
            {
                sb.AppendLine($"        <span>{Escape(durationText)}</span>");
            }

            sb.AppendLine($"        <span class=\"status-pill {statusClass}\">{Escape(statusText)}</span>");
            sb.AppendLine("      </div>");
            sb.AppendLine("    </div>");

            if (scenario.Steps.Count > 0)
            {
                sb.AppendLine("    <div class=\"scenario-body\">");
                sb.AppendLine("      <table>");
                sb.AppendLine("        <thead>");
                sb.AppendLine("          <tr>");
                sb.AppendLine("            <th style=\"width:80px;\">Step</th>");
                sb.AppendLine("            <th>Description</th>");
                sb.AppendLine("            <th style=\"width:90px;\">Status</th>");
                sb.AppendLine("          </tr>");
                sb.AppendLine("        </thead>");
                sb.AppendLine("        <tbody>");

                foreach (var step in scenario.Steps)
                {
                    var stepStatusClass = string.Equals(step.Status, "Passed", StringComparison.OrdinalIgnoreCase)
                        ? "step-status-passed"
                        : "step-status-failed";

                    sb.AppendLine("          <tr>");
                    sb.AppendLine($"            <td>{Escape(step.Keyword)}</td>");
                    sb.AppendLine("            <td>");
                    sb.AppendLine($"              {Escape(step.Text)}");

                    if (!string.IsNullOrWhiteSpace(step.ErrorMessage))
                    {
                        sb.AppendLine($"              <div class=\"error\">{Escape(step.ErrorMessage)}</div>");
                    }

                    sb.AppendLine("            </td>");
                    sb.AppendLine($"            <td><span class=\"step-status {stepStatusClass}\">{Escape(step.Status)}</span></td>");
                    sb.AppendLine("          </tr>");
                }

                sb.AppendLine("        </tbody>");
                sb.AppendLine("      </table>");
                sb.AppendLine("    </div>");
            }

            sb.AppendLine("  </section>");
        }

        sb.AppendLine("</main>");

        sb.AppendLine("<footer>");
        sb.AppendLine("  <span>Pet API test suite &middot; HTML report generated by a custom Reqnroll/NUnit reporter.</span>");
        sb.AppendLine("</footer>");

        sb.AppendLine("</body>");
        sb.AppendLine("</html>");

        var directory = TestContext.CurrentContext.WorkDirectory;
        var filePath = Path.Combine(directory, "PetApiTestReport.html");

        File.WriteAllText(filePath, sb.ToString(), Encoding.UTF8);
        TestContext.Progress.WriteLine($"HTML report generated at: {filePath}");
    }

    private static string GetDurationText(ScenarioResult scenario)
    {
        if (scenario.EndTime is null)
        {
            return string.Empty;
        }

        var duration = scenario.EndTime.Value - scenario.StartTime;

        if (duration.TotalSeconds < 1)
        {
            return $"{duration.TotalMilliseconds:F0} ms";
        }

        if (duration.TotalMinutes < 1)
        {
            return $"{duration.TotalSeconds:F1} s";
        }

        return $"{(int)duration.TotalMinutes} min {duration.Seconds:D2} s";
    }

    private static string Escape(string value)
    {
        return System.Net.WebUtility.HtmlEncode(value);
    }
}

