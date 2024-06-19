namespace Vertical.Cli.Binding;

/// <summary>
/// Instructs the source generator to not create binding code.
/// </summary>
[AttributeUsage(AttributeTargets.Class)]
public class NoGeneratorBindingAttribute : Attribute
{
}