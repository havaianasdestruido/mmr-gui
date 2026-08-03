namespace mmr_gui;

static class Program
{
    [STAThread]
    static int Main(string[] args)
    {
        if (args.Length > 0 && args[0] == "--selftest")
            return RunSelfTest(args[1..]);
        if (args.Length > 0 && args[0] == "--selfcheck")
            return RunSelfCheck();

        ApplicationConfiguration.Initialize();
        Application.Run(new Form1());
        return 0;
    }

    static int RunSelfCheck()
    {
        var cases = TestSuite.Build();
        int dlls = cases.Select(c => c.Dll).Distinct().Count();
        Console.WriteLine($"selfcheck: {cases.Count} checks across {dlls} DLLs");
        var problems = new List<string>();
        if (cases.Count != 128)
            problems.Add($"expected 128 checks, got {cases.Count}");
        if (dlls != 21)
            problems.Add($"expected 21 DLLs, got {dlls}");
        var dup = cases.GroupBy(c => c.Name).FirstOrDefault(g => g.Count() > 1);
        if (dup is not null)
            problems.Add($"duplicate case name: {dup.Key}");
        if (cases.Any(c => string.IsNullOrWhiteSpace(c.Name) || string.IsNullOrWhiteSpace(c.Dll) || string.IsNullOrWhiteSpace(c.Group)))
            problems.Add("case with empty name/dll/group");

        var tmp = Path.Combine(Path.GetTempPath(), "wmmr-selfcheck");
        Directory.CreateDirectory(tmp);
        var junit = Path.Combine(tmp, "suite.xml");
        var json = Path.Combine(tmp, "suite.json");
        var results = cases.Select(c => new TestResult { Name = c.Name, Dll = c.Dll, Group = c.Group, Pass = true, Detail = "selfcheck", DurationMs = 1 }).ToList();
        Exporters.WriteJunit(junit, new RunSummary { Results = results, DurationMs = results.Count }, tmp);
        Exporters.WriteJson(json, new RunSummary { Results = results, DurationMs = results.Count }, tmp);
        if (!File.Exists(junit) || !File.ReadAllText(junit).Contains("failures=\"0\""))
            problems.Add("JUnit export failed");
        if (!File.Exists(json) || !File.ReadAllText(json).Contains("\"pass\": true"))
            problems.Add("JSON export failed");

        if (problems.Count == 0)
        {
            Console.WriteLine("selfcheck: OK");
            return 0;
        }
        foreach (var p in problems)
            Console.Error.WriteLine($"selfcheck FAIL: {p}");
        return 1;
    }

    static int RunSelfTest(string[] args)
    {
        string? binDir = null;
        string? junitPath = null;
        string? jsonPath = null;

        for (int i = 0; i < args.Length; i++)
        {
            switch (args[i])
            {
                case "--junit":
                    if (i + 1 < args.Length) junitPath = args[++i];
                    break;
                case "--json":
                    if (i + 1 < args.Length) jsonPath = args[++i];
                    break;
                default:
                    binDir = args[i];
                    break;
            }
        }

        if (string.IsNullOrWhiteSpace(binDir))
        {
            Console.Error.WriteLine("Usage: mmr-gui --selftest <binDir> [--junit <file>] [--json <file>]");
            return 2;
        }

        if (!Directory.Exists(binDir))
        {
            Console.Error.WriteLine($"binDir does not exist: {binDir}");
            return 2;
        }

        ApplicationConfiguration.Initialize();

        var engine = new TestEngine(TestSuite.Build());
        Console.WriteLine($"WMMR 32-bit DLL suite -- {engine.Cases.Count} checks, {engine.Cases.Select(c => c.Dll).Distinct().Count()} DLLs");
        Console.WriteLine($"binDir: {binDir}");

        var sw = System.Diagnostics.Stopwatch.StartNew();
        RunSummary summary;
        try
        {
            summary = engine.Run(binDir, CancellationToken.None, null, null);
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"run failed: {ex.Message}");
            return 1;
        }
        sw.Stop();

        Console.WriteLine();
        foreach (var line in summary.Lines)
            Console.WriteLine(line);
        Console.WriteLine($"total time: {sw.ElapsedMilliseconds} ms");

        if (junitPath is not null)
        {
            Exporters.WriteJunit(junitPath, summary, binDir);
            Console.WriteLine($"junit: {junitPath}");
        }
        if (jsonPath is not null)
        {
            Exporters.WriteJson(jsonPath, summary, binDir);
            Console.WriteLine($"json: {jsonPath}");
        }

        return summary.AllPassed ? 0 : 1;
    }
}
