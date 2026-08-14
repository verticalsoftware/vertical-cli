namespace Vertical.Cli.Configuration.Assertion.Builders;

/// <summary>
/// Represents an object that builds assertions.
/// </summary>
public interface IAssertionBuilder
{
    /// <summary>
    /// Uses data in the context to build assertions.
    /// </summary>
    /// <param name="context">The assertion context.</param>
    void Build(AssertionContext context);
}