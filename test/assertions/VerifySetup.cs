using System.Runtime.CompilerServices;

namespace Vertical.Cli.ConfigurationAssertionTests;

public static class VerifySetup
{
    [ModuleInitializer]
    public static void Initialize()
    {
        UseProjectRelativeDirectory("Snapshots");
    }
}