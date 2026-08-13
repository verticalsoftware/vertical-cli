using Vertical.Cli.Configuration.Assertion.Types;

namespace Vertical.Cli.Configuration.Assertion;

/// <summary>
/// Extends <see cref="CommandLineApplication"/> with assertion functions.
/// </summary>
public static class ConfigurationAssertionExtensions
{
    /// <param name="app">The application instance.</param>
    extension(CommandLineApplication app)
    {
        /// <summary>
        /// Returns any assertions detected in the configuration of the given application.
        /// </summary>
        /// <returns>A collection of zero or more assertions.</returns>
        public IReadOnlyCollection<ConfigurationAssertion> GetConfigurationAssertions()
        {
            var context = new AssertionContext(app);
            var builders = AssertionFramework.GetBuilders();
        
            foreach (var builder in builders)
            {
                builder.Build(context);
            }

            return context.Assertions;
        }

        /// <summary>
        /// Throws an exception if configuration assertions are found.
        /// </summary>
        /// <exception cref="InvalidOperationException">One or more assertions found.</exception>
        public void AssertConfiguration()
        {
            var assertions = app.GetConfigurationAssertions();

            if (assertions.Count == 0)
                return;

            throw new InvalidOperationException(ConfigurationAssertion.GetAssertionsAsText(assertions));
        }
    }
}