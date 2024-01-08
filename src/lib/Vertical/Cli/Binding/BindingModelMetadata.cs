using System.Reflection;
using Vertical.Cli.Configuration;

namespace Vertical.Cli.Binding;

internal sealed class BindingModelMetadata
{
    internal BindingModelMetadata(Type type)
    {
        Type = type;
        Constructors = ResolveConstructors(type);
        ConstructorTarget = Constructors.Length == 1 ? Constructors[0] : null;
        BindingParameters = BuildBindingParameters(ConstructorTarget, type);
    }

    public BindingParameter[] BindingParameters { get; }

    public Type Type { get; }

    public ConstructorInfo[] Constructors { get; }
    
    public ConstructorInfo? ConstructorTarget { get; }

    public IEnumerable<BindingParameter> FindParameters(SymbolDefinition symbol)
    {
        return symbol
            .Identities
            .SelectMany(identity => BindingParameters.Where(parameter => BindingIdComparer
                .Default
                .Equals(parameter.BindingId, identity)));
    }

    private static ConstructorInfo[] ResolveConstructors(Type type)
    {
        return type
            .GetConstructors()
            .Where(ctor =>
            {
                var parameters = ctor.GetParameters();
                return parameters.Length != 1 || parameters[0].ParameterType != type;
            })
            .ToArray();
    }

    private static BindingParameter[] BuildBindingParameters(ConstructorInfo? constructor, Type type)
    {
        var parameters = new List<BindingParameter>(32);

        if (constructor != null)
        {
            parameters.AddRange(constructor.GetParameters().Select(CreateBindingParameter));       
        }

        var constructorParameterNames = new HashSet<string>(parameters.Select(parameter => parameter.MetadataName));
        
        parameters.AddRange(type.GetProperties()
            .Where(property => !constructorParameterNames.Contains(property.Name))
            .Select(property => CreateBindingParameter(property, property.PropertyType)));
        
        parameters.AddRange(type.GetFields()
            .Where(field => !constructorParameterNames.Contains(field.Name))
            .Select(field => CreateBindingParameter(field, field.FieldType)));

        return parameters.ToArray();
    }

    private static BindingParameter CreateBindingParameter(ParameterInfo parameter)
    {
        return new BindingParameter(
            parameter.GetCustomAttribute<BindToAttribute>()?.BindingId ?? parameter.Name ?? "<none>",
            parameter.Name ?? "<none>",
            parameter.ParameterType,
            parameter);
    }
    
    private static BindingParameter CreateBindingParameter(MemberInfo member, Type type)
    {
        return new BindingParameter(
            member.GetCustomAttribute<BindToAttribute>()?.BindingId ?? member.Name,
            member.Name,
            type,
            member);
    }
}