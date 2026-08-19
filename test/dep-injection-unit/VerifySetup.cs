using System.Runtime.CompilerServices;

namespace Vertical.Cli.DependencyInjection.UnitTests;

public static class VerifySetup
{
    [ModuleInitializer]
    public static void Initialize()
    {
        UseProjectRelativeDirectory("Snapshots");
    }
}