using System.Diagnostics;

namespace mmr_gui;

public partial class Form1 : Form
{
    private readonly TestEngine _engine = new(TestSuite.Build());
    private readonly List<TestResult> _allResults = new();
    private CancellationTokenSource? _cts;
    private int _runningTotal;

    public Form1()
    {
        InitializeComponent();
        RefreshDllList();
    }

    private void RefreshDllList()
    {
        lstDlls.BeginUpdate();
        lstDlls.Items.Clear();
        foreach (var g in _engine.Cases.GroupBy(c => c.Dll).OrderBy(g => g.Key, StringComparer.OrdinalIgnoreCase))
        {
            var item = new ListViewItem(new[] { g.Key, g.Count().ToString(), "" }) { Checked = true, Tag = g.Key };
            lstDlls.Items.Add(item);
        }
        lstDlls.EndUpdate();
        lblStatus.Text = $"{_engine.Cases.Count} checks across {lstDlls.Items.Count} DLLs -- ready";
    }

    private HashSet<string> SelectedDlls()
    {
        var set = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        foreach (ListViewItem it in lstDlls.Items)
            if (it.Checked && it.Tag is string dll)
                set.Add(dll);
        return set;
    }

    private async void BtnRunAll_Click(object? sender, EventArgs e) => await RunAsync(null);

