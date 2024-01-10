using System.Reflection;

namespace Vertical.Cli.Configuration.Metadata;

internal record BindingParameter(string BindingId, string MetadataName, Type Type, object ClrTarget)
{
    /// <inheritdoc />
    public override string ToString()
    {
        return ClrTarget switch
        {
            PropertyInfo property => $"property {property.Name} ({property.PropertyType})",
            FieldInfo field => $"field {field.Name} ({field.FieldType})",
            ParameterInfo parameter => $".ctor parameter {parameter.Name} ({parameter.ParameterType})",
            _ => ClrTarget.ToString() ?? string.Empty
        };
    }
}