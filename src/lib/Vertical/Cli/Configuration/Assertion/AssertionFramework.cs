using System.Collections.Concurrent;
using Vertical.Cli.Configuration.Assertion.Builders;

namespace Vertical.Cli.Configuration.Assertion;

/// <summary>
/// Defines the configuration assertion framework.
/// </summary>
public static class AssertionFramework
{
    private static readonly ConcurrentStack<IAssertionBuilder> _assertionBuilderStack = new(
    [
        new AmbiguousArgumentOrdinalPositionsBuilder(),
        new DeadEndCommandsBuilder(),
        new DuplicateAliasesBuilder(),
        new InvalidVariadicArgumentsBuilder(),
        new MissingConvertersBuilder(),
        new MissingModelBindersBuilder(),
        new MissingPropertyBindingsBuilder(),
        new MultiplePropertyBindingsBuilder(),
        new UniqueCommandNamesBuilder()
    ]);

    /// <summary>
    /// Gets the assertion builders.
    /// </summary>
    public static IAssertionBuilder[] GetBuilders() => _assertionBuilderStack.ToArray();

    /// <summary>
    /// Registers an assertion builder.
    /// </summary>
    /// <param name="builder">The builder to add.</param>
    public static void AddBuilder(IAssertionBuilder builder) => _assertionBuilderStack.Push(
        builder ?? throw new ArgumentNullException(nameof(builder)));
}