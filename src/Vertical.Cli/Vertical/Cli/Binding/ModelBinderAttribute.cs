namespace Vertical.Cli.Binding;

/// <summary>
/// Instructs the source generator that the application will bind the model properties.
/// </summary>
/// <typeparam name="TBinder">Binder implementation type.</typeparam>
[AttributeUsage(AttributeTargets.Class)]
public class ModelBinderAttribute<TBinder> : Attribute where TBinder : ModelBinder
{
}