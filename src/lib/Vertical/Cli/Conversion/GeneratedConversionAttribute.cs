namespace Vertical.Cli.Conversion;

/// <summary>
/// Instructs the source generator to include an argument converter for the target
/// parameter's type.
/// </summary>
[AttributeUsage(AttributeTargets.Parameter)]
public sealed class GeneratedConversionAttribute : Attribute
{
}