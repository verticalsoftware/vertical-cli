namespace Vertical.Cli.Binding;

/// <summary>
/// Marks an interface type or a handler parameter as a signal to the source generator to
/// emit code necessary for model binding.
/// </summary>
[AttributeUsage(AttributeTargets.Interface | AttributeTargets.Parameter)]
public sealed class GeneratedBindingAttribute : Attribute
{
}