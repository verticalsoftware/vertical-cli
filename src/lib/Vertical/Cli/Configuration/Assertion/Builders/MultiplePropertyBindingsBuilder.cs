using Vertical.Cli.Configuration.Assertion.Types;

namespace Vertical.Cli.Configuration.Assertion.Builders;

internal sealed class MultiplePropertyBindingsBuilder : IAssertionBuilder
{
    /// <inheritdoc />
    public void Build(AssertionContext context)
    {
        foreach (var command in context.CallSites)
        {
            FindDuplicateBindings(context, command);
        }
    }

    private static void FindDuplicateBindings(AssertionContext context, Command command)
    {
        var bindings = context
            .GetModelConfiguration(command.ModelType!)
            .BindingSources;

        var duplicates = bindings
            .GroupBy(binding => binding.BindingName)
            .Where(group => group.Count() > 1);
        
        context.Assertions.AddRange(
            duplicates
                .Select(group => new DuplicatePropertyBindingAssertion(
                    command.ModelType!, 
                    group.Key,
                    group.ToArray())));
    }
}