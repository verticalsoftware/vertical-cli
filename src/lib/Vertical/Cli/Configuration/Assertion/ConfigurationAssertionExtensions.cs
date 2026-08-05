using Vertical.Cli.Configuration.Assertion.Types;

namespace Vertical.Cli.Configuration.Assertion;

public static class ConfigurationAssertionExtensions
{
    /// <summary>
    /// Returns any assertions detected in the configuration of the given application.
    /// </summary>
    /// <param name="app">The application instance.</param>
    /// <returns>A collection of zero or more assertions.</returns>
    public static IReadOnlyCollection<ConfigurationAssertion> GetConfigurationAssertions(this CommandLineApplication app)
    {
        var context = new AssertionContext(app);
        
        foreach (var builder in BuilderFactory.CreateBuilders())
        {
            builder.Build(context);
        }

        return context.Assertions;
    }

    /// <summary>
    /// Throws an exception if configuration assertions are found.
    /// </summary>
    /// <param name="app">The application instance.</param>
    /// <exception cref="InvalidOperationException">One or more assertions found.</exception>
    public static void AssertConfiguration(this CommandLineApplication app)
    {
        var assertions = app.GetConfigurationAssertions();

        if (assertions.Count == 0)
            return;

        throw new InvalidOperationException(ConfigurationAssertion.GetAssertionsAsText(assertions));
    }
}