    private async void BtnRunSelected_Click(object? sender, EventArgs e)
    {
        var selected = SelectedDlls();
        if (selected.Count == 0)
        {
            MessageBox.Show("No DLLs checked.", "mmr-gui", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }
        await RunAsync(selected);
    }

    private async Task RunAsync(HashSet<string>? dllFilter)
    {
        if (_cts is not null)
            return;

        string binDir = txtBinDir.Text.Trim();
        if (!Directory.Exists(binDir))
        {
            MessageBox.Show($"DLL directory does not exist:\n{binDir}", "mmr-gui", MessageBoxButtons.OK, MessageBoxIcon.Error);
            return;
        }

        _cts = new CancellationTokenSource();
        _allResults.Clear();
        lvResults.Items.Clear();
        _runningTotal = _engine.CountFor(dllFilter);
        prgBar.Maximum = _runningTotal;
        prgBar.Value = 0;
        SetRunning(true);

        var progress = new Progress<TestResult>(r =>
        {
            AddResult(r);
            prgBar.Value = Math.Min(prgBar.Value + 1, prgBar.Maximum);
        });

        var sw = Stopwatch.StartNew();
        RunSummary summary;
        try
        {
            summary = await Task.Run(() => _engine.Run(binDir, _cts.Token, progress, dllFilter));
        }
        catch (OperationCanceledException)
        {
            sw.Stop();
            lblStatus.Text = $"Cancelled after {sw.ElapsedMilliseconds} ms ({_allResults.Count} completed).";
            SetRunning(false);
            _cts.Dispose();
            _cts = null;
            return;
        }
        sw.Stop();

        SetRunning(false);
        _cts.Dispose();
        _cts = null;
        FinishRun(summary, sw.ElapsedMilliseconds);
    }

    private void FinishRun(RunSummary summary, long wallMs)
    {
        ApplyFilterToRows();
        UpdateDllCounts();
        lblStatus.Text = summary.Failed == 0
            ? $"All {summary.Total} checks passed in {summary.DurationMs} ms (wall {wallMs} ms)."
            : $"{summary.Passed}/{summary.Total} passed, {summary.Failed} failed in {summary.DurationMs} ms.";
        btnExportJunit.Enabled = btnExportJson.Enabled = _allResults.Count > 0;
        Text = $"WMMR 32-bit DLL Suite Browser -- {summary.Passed}/{summary.Total} passed";
    }

    private void AddResult(TestResult r)
    {
        _allResults.Add(r);
        var item = new ListViewItem(new[]
        {
            r.Name,
            r.Dll,
            r.Group,
            r.Pass ? "PASS" : "FAIL",
            r.DurationMs.ToString(),
            r.Error is null ? r.Detail : $"{r.Detail} [{r.Error}]",
        });
        item.ForeColor = r.Pass ? Color.ForestGreen : Color.Firebrick;
        item.Tag = r;
        lvResults.Items.Add(item);
    }

    private void ApplyFilterToRows()
    {
        bool onlyFailures = chkFailuresOnly.Checked;
        var items = lvResults.Items.Cast<ListViewItem>().ToList();
        foreach (var it in items)
        {
            var r = it.Tag as TestResult;
            bool visible = r is null || !onlyFailures || !r.Pass;
            it.Remove();
            if (visible)
                lvResults.Items.Add(it);
        }
    }

    private void ChkFailuresOnly_CheckedChanged(object? sender, EventArgs e)
    {
        lvResults.BeginUpdate();
        ApplyFilterToRows();
        lvResults.EndUpdate();
    }

    private void UpdateDllCounts()
    {
        foreach (ListViewItem it in lstDlls.Items)
        {
            string dll = (string)it.Tag!;
            int total = _engine.Cases.Count(c => c.Dll == dll);
            int passed = _allResults.Count(r => r.Dll == dll && r.Pass);
            it.SubItems[1].Text = total.ToString();
            it.SubItems[2].Text = passed.ToString();
        }
        lstDlls.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
    }

    private void SetRunning(bool running)
    {
        btnRunAll.Enabled = !running;
        btnRunSelected.Enabled = !running;
        btnBrowse.Enabled = !running;
        txtBinDir.Enabled = !running;
        lstDlls.Enabled = !running;
        btnCancel.Enabled = running;
        if (running)
            btnCancel.Text = "Cancel";
    }

    private void BtnCancel_Click(object? sender, EventArgs e) => _cts?.Cancel();

    private void BtnBrowse_Click(object? sender, EventArgs e)
    {
        using var dlg = new FolderBrowserDialog
        {
            Description = "Select the directory containing the 32-bit WMMR DLLs",
            SelectedPath = txtBinDir.Text.Trim(),
        };
        if (dlg.ShowDialog(this) == DialogResult.OK)
            txtBinDir.Text = dlg.SelectedPath;
    }

    private void BtnExportJunit_Click(object? sender, EventArgs e)
    {
        if (_allResults.Count == 0)
            return;
        using var dlg = new SaveFileDialog
        {
            FileName = $"wmmr-suite-{DateTime.Now:yyyyMMdd-HHmmss}.xml",
            Filter = "JUnit XML|*.xml",
            Title = "Export JUnit XML",
        };
        if (dlg.ShowDialog(this) != DialogResult.OK)
            return;
        var summary = new RunSummary
        {
            Results = _allResults.ToList(),
            DurationMs = _allResults.Sum(r => r.DurationMs),
        };
        Exporters.WriteJunit(dlg.FileName, summary, txtBinDir.Text.Trim());
        lblStatus.Text = $"JUnit exported: {dlg.FileName}";
    }

    private void BtnExportJson_Click(object? sender, EventArgs e)
    {
        if (_allResults.Count == 0)
            return;
        using var dlg = new SaveFileDialog
        {
            FileName = $"wmmr-suite-{DateTime.Now:yyyyMMdd-HHmmss}.json",
            Filter = "JSON|*.json",
            Title = "Export JSON results",
        };
        if (dlg.ShowDialog(this) != DialogResult.OK)
            return;
        var summary = new RunSummary
        {
            Results = _allResults.ToList(),
            DurationMs = _allResults.Sum(r => r.DurationMs),
        };
        Exporters.WriteJson(dlg.FileName, summary, txtBinDir.Text.Trim());
        lblStatus.Text = $"JSON exported: {dlg.FileName}";
    }

    protected override void OnFormClosing(FormClosingEventArgs e)
    {
        _cts?.Cancel();
        base.OnFormClosing(e);
    }
}
