namespace Vertical.Cli.Binding;

/// <summary>
/// Annotates a type or a handler's option parameter for binding generation.
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface | AttributeTargets.Parameter)]
public sealed class GeneratedBindingAttribute : Attribute
{
}