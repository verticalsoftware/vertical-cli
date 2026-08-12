namespace Vertical.Cli.Configuration.Assertion.Builders;

/// <summary>
/// Represents an object that builds assertions.
/// </summary>
public interface IAssertionBuilder
{
    void Build(AssertionContext context);
}