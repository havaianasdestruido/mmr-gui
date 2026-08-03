namespace mmr_gui;

public sealed record Check(bool Pass, string? Detail = null, string? Error = null)
{
    public static Check Ok(string? detail = null) => new(true, detail);
    public static Check Fail(string detail, string? error = null) => new(false, detail, error);
}

public sealed class TestCase
{
    public required string Name { get; init; }
    public required string Dll { get; init; }
    public required string Group { get; init; }
    public required Func<Check> Body { get; init; }
}

public sealed class TestResult
{
    public required string Name { get; init; }
    public required string Dll { get; init; }
    public required string Group { get; init; }
    public required bool Pass { get; init; }
    public required string Detail { get; init; }
    public required long DurationMs { get; init; }
    public string? Error { get; init; }

    public string Summary => Pass
        ? $"PASS {Name} ({Detail})"
        : $"FAIL {Name} ({Detail}){(Error is null ? "" : $" [{Error}]")}";
}

public sealed class RunSummary
{
    public required IReadOnlyList<TestResult> Results { get; init; }
    public required long DurationMs { get; init; }

    public int Total => Results.Count;
    public int Passed => Results.Count(r => r.Pass);
    public int Failed => Total - Passed;
    public bool AllPassed => Failed == 0 && Total > 0;

    public IEnumerable<string> Lines
    {
        get
        {
            foreach (var r in Results)
                yield return r.Summary;
            yield return $"RESULT: {Passed} passed, {Failed} failed, in {DurationMs} ms";
        }
    }
}

public sealed class TestEngine
{
    private readonly List<TestCase> _cases;

    public TestEngine(IEnumerable<TestCase> cases) => _cases = cases.ToList();

    public IReadOnlyList<TestCase> Cases => _cases;

    public int CountFor(HashSet<string>? dllFilter) =>
        dllFilter is null ? _cases.Count : _cases.Count(c => dllFilter.Contains(c.Dll));

    public RunSummary Run(
        string binDir,
        CancellationToken ct,
        IProgress<TestResult>? progress = null,
        HashSet<string>? dllFilter = null,
        TextWriter? log = null)
    {
        if (!string.IsNullOrWhiteSpace(binDir) && !Native.SetDllDirectoryW(binDir))
            log?.WriteLine($"WARN SetDllDirectoryW failed: {binDir}");

        var results = new List<TestResult>();
        foreach (var c in _cases)
        {
            ct.ThrowIfCancellationRequested();
            if (dllFilter is not null && !dllFilter.Contains(c.Dll))
                continue;

            var sw = System.Diagnostics.Stopwatch.StartNew();
            Check check;
            string? error = null;
            try
            {
                check = c.Body();
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception ex)
            {
                check = Check.Fail($"exception: {ex.Message}", ex.GetType().Name);
                error = $"{ex.GetType().Name}: {ex.Message}";
            }
            sw.Stop();

            var r = new TestResult
            {
                Name = c.Name,
                Dll = c.Dll,
                Group = c.Group,
                Pass = check.Pass,
                Detail = check.Detail ?? (check.Pass ? "ok" : "failed"),
                DurationMs = sw.ElapsedMilliseconds,
                Error = error ?? check.Error,
            };
            results.Add(r);
            progress?.Report(r);
            log?.WriteLine(r.Summary);
        }

        return new RunSummary { Results = results, DurationMs = results.Sum(x => x.DurationMs) };
    }
}
