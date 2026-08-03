using System.Text;

namespace mmr_gui;

public partial class Form1 : Form
{
    public Form1()
    {
        InitializeComponent();
    }

    private void LogLine(string line) => lstLog.Items.Add(line);

    private void RunSuite(string label)
    {
        lstLog.Items.Clear();
        LogLine($"--- {label} ---");
        var sw = new StringWriter();
        bool ok = TestRunner.RunAllTests(txtBinDir.Text.Trim(), sw);
        foreach (var line in sw.ToString().Split(new[] { Environment.NewLine }, StringSplitOptions.None))
        {
            if (!string.IsNullOrEmpty(line))
                lstLog.Items.Add(line);
        }
        LogLine(ok ? "RESULT: ALL PASS" : "RESULT: FAILURES PRESENT");
    }

    private void BtnRunAll_Click(object? sender, EventArgs e) => RunSuite("Run All");

    private void BtnWlidcli_Click(object? sender, EventArgs e) => RunSuite("wlidcli.dll");

    private void BtnUxctl_Click(object? sender, EventArgs e) => RunSuite("uxctl.dll");

    private void BtnVideo_Click(object? sender, EventArgs e) => RunSuite("WLXVideoTrim.dll");

    private void BtnPipe_Click(object? sender, EventArgs e) => RunSuite("WLXPipetran.dll");

    private void BtnMM_Click(object? sender, EventArgs e) => RunSuite("MovieMakerCore.dll");
}
