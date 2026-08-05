using Vertical.Cli.Configuration.Assertion.Types;

namespace Vertical.Cli.Configuration.Assertion.Builders;

internal sealed class MissingPropertyBindingsBuilder : IAssertionBuilder
{
    /// <inheritdoc />
    public void Build(AssertionContext context)
    {
        foreach (var command in context.CallSites)
        {
            ValidatePropertyBindings(context, command);
        }
    }

    private static void ValidatePropertyBindings(AssertionContext context, Command command)
    {
        var modelType = command.ModelType!;
        var configuration = context.GetModelConfiguration(modelType);
        var properties = modelType.GetProperties();
        var boundPropertyNames = configuration
            .BindingSources
            .Select(source => source.BindingName)
            .ToHashSet();
        
        context
            .Assertions
            .AddRange(properties
                .Where(property => !boundPropertyNames.Contains(property.Name))
                .Select(property => new MissingPropertyBindingAssertion(modelType, property)));
    }
}