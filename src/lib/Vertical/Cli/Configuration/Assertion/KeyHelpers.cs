namespace Vertical.Cli.Configuration.Assertion;

internal static class KeyHelpers
{
    public const string Services = "Service configuration";

    public const string Conversion = "Conversion services";

    public const string Binding = "Binding configuration";
    
    public static string Create(Command command) => $"Command '{command.Path}'";

    public static string Create(Type modelType) => $"Model type {modelType.FullName}";
}