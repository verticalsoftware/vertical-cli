using Vertical.Cli.Configuration.Assertion.Builders;

namespace Vertical.Cli.Configuration.Assertion;

/// <summary>
/// Defines the configuration assertion framework.
/// </summary>
internal static class AssertionFramework
{
    private static readonly IAssertionBuilder[] _assertionBuilderStack =
    [
        new AmbiguousArgumentOrdinalPositionsBuilder(),
        new DeadEndCommandAssertionBuilder(),
        new DuplicateAliasAssertionBuilder(),
        new DuplicateCommandNameAssertionBuilder(),
        new DuplicatePropertyBindingAssertionBuilder(),
        new MissingConverterAssertionBuilder(),
        new MissingModelBinderAssertionBuilder(),
        new MissingPropertyBindingAssertionBuilder(),
        new MultipleVariadicArgumentsAssertionBuilder(),
        new VariadicArgumentPositionAssertionBuilder()
    ];

    /// <summary>
    /// Gets the assertion builders.
    /// </summary>
    public static IAssertionBuilder[] GetBuilders() => _assertionBuilderStack.ToArray();
}