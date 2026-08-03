using System.Runtime.InteropServices;

namespace mmr_gui;

static class Program
{
    /// <summary>
    ///  The main entry point for the application.
    /// </summary>
    [STAThread]
    static int Main(string[] args)
    {
        if (args.Length >= 2 && args[0] == "--selftest")
        {
            ApplicationConfiguration.Initialize();
            return TestRunner.RunAllTests(args[1], Console.Out) ? 0 : 1;
        }
        ApplicationConfiguration.Initialize();
        Application.Run(new Form1());
        return 0;
    }
}
