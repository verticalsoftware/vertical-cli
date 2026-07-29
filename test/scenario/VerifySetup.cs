using System.Runtime.CompilerServices;

namespace Vertical.Cli.ScenarioTests;

public static class VerifySetup
{
    [ModuleInitializer]
    public static void Initialize()
    {
        UseProjectRelativeDirectory("Snapshots");
    }
}