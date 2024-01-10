using CommunityToolkit.Diagnostics;

namespace Vertical.Cli.Binding;

/// <summary>
/// Associates a binding id to the parameter, property, or field of a parameter model.
/// </summary>
[AttributeUsage(AttributeTargets.Parameter | AttributeTargets.Property | AttributeTargets.Field)]
public sealed class BindToAttribute : Attribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="BindToAttribute"/> class.
    /// </summary>
    /// <param name="bindingId">The binding id.</param>
    public BindToAttribute(string bindingId)
    {
        Guard.IsNotNullOrWhiteSpace(bindingId);
        
        BindingId = bindingId;
    }

    /// <summary>
    /// Gets the binding id.
    /// </summary>
    public string BindingId { get; }
}