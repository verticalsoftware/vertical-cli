using Vertical.Cli.Configuration.Assertion.Types;

namespace Vertical.Cli.Configuration.Assertion.Builders;

internal sealed class CallSiteServiceContextBuilder : IAssertionBuilder
{
    /// <inheritdoc />
    public void Build(AssertionContext context)
    {
        var serviceProviderCallSites = context
            .CallSites
            .Where(command => command.RequiresServices)
            .ToArray();

        if (serviceProviderCallSites.Length == 0 || context.Configuration.HasClientServiceContext)
            return;

        context.Assertions.Add(new DefaultServiceContextAssertion(serviceProviderCallSites));
    }
}