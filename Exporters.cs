using System.Globalization;
using System.Text;
using System.Text.Json;

namespace mmr_gui;

public static class Exporters
{
    private static string Sec(long ms) => (ms / 1000.0).ToString("0.###", CultureInfo.InvariantCulture);

    public static void WriteJunit(string path, RunSummary summary, string binDir)
    {
        var sb = new StringBuilder();
        sb.AppendLine("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
        sb.AppendLine($"<testsuites name=\"WMMR 32-bit DLL Suite\" tests=\"{summary.Total}\" failures=\"{summary.Failed}\" errors=\"0\" time=\"{Sec(summary.DurationMs)}\">");

        foreach (var group in summary.Results.GroupBy(r => r.Group).OrderBy(g => g.Key, StringComparer.Ordinal))
        {
            var grp = group.ToList();
            int fails = grp.Count(r => !r.Pass);
            sb.AppendLine($"  <testsuite name=\"{Escape(group.Key)}\" tests=\"{grp.Count}\" failures=\"{fails}\" errors=\"0\" time=\"{Sec(grp.Sum(r => r.DurationMs))}\">");
            foreach (var r in grp)
            {
                sb.Append($"    <testcase name=\"{Escape(r.Name)}\" classname=\"{Escape(r.Dll)}\" time=\"{Sec(r.DurationMs)}\"");
                if (r.Pass)
                {
                    sb.AppendLine("/>");
                }
                else
                {
                    sb.AppendLine(">");
                    sb.AppendLine($"      <failure message=\"{Escape(r.Detail)}\">{(r.Error is null ? "" : Escape(r.Error))}</failure>");
                    sb.AppendLine("    </testcase>");
                }
            }
            sb.AppendLine("  </testsuite>");
        }
        sb.AppendLine("</testsuites>");
        File.WriteAllText(path, sb.ToString(), new UTF8Encoding(false));
    }

    public static void WriteJson(string path, RunSummary summary, string binDir)
    {
        var doc = new
        {
            generatedUtc = DateTime.UtcNow.ToString("O"),
            binDir,
            suite = new { total = summary.Total, passed = summary.Passed, failed = summary.Failed, durationMs = summary.DurationMs },
            results = summary.Results.Select(r => new
            {
                name = r.Name,
                dll = r.Dll,
                group = r.Group,
                pass = r.Pass,
                detail = r.Detail,
                error = r.Error,
                durationMs = r.DurationMs,
            }),
        };
        var json = JsonSerializer.Serialize(doc, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(path, json, new UTF8Encoding(false));
    }

    private static string Escape(string s) =>
        s.Replace("&", "&amp;").Replace("<", "&lt;").Replace(">", "&gt;")
         .Replace("\"", "&quot;").Replace("'", "&apos;");
}
