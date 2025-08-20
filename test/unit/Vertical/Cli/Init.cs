using System.Runtime.CompilerServices;
using DiffEngine;

namespace Vertical.Cli;

public static class Init
{
    [ModuleInitializer]
    internal static void Configure()
    {
        DiffRunner.Disabled = true;
    }
}