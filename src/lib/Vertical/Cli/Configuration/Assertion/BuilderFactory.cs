using Vertical.Cli.Configuration.Assertion.Builders;

namespace Vertical.Cli.Configuration.Assertion;

internal static class BuilderFactory
{
    public static IAssertionBuilder[] CreateBuilders() =>
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
    ];
}